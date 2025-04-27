using System;
using AIHelperLibrary.Abstractions;

namespace AIHelperLibrary.Models
{
    /// <summary>
    /// Anthropic Claude model identifiers.
    /// </summary>
    public enum AnthropicModel : int
    {
        // Claude 3 Family
        Claude3Opus,
        Claude3Sonnet,
        Claude3Haiku,
        
        // Claude 3.5 Family
        Claude3_5_Sonnet,
        
        // Claude 2 Family (Legacy)
        Claude2,
        Claude2_1,
        
        // Claude Instant Family (Legacy)
        ClaudeInstant1,
        ClaudeInstant1_2
    }
    
    /// <summary>
    /// Extension methods for AnthropicModel enum.
    /// </summary>
    public static class AnthropicModelExtensions
    {
        /// <summary>
        /// Maps the AnthropicModel enum to its corresponding API model identifier string.
        /// </summary>
        /// <param name="model">The Anthropic model enum value.</param>
        /// <returns>The model identifier string for API requests.</returns>
        public static string GetModelIdentifier(this AnthropicModel model)
        {
            return model switch
            {
                AnthropicModel.Claude3Opus => "claude-3-opus-20240229",
                AnthropicModel.Claude3Sonnet => "claude-3-sonnet-20240229",
                AnthropicModel.Claude3Haiku => "claude-3-haiku-20240307",
                AnthropicModel.Claude3_5_Sonnet => "claude-3-5-sonnet-20240425",
                AnthropicModel.Claude2 => "claude-2",
                AnthropicModel.Claude2_1 => "claude-2.1",
                AnthropicModel.ClaudeInstant1 => "claude-instant-1",
                AnthropicModel.ClaudeInstant1_2 => "claude-instant-1.2",
                _ => throw new ArgumentException($"Unknown Anthropic model: {model}")
            };
        }
        
        /// <summary>
        /// Returns the provider for Anthropic models.
        /// </summary>
        /// <param name="model">The Anthropic model enum value.</param>
        /// <returns>The AIProvider enum value for Anthropic.</returns>
        public static AIProvider GetProvider(this AnthropicModel model)
        {
            return AIProvider.Anthropic;
        }
    }
}