using AIHelperLibrary.Models;

namespace AIHelperLibrary.Configurations
{
    /// <summary>
    /// Configuration options for the AI Extension Helper.
    /// This class provides a flexible way to customize the behavior of the AI client.
    /// </summary>
    public class AIExtensionHelperConfiguration
    {
        /// <summary>
        /// Specifies the language preference for AI output.
        /// Default is English.
        /// </summary>
        public Language Language { get; set; } = Language.English;

        /// <summary>
        /// Specifies the default AI model to use for requests.
        /// Supported values are defined in the AIModel enumeration.
        /// Default is GPT-3.5-Turbo.
        /// </summary>
        public AIModel DefaultModel { get; set; } = AIModel.GPT_3_5_Turbo;

        /// <summary>
        /// Maximum number of tokens allowed per response.
        /// Helps control the length of AI-generated output.
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
        /// Top-p sampling controls the diversity of responses.
        /// Range: 0.0 to 1.0, where 1.0 includes all possible options.
        /// Default is 1.0.
        /// </summary>
        public double TopP { get; set; } = 1.0;

        /// <summary>
        /// Timeout for API requests in milliseconds.
        /// Default is 10,000 milliseconds (10 seconds).
        /// </summary>
        public int RequestTimeoutMs { get; set; } = 10000;

        /// <summary>
        /// Optional custom headers for API requests.
        /// Useful for adding metadata or supporting custom setups.
        /// </summary>
        public Dictionary<string, string> CustomHeaders { get; set; } = new();

        /// <summary>
        /// Enables or disables logging of AI client activities.
        /// Default is false (logging is disabled).
        /// </summary>
        public bool EnableLogging { get; set; } = false;

        /// <summary>
        /// Maximum number of messages to retain in the chat history for multi-turn conversations.
        /// Default is 20 messages.
        /// </summary>
        public int MaxChatHistorySize { get; set; } = 20;

        /// <summary>
        /// General instructions for the AI assistant's behavior.
        /// Example: "You are a helpful assistant specializing in coding."
        /// Default is "You are an AI assistant."
        /// </summary>
        public string Instructions { get; set; } = "You are an AI assistant.";

        /// <summary>
        /// A list of tools available for the AI assistant.
        /// Example: ["code_interpreter", "image_generator"].
        /// Default includes "code_interpreter".
        /// </summary>
        public List<string> Tools { get; set; } = new List<string> { "code_interpreter" };

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
        /// Example: "http://proxy.example.com".
        /// </summary>
        public string ProxyUrl { get; set; } = string.Empty;

        /// <summary>
        /// Proxy port for API requests, if required.
        /// Default is 0 (no proxy).
        /// </summary>
        public int ProxyPort { get; set; } = 0;
    }
}