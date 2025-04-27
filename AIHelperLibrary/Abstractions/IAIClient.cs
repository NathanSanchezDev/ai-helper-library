using AIHelperLibrary.Prompts;

namespace AIHelperLibrary.Abstractions
{
    /// <summary>
    /// Defines the contract for AI clients.
    /// </summary>
    public interface IAIClient
    {
        /// <summary>
        /// Generates a response for the provided text prompt.
        /// </summary>
        /// <param name="prompt">The input text prompt.</param>
        /// <returns>The AI-generated response as a string.</returns>
        Task<string> GenerateTextAsync(string prompt);

        /// <summary>
        /// Generates a response using a predefined prompt and user input.
        /// </summary>
        /// <param name="predefinedPrompt">The predefined prompt template.</param>
        /// <param name="userInput">The user input to append to the predefined prompt.</param>
        /// <returns>The AI-generated response as a string.</returns>
        Task<string> GenerateTextWithPredefinedPromptAsync(string predefinedPrompt, string userInput);

        /// <summary>
        /// Handles multi-turn conversations, maintaining chat context.
        /// </summary>
        /// <param name="instanceKey">A unique identifier for the chat instance.</param>
        /// <param name="userMessage">The user's message.</param>
        /// <param name="initialPrompt">The system's initial prompt for context.</param>
        /// <returns>The AI-generated response as a string.</returns>
        Task<string> GenerateChatResponseAsync(string instanceKey, string userMessage, string initialPrompt);
        
        /// <summary>
        /// Generates a response using a dynamic prompt managed by a prompt manager.
        /// </summary>
        /// <param name="promptManager">The dynamic prompt manager.</param>
        /// <param name="key">The key identifying the specific prompt template.</param>
        /// <param name="userInput">The user input to incorporate with the prompt.</param>
        /// <returns>The AI-generated response as a string.</returns>
        Task<string> GenerateTextWithDynamicPromptAsync(DynamicPromptManager promptManager, string key, string userInput);
    }
}