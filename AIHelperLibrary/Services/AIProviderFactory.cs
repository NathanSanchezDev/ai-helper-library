using System;
using AIHelperLibrary.Abstractions;
using AIHelperLibrary.Configurations;

namespace AIHelperLibrary.Services
{
    /// <summary>
    /// Factory for creating AI client instances based on provider configuration.
    /// </summary>
    public class AIProviderFactory : IAIProviderFactory
    {
        /// <summary>
        /// Creates an AI client instance for the specified provider.
        /// </summary>
        /// <param name="apiKey">The API key for the service.</param>
        /// <param name="config">The configuration object for the client.</param>
        /// <returns>An implementation of IAIClient for the specified provider.</returns>
        public IAIClient CreateClient(string apiKey, AIBaseConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
                
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));
                
            return config.Provider switch
            {
                Models.AIProvider.OpenAI => 
                    new OpenAIClient(apiKey, config as AIExtensionHelperConfiguration ?? 
                        throw new ArgumentException("Invalid configuration type for OpenAI provider.")),
                        
                Models.AIProvider.Anthropic => 
                    new ClaudeClient(apiKey, config as AnthropicConfiguration ?? 
                        throw new ArgumentException("Invalid configuration type for Anthropic provider.")),
                        
                _ => throw new ArgumentException($"Unsupported AI provider: {config.Provider}")
            };
        }
    }
}