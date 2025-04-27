using AIHelperLibrary.Configurations;

namespace AIHelperLibrary.Abstractions
{
    /// <summary>
    /// Factory interface for creating AI client instances for different providers.
    /// </summary>
    public interface IAIProviderFactory
    {
        /// <summary>
        /// Creates an AI client instance for the specified provider using the given configuration.
        /// </summary>
        /// <param name="apiKey">The API key for the service.</param>
        /// <param name="config">The configuration object for the client.</param>
        /// <returns>An implementation of IAIClient for the specified provider.</returns>
        IAIClient CreateClient(string apiKey, AIBaseConfiguration config);
    }
}