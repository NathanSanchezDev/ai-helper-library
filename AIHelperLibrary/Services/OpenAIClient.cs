using System.Text;
using Newtonsoft.Json;
using AIHelperLibrary.Configurations;
using AIHelperLibrary.Models;
using AIHelperLibrary.Prompts;
using AIHelperLibrary.Abstractions;
using System.Net;

namespace AIHelperLibrary.Services
{
    /// <summary>
    /// A client for interacting with the OpenAI API, implementing the <see cref="IAIClient"/> interface.
    /// Supports dynamic prompts, retry logic, and configuration options.
    /// </summary>
    public class OpenAIClient : IAIClient
    {
        private readonly string _apiKey;
        private readonly AIExtensionHelperConfiguration _config;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.openai.com/v1";
        private readonly Dictionary<string, List<object>> _chatHistories = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIClient"/> class.
        /// </summary>
        /// <param name="apiKey">Your OpenAI API key.</param>
        /// <param name="config">The configuration for the client.</param>
        public OpenAIClient(string apiKey, AIExtensionHelperConfiguration config)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpClient = CreateHttpClient();
        }

        /// <summary>
        /// Creates a configured HttpClient with optional proxy settings.
        /// </summary>
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

        /// <summary>
        /// Maps the configured model to its corresponding OpenAI model string.
        /// </summary>
        /// <returns>The OpenAI model string.</returns>
        private string GetModelString()
        {
            return _config.DefaultModel switch
            {
                // General Purpose Text Models
                AIModel.GPT_3_5_Turbo => "gpt-3.5-turbo",
                AIModel.GPT_3_5_Turbo_16k => "gpt-3.5-turbo-16k",
                AIModel.GPT_3_5_Turbo_0125 => "gpt-3.5-turbo-0125",
                AIModel.GPT_3_5_Turbo_Instruct => "gpt-3.5-turbo-instruct",
                AIModel.GPT_3_5_Turbo_Instruct_0914 => "gpt-3.5-turbo-instruct-0914",
                AIModel.GPT_3_5_Turbo_1106 => "gpt-3.5-turbo-1106",
                AIModel.GPT_4 => "gpt-4",
                AIModel.GPT_4_Turbo => "gpt-4-turbo",
                AIModel.GPT_4_Turbo_2024_04_09 => "gpt-4-turbo-2024-04-09",
                AIModel.GPT_4_Turbo_Preview => "gpt-4-turbo-preview",
                AIModel.GPT_4_0613 => "gpt-4-0613",
                AIModel.GPT_4_0125_Preview => "gpt-4-0125-preview",
                AIModel.GPT_4_1106_Preview => "gpt-4-1106-preview",

                // Optimized Models
                AIModel.GPT_4o => "gpt-4o",
                AIModel.GPT_4o_2024_08_06 => "gpt-4o-2024-08-06",
                AIModel.ChatGPT_4o_Latest => "chatgpt-4o-latest",
                AIModel.GPT_4o_Mini => "gpt-4o-mini",
                AIModel.GPT_4o_Mini_2024_07_18 => "gpt-4o-mini-2024-07-18",

                AIModel.O1 => "o1",
                AIModel.O1_2024_12_17 => "o1-2024-12-17",
                AIModel.O1_Mini => "o1-mini",
                AIModel.O1_Mini_2024_09_12 => "o1-mini-2024-09-12",
                AIModel.O3_Mini => "o3-mini",
                AIModel.O3_Mini_2025_01_31 => "o3-mini-2025-01-31",
                AIModel.O1_Preview => "o1-preview",
                AIModel.O1_Preview_2024_09_12 => "o1-preview-2024-09-12",

                AIModel.GPT_4o_RealTime_Preview => "gpt-4o-realtime-preview",
                AIModel.GPT_4o_RealTime_Preview_2024_10_01 => "gpt-4o-realtime-preview-2024-10-01",
                AIModel.GPT_4o_RealTime_Preview_2024_12_17 => "gpt-4o-realtime-preview-2024-12-17",

                AIModel.GPT_4o_Mini_RealTime_Preview => "gpt-4o-mini-realtime-preview",
                AIModel.GPT_4o_Mini_RealTime_Preview_2024_12_17 => "gpt-4o-mini-realtime-preview-2024-12-17",

                AIModel.GPT_4o_Audio_Preview => "gpt-4o-audio-preview",
                AIModel.GPT_4o_Audio_Preview_2024_12_17 => "gpt-4o-audio-preview-2024-12-17",

                AIModel.GPT_4o_2024_05_13 => "gpt-4o-2024-05-13",
                AIModel.GPT_4o_2024_11_20 => "gpt-4o-2024-11-20",

                _ => throw new ArgumentException("Invalid AI model.")
            };
        }

        /// <inheritdoc />
        public async Task<string> GenerateTextAsync(string prompt)
        {
            ValidateInput(prompt, nameof(prompt));
            var requestBody = BuildRequestBody(prompt);
            return await ExecuteRequestAsync(requestBody);
        }

        /// <inheritdoc />
        public async Task<string> GenerateTextWithPredefinedPromptAsync(string predefinedPrompt, string userInput)
        {
            ValidateInput(predefinedPrompt, nameof(predefinedPrompt));
            ValidateInput(userInput, nameof(userInput));
            var combinedPrompt = $"{predefinedPrompt}\n{userInput}";
            return await GenerateTextAsync(combinedPrompt);
        }

        /// <inheritdoc />
        public async Task<string> GenerateChatResponseAsync(string instanceKey, string userMessage, string initialPrompt)
        {
            ValidateInput(instanceKey, nameof(instanceKey));
            ValidateInput(userMessage, nameof(userMessage));

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

            var requestBody = new
            {
                model = GetModelString(),
                messages = _chatHistories[instanceKey],
                max_tokens = _config.MaxTokens,
                temperature = _config.Temperature,
                top_p = _config.TopP
            };

            return await ExecuteRequestAsync(requestBody);
        }

        /// <inheritdoc />
        public async Task<string> GenerateTextWithDynamicPromptAsync(DynamicPromptManager promptManager, string key, string userInput)
        {
            if (promptManager == null)
                throw new ArgumentNullException(nameof(promptManager));

            ValidateInput(key, nameof(key));
            ValidateInput(userInput, nameof(userInput));

            var predefinedPrompt = promptManager.GetPrompt(key);
            ValidateInput(predefinedPrompt, nameof(predefinedPrompt));

            var requestBody = new
            {
                model = GetModelString(),
                messages = new[]
                {
                    new { role = "system", content = predefinedPrompt },
                    new { role = "user", content = userInput }
                },
                max_tokens = _config.MaxTokens,
                temperature = _config.Temperature,
                top_p = _config.TopP
            };

            return await ExecuteRequestAsync(requestBody);
        }

        /// <summary>
        /// Executes an HTTP request with retry logic.
        /// </summary>
        private async Task<string> ExecuteRequestAsync(object requestBody)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            foreach (var header in _config.CustomHeaders)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var requestJson = JsonConvert.SerializeObject(requestBody);
            if (_config.EnableLogging)
            {
                Console.WriteLine($"Request: {requestJson}");
            }

            var response = await ExecuteWithRetryAsync(() =>
                _httpClient.PostAsync($"{BaseUrl}/chat/completions",
                    new StringContent(requestJson, Encoding.UTF8, "application/json")));

            if (_config.EnableLogging)
            {
                Console.WriteLine($"Response: {response}");
            }

            var result = JsonConvert.DeserializeObject<dynamic>(response);
            if (result?.choices?[0]?.message?.content == null)
            {
                throw new InvalidOperationException("Unexpected response format.");
            }
            return result.choices[0].message.content.ToString();
        }

        /// <summary>
        /// Executes a task with retry logic based on configuration settings.
        /// </summary>
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

        /// <summary>
        /// Builds the API request body.
        /// </summary>
        private object BuildRequestBody(string prompt)
        {
            return new
            {
                model = GetModelString(),
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = _config.MaxTokens,
                temperature = _config.Temperature,
                top_p = _config.TopP
            };
        }

        /// <summary>
        /// Validates input parameters.
        /// </summary>
        private static void ValidateInput(string input, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);
            }
        }
    }
}