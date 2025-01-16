using AIHelperLibrary.Models;

namespace AIHelperLibrary.Configurations
{
    public class AIExtensionHelperConfiguration
    {
        // Language preference for output
        public Language Language { get; set; } = Language.English;

        // Default AI model to use
        public AIModel DefaultModel { get; set; } = AIModel.GPT_3_5_Turbo;

        // Maximum tokens allowed per response
        public int MaxTokens { get; set; } = 150;

        // Creativity level of the AI (0.0 = deterministic, 1.0 = very creative)
        public double Temperature { get; set; } = 0.7;

        // Top-p sampling (alternative to temperature, 1.0 = full probability mass)
        public double TopP { get; set; } = 1.0;

        // Timeout for API requests in milliseconds
        public int RequestTimeoutMs { get; set; } = 10000;

        // Optional user-defined headers (e.g., for custom API setups)
        public Dictionary<string, string> CustomHeaders { get; set; } = new();

        // Custom behavior flags or settings can go here
        public bool EnableLogging { get; set; } = false;
    }
}