namespace AIHelperLibrary.Models
{
    /// <summary>
    /// Defines the supported AI service providers.
    /// </summary>
    public enum AIProvider
    {
        /// <summary>
        /// OpenAI API provider (GPT models)
        /// </summary>
        OpenAI,
        
        /// <summary>
        /// Anthropic API provider (Claude models)
        /// </summary>
        Anthropic,
        
        /// <summary>
        /// Cohere API provider
        /// </summary>
        Cohere
    }
}