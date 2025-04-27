using System.Text;
using Newtonsoft.Json;
using AIHelperLibrary.Configurations;
using AIHelperLibrary.Models;
using AIHelperLibrary.Prompts;
using AIHelperLibrary.Abstractions;
using AIHelperLibrary.Helpers;
using System.Net;

namespace AIHelperLibrary.Services;

public class OpenAIClient : IAIClient
{
    private readonly string _apiKey;
    private readonly OpenAIConfiguration _config;
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.openai.com/v1";
    private readonly Dictionary<string, List<object>> _chatHistories = new();

    public OpenAIClient(string apiKey, OpenAIConfiguration config)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _httpClient = CreateHttpClient();
    }

    private HttpClient CreateHttpClient()
    {
        var handler = new HttpClientHandler();
        if (!string.IsNullOrWhiteSpace(_config.ProxyUrl))
        {
            handler.Proxy = new WebProxy(_config.ProxyUrl, _config.ProxyPort);
            handler.UseProxy = true;
        }
        return new HttpClient(handler);
    }

    private string GetModelString() => _config.DefaultModel.GetModelIdentifier();

    public async Task<string> GenerateTextAsync(string prompt)
    {
        ValidateInput(prompt, nameof(prompt));
        var requestBody = BuildRequestBody(prompt);
        return await ExecuteRequestAsync(requestBody);
    }

    public async Task<string> GenerateTextWithPredefinedPromptAsync(string predefinedPrompt, string userInput)
    {
        ValidateInput(predefinedPrompt, nameof(predefinedPrompt));
        ValidateInput(userInput, nameof(userInput));
        var combinedPrompt = $"{predefinedPrompt}\n{userInput}";
        return await GenerateTextAsync(combinedPrompt);
    }

    public async Task<string> GenerateChatResponseAsync(string instanceKey, string userMessage, string initialPrompt)
    {
        ValidateInput(instanceKey, nameof(instanceKey));
        ValidateInput(userMessage, nameof(userMessage));

        var modelId = GetModelString();
        OpenAIModelHelper.EnsureChatModel(modelId);

        if (!_chatHistories.ContainsKey(instanceKey))
        {
            _chatHistories[instanceKey] = new List<object>
            {
                new { role = "system", content = initialPrompt }
            };
        }

        _chatHistories[instanceKey].Add(new { role = "user", content = userMessage });

        if (_chatHistories[instanceKey].Count > _config.MaxChatHistorySize)
        {
            _chatHistories[instanceKey].RemoveAt(0);
        }

        var requestBody = BuildChatRequestBody(_chatHistories[instanceKey]);
        return await ExecuteRequestAsync(requestBody, forceChat: true);
    }

    public async Task<string> GenerateTextWithDynamicPromptAsync(DynamicPromptManager promptManager, string key, string userInput)
    {
        if (promptManager == null)
            throw new ArgumentNullException(nameof(promptManager));

        ValidateInput(key, nameof(key));
        ValidateInput(userInput, nameof(userInput));

        var predefinedPrompt = promptManager.GetPrompt(key);
        ValidateInput(predefinedPrompt, nameof(predefinedPrompt));

        return await GenerateTextAsync(predefinedPrompt + "\n" + userInput);
    }

    private async Task<string> ExecuteRequestAsync(object requestBody, bool? forceChat = null)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        foreach (var header in _config.CustomHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }

        var modelId = GetModelString();
        bool isChat = forceChat ?? OpenAIModelHelper.IsChatModel(modelId);
        string endpoint = isChat ? "/chat/completions" : "/completions";

        var requestJson = JsonConvert.SerializeObject(requestBody);
        if (_config.EnableLogging)
        {
            Console.WriteLine($"Request: {requestJson}");
        }

        var response = await ExecuteWithRetryAsync(() =>
            _httpClient.PostAsync($"{BaseUrl}{endpoint}",
                new StringContent(requestJson, Encoding.UTF8, "application/json")));

        if (_config.EnableLogging)
        {
            Console.WriteLine($"Response: {response}");
        }

        var result = JsonConvert.DeserializeObject<dynamic>(response);

        if (result?.choices?[0]?.message?.content != null)
        {
            return result.choices[0].message.content.ToString();
        }
        else if (result?.choices?[0]?.text != null)
        {
            return result.choices[0].text.ToString();
        }
        else
        {
            throw new InvalidOperationException("Unexpected response format.");
        }
    }

    private async Task<string> ExecuteWithRetryAsync(Func<Task<HttpResponseMessage>> action)
    {
        for (int attempt = 0; attempt < _config.MaxRetryCount; attempt++)
        {
            try
            {
                var response = await action();
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    throw new HttpRequestException($"Non-retriable error: {response.StatusCode}");
                }
            }
            catch (Exception ex) when (attempt < _config.MaxRetryCount - 1)
            {
                if (_config.EnableLogging)
                {
                    Console.WriteLine($"Retry {attempt + 1}/{_config.MaxRetryCount} failed: {ex.Message}");
                }
                await Task.Delay(_config.RetryDelayMs);
            }
        }
        throw new Exception("Exceeded maximum retry attempts.");
    }

    private object BuildRequestBody(string prompt)
    {
        var modelId = GetModelString();
        bool isChat = OpenAIModelHelper.IsChatModel(modelId);
        bool isOModel = modelId.StartsWith("o1") || modelId.StartsWith("o3") || modelId.StartsWith("o4");

        if (isChat)
        {
            var baseChatRequest = new
            {
                model = modelId,
                messages = new[] { new { role = "user", content = prompt } }
            };

            if (isOModel)
            {
                return new
                {
                    baseChatRequest.model,
                    baseChatRequest.messages,
                    max_completion_tokens = _config.MaxTokens
                };
            }
            else
            {
                return new
                {
                    baseChatRequest.model,
                    baseChatRequest.messages,
                    max_tokens = _config.MaxTokens,
                    temperature = _config.Temperature,
                    top_p = _config.TopP
                };
            }
        }
        else
        {
            return new
            {
                model = modelId,
                prompt = prompt,
                max_tokens = _config.MaxTokens,
                temperature = _config.Temperature,
                top_p = _config.TopP
            };
        }
    }

    private object BuildChatRequestBody(List<object> messages)
    {
        var modelId = GetModelString();
        bool isOModel = modelId.StartsWith("o1") || modelId.StartsWith("o3") || modelId.StartsWith("o4");

        if (isOModel)
        {
            return new
            {
                model = modelId,
                messages = messages,
                max_completion_tokens = _config.MaxTokens
            };
        }
        else
        {
            return new
            {
                model = modelId,
                messages = messages,
                max_tokens = _config.MaxTokens,
                temperature = _config.Temperature,
                top_p = _config.TopP
            };
        }
    }

    private static void ValidateInput(string input, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);
        }
    }
}