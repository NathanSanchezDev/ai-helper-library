namespace AIHelperLibrary.Models;
/// <summary>
/// Anthropic Claude model identifiers.
/// </summary>
public enum AnthropicModel : int
{
    // Claude 3.7 Family (Latest)
    Claude3_7_Sonnet,

    // Claude 3.5 Family 
    Claude3_5_Sonnet,
    Claude3_5_Haiku,

    // Claude 3 Family
    Claude3Opus,
    Claude3Sonnet,
    Claude3Haiku,

    // Legacy Models
    Claude2_1,
    Claude2,
    ClaudeInstant1_2,
    ClaudeInstant1
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
            // Latest Models
            AnthropicModel.Claude3_7_Sonnet => "claude-3-7-sonnet-20250219",

            // Claude 3.5 Family
            AnthropicModel.Claude3_5_Sonnet => "claude-3-5-sonnet-20240620",
            AnthropicModel.Claude3_5_Haiku => "claude-3-5-haiku-20241022",

            // Claude 3 Family
            AnthropicModel.Claude3Opus => "claude-3-opus-20240229",
            AnthropicModel.Claude3Sonnet => "claude-3-sonnet-20240229",
            AnthropicModel.Claude3Haiku => "claude-3-haiku-20240307",

            // Legacy Models
            AnthropicModel.Claude2_1 => "claude-2.1",
            AnthropicModel.Claude2 => "claude-2",
            AnthropicModel.ClaudeInstant1_2 => "claude-instant-1.2",
            AnthropicModel.ClaudeInstant1 => "claude-instant-1",

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