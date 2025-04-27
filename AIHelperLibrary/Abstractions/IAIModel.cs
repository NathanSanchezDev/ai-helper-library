using AIHelperLibrary.Models;

namespace AIHelperLibrary.Abstractions
{
    /// <summary>
    /// Defines the contract for AI model identifiers across different providers.
    /// </summary>
    public interface IAIModel
    {
        /// <summary>
        /// Gets the provider-specific model identifier string used in API calls.
        /// </summary>
        /// <returns>The model identifier string.</returns>
        string GetModelIdentifier();
        
        /// <summary>
        /// Gets the provider associated with this model.
        /// </summary>
        AIProvider Provider { get; }
    }
}