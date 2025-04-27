namespace AIHelperLibrary.Models;

/// <summary>
/// OpenAI model identifiers (synced with rate-limit table, April 27 2025).
/// </summary>
public enum OpenAIModel : int
{
    // === General-purpose Chat / Text ===
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

    // === Flagship GPT-4.1 (Apr 2025) ===
    GPT_4_1,
    GPT_4_1_2025_04_14,
    GPT_4_1_Mini,
    GPT_4_1_Mini_2025_04_14,
    GPT_4_1_Nano,
    GPT_4_1_Nano_2025_04_14,

    // === Optimized GPT-4o Family ===
    GPT_4o,
    GPT_4o_2024_05_13,
    GPT_4o_2024_08_06,
    GPT_4o_2024_11_20,
    ChatGPT_4o_Latest,
    GPT_4o_Mini,
    GPT_4o_Mini_2024_07_18,
    GPT_4o_Nano,
    GPT_4o_Nano_2025_04_14,

    // === Specialized GPT-4o Variants ===
    GPT_4o_RealTime_Preview,
    GPT_4o_RealTime_Preview_2024_10_01,
    GPT_4o_RealTime_Preview_2024_12_17,
    GPT_4o_Mini_RealTime_Preview,
    GPT_4o_Mini_RealTime_Preview_2024_12_17,
    GPT_4o_Audio_Preview,
    GPT_4o_Audio_Preview_2024_12_17,
    GPT_4o_Transcribe_2025_03_20,
    GPT_4o_Mini_TTS_2025_03_20,
    GPT_4o_Mini_Transcribe_2025_03_20,
    GPT_4o_Mini_Audio_Preview,
    GPT_4o_Mini_Audio_Preview_2024_12_17,

    // === o-Series Reasoning ===
    O1,
    O1_2024_12_17,
    O1_Preview,
    O1_Preview_2024_09_12,
    O1_Mini,
    O1_Mini_2024_09_12,
    O3_Mini,
    O3_Mini_2025_01_31,
    O4_Mini,
    O4_Mini_2025_04_16,

    // === Legacy & Preview ===
    GPT_4_5_Preview,
    GPT_4_5_Preview_2025_02_27
}

/// <summary>
/// Extension helpers for <see cref="OpenAIModel"/>.
/// </summary>
public static class OpenAIModelExtensions
{
    /// <summary>Maps enum → official model ID string.</summary>
    public static string GetModelIdentifier(this OpenAIModel model) => model switch
    {
        // General chat / text
        OpenAIModel.GPT_3_5_Turbo => "gpt-3.5-turbo",
        OpenAIModel.GPT_3_5_Turbo_16k => "gpt-3.5-turbo-16k",
        OpenAIModel.GPT_3_5_Turbo_0125 => "gpt-3.5-turbo-0125",
        OpenAIModel.GPT_3_5_Turbo_Instruct => "gpt-3.5-turbo-instruct",
        OpenAIModel.GPT_3_5_Turbo_Instruct_0914 => "gpt-3.5-turbo-instruct-0914",
        OpenAIModel.GPT_3_5_Turbo_1106 => "gpt-3.5-turbo-1106",
        OpenAIModel.GPT_4 => "gpt-4",
        OpenAIModel.GPT_4_Turbo => "gpt-4-turbo",
        OpenAIModel.GPT_4_Turbo_2024_04_09 => "gpt-4-turbo-2024-04-09",
        OpenAIModel.GPT_4_Turbo_Preview => "gpt-4-turbo-preview",
        OpenAIModel.GPT_4_0613 => "gpt-4-0613",
        OpenAIModel.GPT_4_0125_Preview => "gpt-4-0125-preview",
        OpenAIModel.GPT_4_1106_Preview => "gpt-4-1106-preview",

        // GPT-4.1
        OpenAIModel.GPT_4_1 => "gpt-4.1",
        OpenAIModel.GPT_4_1_2025_04_14 => "gpt-4.1-2025-04-14",
        OpenAIModel.GPT_4_1_Mini => "gpt-4.1-mini",
        OpenAIModel.GPT_4_1_Mini_2025_04_14 => "gpt-4.1-mini-2025-04-14",
        OpenAIModel.GPT_4_1_Nano => "gpt-4.1-nano",
        OpenAIModel.GPT_4_1_Nano_2025_04_14 => "gpt-4.1-nano-2025-04-14",

        // GPT-4o core
        OpenAIModel.GPT_4o => "gpt-4o",
        OpenAIModel.GPT_4o_2024_05_13 => "gpt-4o-2024-05-13",
        OpenAIModel.GPT_4o_2024_08_06 => "gpt-4o-2024-08-06",
        OpenAIModel.GPT_4o_2024_11_20 => "gpt-4o-2024-11-20",
        OpenAIModel.ChatGPT_4o_Latest => "chatgpt-4o-latest",
        OpenAIModel.GPT_4o_Mini => "gpt-4o-mini",
        OpenAIModel.GPT_4o_Mini_2024_07_18 => "gpt-4o-mini-2024-07-18",
        OpenAIModel.GPT_4o_Nano => "gpt-4o-nano",
        OpenAIModel.GPT_4o_Nano_2025_04_14 => "gpt-4o-nano-2025-04-14",

        // Specialized 4o
        OpenAIModel.GPT_4o_RealTime_Preview => "gpt-4o-realtime-preview",
        OpenAIModel.GPT_4o_RealTime_Preview_2024_10_01 => "gpt-4o-realtime-preview-2024-10-01",
        OpenAIModel.GPT_4o_RealTime_Preview_2024_12_17 => "gpt-4o-realtime-preview-2024-12-17",
        OpenAIModel.GPT_4o_Mini_RealTime_Preview => "gpt-4o-mini-realtime-preview",
        OpenAIModel.GPT_4o_Mini_RealTime_Preview_2024_12_17 => "gpt-4o-mini-realtime-preview-2024-12-17",
        OpenAIModel.GPT_4o_Audio_Preview => "gpt-4o-audio-preview",
        OpenAIModel.GPT_4o_Audio_Preview_2024_12_17 => "gpt-4o-audio-preview-2024-12-17",
        OpenAIModel.GPT_4o_Transcribe_2025_03_20 => "gpt-4o-transcribe-2025-03-20",
        OpenAIModel.GPT_4o_Mini_TTS_2025_03_20 => "gpt-4o-mini-tts-2025-03-20",
        OpenAIModel.GPT_4o_Mini_Transcribe_2025_03_20 => "gpt-4o-mini-transcribe-2025-03-20",
        OpenAIModel.GPT_4o_Mini_Audio_Preview => "gpt-4o-mini-audio-preview",
        OpenAIModel.GPT_4o_Mini_Audio_Preview_2024_12_17 => "gpt-4o-mini-audio-preview-2024-12-17",

        // o-Series
        OpenAIModel.O1 => "o1",
        OpenAIModel.O1_2024_12_17 => "o1",
        OpenAIModel.O1_Preview => "o1-preview",
        OpenAIModel.O1_Preview_2024_09_12 => "o1-preview",
        OpenAIModel.O1_Mini => "o1-mini",
        OpenAIModel.O1_Mini_2024_09_12 => "o1-mini",
        OpenAIModel.O3_Mini => "o3-mini",
        OpenAIModel.O3_Mini_2025_01_31 => "o3-mini",
        OpenAIModel.O4_Mini => "o4-mini",
        OpenAIModel.O4_Mini_2025_04_16 => "o4-mini",


        // Legacy & preview
        OpenAIModel.GPT_4_5_Preview => "gpt-4.5-preview",
        OpenAIModel.GPT_4_5_Preview_2025_02_27 => "gpt-4.5-preview-2025-02-27",

        _ => throw new ArgumentException($"Invalid AI model: {model}")
    };

    /// <summary>
    /// Provider helper.
    /// </summary>
    public static AIProvider GetProvider(this OpenAIModel _) => AIProvider.OpenAI;
}