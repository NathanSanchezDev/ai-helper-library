using AIHelperLibrary.Abstractions;

namespace AIHelperLibrary.Models
{
    /// <summary>
    /// OpenAI model identifiers.
    /// </summary>
    public enum AIModel : int
    {
        // General Purpose Text Models
        GPT_3_5_Turbo,
        GPT_3_5_Turbo_16k,
        GPT_3_5_Turbo_0125,
        GPT_3_5_Turbo_Instruct,
        GPT_3_5_Turbo_Instruct_0914,
        GPT_3_5_Turbo_1106,
        GPT_4,
        GPT_4_Turbo,
        GPT_4_Turbo_2024_04_09,
        GPT_4_Turbo_Preview,
        GPT_4_0613,
        GPT_4_0125_Preview,
        GPT_4_1106_Preview,

        // Optimized Models
        GPT_4o,
        GPT_4o_2024_08_06,
        ChatGPT_4o_Latest,
        GPT_4o_Mini,
        GPT_4o_Mini_2024_07_18,

        // New Optimized Model Aliases
        O1,
        O1_2024_12_17,

        O1_Mini,
        O1_Mini_2024_09_12,

        O3_Mini,
        O3_Mini_2025_01_31,

        O1_Preview,
        O1_Preview_2024_09_12,

        GPT_4o_RealTime_Preview,
        GPT_4o_RealTime_Preview_2024_10_01,
        GPT_4o_RealTime_Preview_2024_12_17,

        GPT_4o_Mini_RealTime_Preview,
        GPT_4o_Mini_RealTime_Preview_2024_12_17,

        GPT_4o_Audio_Preview,
        GPT_4o_Audio_Preview_2024_12_17,

        GPT_4o_2024_05_13,
        GPT_4o_2024_11_20
    }
    
    /// <summary>
    /// Extension methods for AIModel enum.
    /// </summary>
    public static class AIModelExtensions
    {
        /// <summary>
        /// Maps the AIModel enum to its corresponding OpenAI model string.
        /// </summary>
        /// <param name="model">The OpenAI model enum value.</param>
        /// <returns>The model identifier string for API requests.</returns>
        public static string GetModelIdentifier(this AIModel model)
        {
            return model switch
            {
                // General Purpose Text Models
                AIModel.GPT_3_5_Turbo => "gpt-3.5-turbo",
                AIModel.GPT_3_5_Turbo_16k => "gpt-3.5-turbo-16k",
                AIModel.GPT_3_5_Turbo_0125 => "gpt-3.5-turbo-0125",
                AIModel.GPT_3_5_Turbo_Instruct => "gpt-3.5-turbo-instruct",
                AIModel.GPT_3_5_Turbo_Instruct_0914 => "gpt-3.5-turbo-instruct-0914",
                AIModel.GPT_3_5_Turbo_1106 => "gpt-3.5-turbo-1106",
                AIModel.GPT_4 => "gpt-4",
                AIModel.GPT_4_Turbo => "gpt-4-turbo",
                AIModel.GPT_4_Turbo_2024_04_09 => "gpt-4-turbo-2024-04-09",
                AIModel.GPT_4_Turbo_Preview => "gpt-4-turbo-preview",
                AIModel.GPT_4_0613 => "gpt-4-0613",
                AIModel.GPT_4_0125_Preview => "gpt-4-0125-preview",
                AIModel.GPT_4_1106_Preview => "gpt-4-1106-preview",

                // Optimized Models
                AIModel.GPT_4o => "gpt-4o",
                AIModel.GPT_4o_2024_08_06 => "gpt-4o-2024-08-06",
                AIModel.ChatGPT_4o_Latest => "chatgpt-4o-latest",
                AIModel.GPT_4o_Mini => "gpt-4o-mini",
                AIModel.GPT_4o_Mini_2024_07_18 => "gpt-4o-mini-2024-07-18",

                AIModel.O1 => "o1",
                AIModel.O1_2024_12_17 => "o1-2024-12-17",
                AIModel.O1_Mini => "o1-mini",
                AIModel.O1_Mini_2024_09_12 => "o1-mini-2024-09-12",
                AIModel.O3_Mini => "o3-mini",
                AIModel.O3_Mini_2025_01_31 => "o3-mini-2025-01-31",
                AIModel.O1_Preview => "o1-preview",
                AIModel.O1_Preview_2024_09_12 => "o1-preview-2024-09-12",

                AIModel.GPT_4o_RealTime_Preview => "gpt-4o-realtime-preview",
                AIModel.GPT_4o_RealTime_Preview_2024_10_01 => "gpt-4o-realtime-preview-2024-10-01",
                AIModel.GPT_4o_RealTime_Preview_2024_12_17 => "gpt-4o-realtime-preview-2024-12-17",

                AIModel.GPT_4o_Mini_RealTime_Preview => "gpt-4o-mini-realtime-preview",
                AIModel.GPT_4o_Mini_RealTime_Preview_2024_12_17 => "gpt-4o-mini-realtime-preview-2024-12-17",

                AIModel.GPT_4o_Audio_Preview => "gpt-4o-audio-preview",
                AIModel.GPT_4o_Audio_Preview_2024_12_17 => "gpt-4o-audio-preview-2024-12-17",

                AIModel.GPT_4o_2024_05_13 => "gpt-4o-2024-05-13",
                AIModel.GPT_4o_2024_11_20 => "gpt-4o-2024-11-20",

                _ => throw new ArgumentException($"Invalid AI model: {model}")
            };
        }
        
        /// <summary>
        /// Returns the provider for OpenAI models.
        /// </summary>
        /// <param name="model">The OpenAI model enum value.</param>
        /// <returns>The AIProvider enum value for OpenAI.</returns>
        public static AIProvider GetProvider(this AIModel model)
        {
            return AIProvider.OpenAI;
        }
    }
}