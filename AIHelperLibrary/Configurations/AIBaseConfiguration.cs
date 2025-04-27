using System.Collections.Generic;
using AIHelperLibrary.Models;

namespace AIHelperLibrary.Configurations
{
    /// <summary>
    /// Base configuration options for AI clients across different providers.
    /// </summary>
    public abstract class AIBaseConfiguration
    {
        /// <summary>
        /// Gets the AI provider associated with this configuration.
        /// </summary>
        public abstract AIProvider Provider { get; }
        
        /// <summary>
        /// Maximum number of tokens allowed per response.
        /// Default is 150 tokens.
        /// </summary>
        public int MaxTokens { get; set; } = 150;
        
        /// <summary>
        /// Creativity level of the AI's responses.
        /// Range: 0.0 (deterministic) to 1.0 (highly creative).
        /// Default is 0.7.
        /// </summary>
        public double Temperature { get; set; } = 0.7;
        
        /// <summary>
        /// Timeout for API requests in milliseconds.
        /// Default is 10,000 milliseconds (10 seconds).
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 10000;
        
        /// <summary>
        /// Enables or disables logging of AI client activities.
        /// Default is false (logging is disabled).
        /// </summary>
        public bool EnableLogging { get; set; } = false;
        
        /// <summary>
        /// Optional custom headers for API requests.
        /// </summary>
        public Dictionary<string, string> CustomHeaders { get; set; } = new();
        
        /// <summary>
        /// Maximum number of retry attempts for API requests in case of transient failures.
        /// Default is 3 retries.
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;
        
        /// <summary>
        /// Delay in milliseconds between retry attempts.
        /// Default is 2,000 milliseconds (2 seconds).
        /// </summary>
        public int RetryDelayMs { get; set; } = 2000;
        
        /// <summary>
        /// Proxy URL for API requests, if required.
        /// Leave empty for no proxy.
        /// </summary>
        public string ProxyUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Proxy port for API requests, if required.
        /// Default is 0 (no proxy).
        /// </summary>
        public int ProxyPort { get; set; } = 0;
        
        /// <summary>
        /// Maximum number of messages to retain in the chat history for multi-turn conversations.
        /// Default is 20 messages.
        /// </summary>
        public int MaxChatHistorySize { get; set; } = 20;
    }
}