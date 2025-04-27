namespace AIHelperLibrary.Helpers
{
    public static class OpenAIModelHelper
    {
        /// <summary>
        /// Determines if the given model identifier is a chat-capable model.
        /// </summary>
        public static bool IsChatModel(string modelId)
        {
            if (string.IsNullOrWhiteSpace(modelId))
                return false;

            modelId = modelId.ToLowerInvariant();

            return
                modelId.StartsWith("gpt-") ||      // GPT-3.5, GPT-4, GPT-4o, etc.
                modelId.StartsWith("chatgpt-") ||   // ChatGPT versions
                modelId.StartsWith("o1") ||         // O1, O1-Mini
                modelId.StartsWith("o3") ||         // O3-Mini
                modelId.StartsWith("o4");           // O4-Mini
        }

        /// <summary>
        /// Validates that the selected model is a chat model. Throws if not.
        /// </summary>
        public static void EnsureChatModel(string modelId)
        {
            if (!IsChatModel(modelId))
            {
                throw new InvalidOperationException(
                    $"Model '{modelId}' is not a chat-capable model. Please use a chat-supported model (e.g., gpt-4o, gpt-4.1, o1, o3-mini, etc.)."
                );
            }
        }
    }
}