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
    /// <summary>
    /// A client for interacting with the OpenAI API.
    /// Supports predefined, dynamic prompts and context-aware chat sessions.
    /// </summary>
    public class OpenAIClient
    {
        private readonly string _apiKey;
        private readonly AIExtensionHelperConfiguration _config;
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string BaseUrl = "https://api.openai.com/v1";

        // Maintain chat histories for instances
        private readonly Dictionary<string, List<object>> _chatHistories = new();

        /// <summary>
        /// Initializes a new instance of the OpenAIClient.
        /// </summary>
        /// <param name="apiKey">Your OpenAI API key.</param>
        /// <param name="config">The configuration for the client.</param>
        public OpenAIClient(string apiKey, AIExtensionHelperConfiguration config)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Maps the configured model to its corresponding OpenAI model string.
        /// </summary>
        /// <returns>The OpenAI model string.</returns>
        private string GetModelString()
        {
            return _config.DefaultModel switch
            {
                AIModel.GPT_4 => "gpt-4",
                AIModel.GPT_3_5_Turbo => "gpt-3.5-turbo",
                _ => throw new ArgumentException("Invalid AI model.")
            };
        }

        /// <summary>
        /// Sends a text prompt to the OpenAI API and retrieves the response.
        /// </summary>
        /// <param name="prompt">The user prompt.</param>
        /// <returns>The AI-generated response.</returns>
        /// <exception cref="ArgumentException">Thrown when the prompt is null or empty.</exception>
        public async Task<string> GenerateTextAsync(string prompt)
        {
            ValidateInput(prompt, nameof(prompt));

            var requestBody = BuildRequestBody(prompt);
            return await ExecuteRequestAsync(requestBody);
        }

        /// <summary>
        /// Sends a predefined prompt along with user input to the OpenAI API.
        /// </summary>
        /// <param name="predefinedPrompt">The predefined prompt template.</param>
        /// <param name="userInput">The user input to append to the predefined prompt.</param>
        /// <returns>The AI-generated response.</returns>
        public async Task<string> GenerateTextWithPredefinedPromptAsync(string predefinedPrompt, string userInput)
        {
            ValidateInput(predefinedPrompt, nameof(predefinedPrompt));
            ValidateInput(userInput, nameof(userInput));

            var prompt = $"{predefinedPrompt}\n{userInput}";
            return await GenerateTextAsync(prompt);
        }

        /// <summary>
        /// Sends a dynamic prompt along with user input to the OpenAI API.
        /// </summary>
        /// <param name="promptManager">The manager responsible for storing dynamic prompts.</param>
        /// <param name="key">The key identifying the dynamic prompt.</param>
        /// <param name="userInput">The user input to append to the dynamic prompt.</param>
        /// <returns>The AI-generated response.</returns>
        public async Task<string> GenerateTextWithDynamicPromptAsync(DynamicPromptManager promptManager, string key, string userInput)
        {
            if (promptManager == null)
                throw new ArgumentNullException(nameof(promptManager));

            ValidateInput(key, nameof(key));
            ValidateInput(userInput, nameof(userInput));

            var predefinedPrompt = promptManager.GetPrompt(key);
            ValidateInput(predefinedPrompt, nameof(predefinedPrompt));

            var prompt = $"{predefinedPrompt}\n{userInput}";
            return await GenerateTextAsync(prompt);
        }

        /// <summary>
        /// Handles multi-turn conversations, maintaining chat context.
        /// </summary>
        /// <param name="instanceKey">A unique key identifying the chat instance.</param>
        /// <param name="userMessage">The user's message.</param>
        /// <param name="initialPrompt">The system's initial prompt.</param>
        /// <returns>The AI-generated response.</returns>
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

        /// <summary>
        /// Validates that the input string is not null, empty, or whitespace.
        /// </summary>
        private static void ValidateInput(string input, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);
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
        /// Executes the API request and processes the response.
        /// </summary>
        private async Task<string> ExecuteRequestAsync(object requestBody)
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            foreach (var header in _config.CustomHeaders)
            {
                HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var response = await HttpClient.PostAsync($"{BaseUrl}/chat/completions",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            dynamic? json = JsonConvert.DeserializeObject(result);

            if (json?.choices == null || json.choices.Count == 0 || json.choices[0]?.message?.content == null)
                throw new InvalidOperationException("Unexpected response format or content is null.");

            return json.choices[0].message.content;
        }
    }
}