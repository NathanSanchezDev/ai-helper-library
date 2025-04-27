using System.Collections.Generic;
using AIHelperLibrary.Models;

namespace AIHelperLibrary.Configurations
{
    /// <summary>
    /// Configuration options specific to the Anthropic Claude API.
    /// </summary>
    public class AnthropicConfiguration : AIBaseConfiguration
    {
        /// <summary>
        /// Gets the AI provider for this configuration.
        /// </summary>
        public override AIProvider Provider => AIProvider.Anthropic;
        
        /// <summary>
        /// Specifies the Anthropic API version to use.
        /// Default is "2023-06-01".
        /// </summary>
        public string ApiVersion { get; set; } = "2023-06-01";
        
        /// <summary>
        /// Specifies the default Anthropic model to use for requests.
        /// Default is Claude3Sonnet.
        /// </summary>
        public AnthropicModel DefaultModel { get; set; } = AnthropicModel.Claude3Sonnet;
        
        /// <summary>
        /// Top-p sampling controls the diversity of responses.
        /// Range: 0.0 to 1.0, where 1.0 includes all possible options.
        /// Default is 1.0.
        /// </summary>
        public double TopP { get; set; } = 1.0;
        
        /// <summary>
        /// List of sequences that will stop generation when encountered.
        /// </summary>
        public List<string> StopSequences { get; set; } = new List<string>();
        
        /// <summary>
        /// System instructions for the Claude assistant's behavior.
        /// </summary>
        public string SystemPrompt { get; set; } = "You are Claude, an AI assistant created by Anthropic.";
    }
}