using AIHelperLibrary.Models;

namespace AIHelperLibrary.Configurations;
/// <summary>
/// Configuration options for the OpenAI API client.
/// </summary>
public class OpenAIConfiguration : AIBaseConfiguration
{
    /// <summary>
    /// Gets the AI provider for this configuration.
    /// </summary>
    public override AIProvider Provider => AIProvider.OpenAI;

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
    public OpenAIModel DefaultModel { get; set; } = OpenAIModel.GPT_3_5_Turbo;

    /// <summary>
    /// Top-p sampling controls the diversity of responses.
    /// Range: 0.0 to 1.0, where 1.0 includes all possible options.
    /// Default is 1.0.
    /// </summary>
    public double TopP { get; set; } = 1.0;

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
}