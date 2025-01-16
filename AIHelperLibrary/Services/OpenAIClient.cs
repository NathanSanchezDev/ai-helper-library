using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AIHelperLibrary.Configurations;
using AIHelperLibrary.Models;
using AIHelperLibrary.Prompts;

namespace AIHelperLibrary.Services
{
    public class OpenAIClient
    {
        private readonly string _apiKey;
        private readonly AIExtensionHelperConfiguration _config;
        private const string BaseUrl = "https://api.openai.com/v1";

        // Maintain chat histories for instances
        private readonly Dictionary<string, List<object>> _chatHistories = new();

        public OpenAIClient(string apiKey, AIExtensionHelperConfiguration config)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        private string GetModelString()
        {
            return _config.DefaultModel switch
            {
                AIModel.GPT_4 => "gpt-4",
                AIModel.GPT_3_5_Turbo => "gpt-3.5-turbo",
                _ => throw new ArgumentException("Invalid AI model.")
            };
        }

        public async Task<string> GenerateTextAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));

            var requestBody = new
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

            using var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(_config.RequestTimeoutMs) };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            foreach (var header in _config.CustomHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            try
            {
                var response = await client.PostAsync($"{BaseUrl}/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                if (result == null)
                    throw new InvalidOperationException("The response content was null.");

                dynamic json = JsonConvert.DeserializeObject(result);
                if (json?.choices == null || json.choices.Count == 0 || json.choices[0]?.message?.content == null)
                    throw new InvalidOperationException("Unexpected response format.");

                return json.choices[0].message.content;
            }
            catch (HttpRequestException ex)
            {
                if (_config.EnableLogging)
                {
                    Console.WriteLine($"[ERROR] Request failed: {ex.Message}");
                }
                throw;
            }
        }

        public async Task<string> GenerateTextWithPredefinedPromptAsync(string predefinedPrompt, string userInput)
        {
            if (string.IsNullOrWhiteSpace(predefinedPrompt))
                throw new ArgumentException("Predefined prompt cannot be null or empty.", nameof(predefinedPrompt));

            if (string.IsNullOrWhiteSpace(userInput))
                throw new ArgumentException("User input cannot be null or empty.", nameof(userInput));

            var prompt = $"{predefinedPrompt}\n{userInput}";
            return await GenerateTextAsync(prompt);
        }

        public async Task<string> GenerateTextWithDynamicPromptAsync(DynamicPromptManager promptManager, string key, string userInput)
        {
            if (promptManager == null)
                throw new ArgumentNullException(nameof(promptManager));

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Dynamic prompt key cannot be null or empty.", nameof(key));

            var predefinedPrompt = promptManager.GetPrompt(key);
            if (string.IsNullOrWhiteSpace(predefinedPrompt))
                throw new ArgumentException($"Dynamic prompt for key '{key}' not found or empty.", nameof(predefinedPrompt));

            if (string.IsNullOrWhiteSpace(userInput))
                throw new ArgumentException("User input cannot be null or empty.", nameof(userInput));

            var prompt = $"{predefinedPrompt}\n{userInput}";
            return await GenerateTextAsync(prompt);
        }

        public async Task<string> GenerateChatResponseAsync(string instanceKey, string userMessage, string initialPrompt)
        {
            if (string.IsNullOrWhiteSpace(instanceKey))
                throw new ArgumentException("Instance key cannot be null or empty.", nameof(instanceKey));

            if (string.IsNullOrWhiteSpace(userMessage))
                throw new ArgumentException("User message cannot be null or empty.", nameof(userMessage));

            if (!_chatHistories.ContainsKey(instanceKey))
            {
                _chatHistories[instanceKey] = new List<object>
                {
                    new { role = "system", content = initialPrompt }
                };
            }

            _chatHistories[instanceKey].Add(new { role = "user", content = userMessage });

            var requestBody = new
            {
                model = GetModelString(),
                messages = _chatHistories[instanceKey],
                max_tokens = _config.MaxTokens,
                temperature = _config.Temperature,
                top_p = _config.TopP
            };

            using var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(_config.RequestTimeoutMs) };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            foreach (var header in _config.CustomHeaders)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = await client.PostAsync($"{BaseUrl}/chat/completions",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            if (result == null)
                throw new InvalidOperationException("The response content was null.");

            dynamic json = JsonConvert.DeserializeObject(result);
            if (json?.choices == null || json.choices.Count == 0 || json.choices[0]?.message?.content == null)
                throw new InvalidOperationException("Unexpected response format.");

            var assistantMessage = json.choices[0].message.content;
            _chatHistories[instanceKey].Add(new { role = "assistant", content = assistantMessage });

            return assistantMessage;
        }
    }
}