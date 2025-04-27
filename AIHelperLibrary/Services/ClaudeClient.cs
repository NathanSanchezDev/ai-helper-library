using System.Net;
using System.Text;
using Newtonsoft.Json;
using AIHelperLibrary.Abstractions;
using AIHelperLibrary.Configurations;
using AIHelperLibrary.Models;
using AIHelperLibrary.Prompts;

namespace AIHelperLibrary.Services
{
    /// <summary>
    /// A client for interacting with the Anthropic Claude API, implementing the <see cref="IAIClient"/> interface.
    /// </summary>
    public class ClaudeClient : IAIClient
    {
        private readonly string _apiKey;
        private readonly AnthropicConfiguration _config;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.anthropic.com/v1";
        private readonly Dictionary<string, List<object>> _chatHistories = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaudeClient"/> class.
        /// </summary>
        /// <param name="apiKey">Your Anthropic API key.</param>
        /// <param name="config">The configuration for the client.</param>
        public ClaudeClient(string apiKey, AnthropicConfiguration config)
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

        /// <inheritdoc />
        public async Task<string> GenerateTextAsync(string prompt)
        {
            ValidateInput(prompt, nameof(prompt));

            var messages = new List<object>
            {
                new { role = "user", content = prompt }
            };

            return await ExecuteMessagesRequestAsync(messages);
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
                _chatHistories[instanceKey] = new List<object>();
            }

            var messages = new List<object>(_chatHistories[instanceKey])
            {
                new { role = "user", content = userMessage }
            };

            // Maintain history size limit
            if (messages.Count > _config.MaxChatHistorySize)
            {
                // Remove oldest messages but keep system prompt if it exists
                int removeCount = messages.Count - _config.MaxChatHistorySize;
                messages.RemoveRange(0, removeCount);
            }

            var response = await ExecuteMessagesRequestAsync(messages, initialPrompt);

            // Update chat history
            _chatHistories[instanceKey] = new List<object>(messages)
            {
                new { role = "assistant", content = response }
            };

            return response;
        }

        /// <summary>
        /// Executes a request to the Claude API with messages format.
        /// </summary>
        private async Task<string> ExecuteMessagesRequestAsync(List<object> messages, string systemPrompt = null)
        {
            var requestBody = new
            {
                model = _config.DefaultModel.GetModelIdentifier(),
                messages = messages,
                system = string.IsNullOrEmpty(systemPrompt) ? _config.SystemPrompt : systemPrompt,
                max_tokens = _config.MaxTokens,
                temperature = _config.Temperature,
                top_p = _config.TopP,
                stop_sequences = _config.StopSequences.Count > 0 ? _config.StopSequences : null
            };

            return await ExecuteRequestAsync(requestBody);
        }

        /// <summary>
        /// Executes an HTTP request to the Claude API with retry logic.
        /// </summary>
        private async Task<string> ExecuteRequestAsync(object requestBody)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", _config.ApiVersion);
            _httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json");

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
                _httpClient.PostAsync($"{BaseUrl}/messages",
                    new StringContent(requestJson, Encoding.UTF8, "application/json")));

            if (_config.EnableLogging)
            {
                Console.WriteLine($"Response: {response}");
            }

            var result = JsonConvert.DeserializeObject<dynamic>(response);
            if (result?.content == null)
            {
                throw new InvalidOperationException("Unexpected response format from Claude API.");
            }

            // Extract text from the response
            string content = string.Empty;
            foreach (var block in result.content)
            {
                if (block.type == "text")
                {
                    content += block.text.ToString();
                }
            }

            return content;
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

                    // Handle rate limiting (429)
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        int retryAfterMs = 1000;
                        if (response.Headers.TryGetValues("retry-after", out var values))
                        {
                            if (int.TryParse(values.FirstOrDefault(), out int seconds))
                            {
                                retryAfterMs = seconds * 1000;
                            }
                        }

                        if (_config.EnableLogging)
                        {
                            Console.WriteLine($"Rate limited. Waiting {retryAfterMs}ms before retry.");
                        }

                        await Task.Delay(retryAfterMs);
                        continue;
                    }

                    // Non-retriable client errors
                    if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Claude API error: {response.StatusCode}. {errorContent}");
                    }
                }
                catch (Exception ex) when (attempt < _config.MaxRetryCount - 1 &&
                                         !(ex is HttpRequestException && ex.Message.Contains("Claude API error")))
                {
                    if (_config.EnableLogging)
                    {
                        Console.WriteLine($"Retry {attempt + 1}/{_config.MaxRetryCount} failed: {ex.Message}");
                    }
                    await Task.Delay(_config.RetryDelayMs);
                }
            }
            throw new Exception("Exceeded maximum retry attempts for Claude API.");
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
        /// <inheritdoc />
        public async Task<string> GenerateTextWithDynamicPromptAsync(DynamicPromptManager promptManager, string key, string userInput)
        {
            if (promptManager == null)
                throw new ArgumentNullException(nameof(promptManager));

            ValidateInput(key, nameof(key));
            ValidateInput(userInput, nameof(userInput));

            // Retrieve the prompt template from the manager
            var predefinedPrompt = promptManager.GetPrompt(key);
            ValidateInput(predefinedPrompt, nameof(predefinedPrompt));

            // For Claude, we format the request using the messages format
            var messages = new List<object>
            {
                new { role = "user", content = $"{predefinedPrompt}\n{userInput}" }
            };

            // Execute the request using our existing messages request method
            return await ExecuteMessagesRequestAsync(messages);
        }
    }
}