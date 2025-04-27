using AIHelperLibrary.Abstractions;
using AIHelperLibrary.Configurations;
using AIHelperLibrary.Models;
using AIHelperLibrary.Prompts;
using AIHelperLibrary.Services;
using System.Text;

namespace AIHelperConsole;

class Program
{
    private static IAIClient? _client;
    private static AIProvider _selectedProvider;
    private static string _sessionId = Guid.NewGuid().ToString();
    private static readonly List<HistoryItem> _history = new();

    public static async Task Main(string[] args)
    {
        Console.Title = "AI Helper Library Test Console";
        DisplayWelcome();

        // Setup AI client
        if (!SetupAIClient())
        {
            Console.WriteLine("Failed to initialize AI client. Press any key to exit...");
            Console.ReadKey();
            return;
        }

        // Main application loop
        while (true)
        {
            try
            {
                DisplayMainMenu();
                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        await RunCustomPromptTest();
                        break;
                    case "2":
                        await RunPredefinedPromptTest();
                        break;
                    case "3":
                        await RunDynamicPromptTest();
                        break;
                    case "4":
                        await RunChatTest();
                        break;
                    case "5":
                        ModifyConfiguration();
                        break;
                    case "6":
                        ViewHistory();
                        break;
                    case "7":
                        DisplayHelp();
                        break;
                    case "8":
                        if (ConfirmExit())
                            return;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                DisplayError($"An unexpected error occurred: {ex.Message}");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
            }
        }
    }

    #region Setup

    private static bool SetupAIClient()
    {
        // Provider selection
        _selectedProvider = SelectProvider();

        // Create appropriate client
        switch (_selectedProvider)
        {
            case AIProvider.OpenAI:
                _client = CreateOpenAIClient();
                break;
            case AIProvider.Anthropic:
                _client = CreateClaudeClient();
                break;
            default:
                return false;
        }

        return _client != null;
    }

    private static AIProvider SelectProvider()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║          SELECT AI PROVIDER              ║");
            Console.WriteLine("╠══════════════════════════════════════════╣");
            Console.WriteLine("║  1. OpenAI (GPT models)                  ║");
            Console.WriteLine("║  2. Anthropic (Claude models)            ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");
            Console.Write("Enter your choice (1-2): ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    return AIProvider.OpenAI;
                case "2":
                    return AIProvider.Anthropic;
                default:
                    Console.WriteLine("Invalid choice. Press any key to try again...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    private static IAIClient? CreateOpenAIClient()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║           OPENAI CONFIGURATION           ║");
        Console.WriteLine("╚══════════════════════════════════════════╝\n");

        Console.Write("Enter your OpenAI API Key: ");
        var apiKey = ReadPasswordMasked();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            DisplayError("API Key cannot be null or empty.");
            return null;
        }

        // ── Model selection ──────────────────────────────────────────────────────
        var options = new (string Label, OpenAIModel Model)[]
        {
        // 🔹 GPT-3.5 & GPT-4 series
        ("GPT‑3.5‑Turbo (baseline / fastest)",    OpenAIModel.GPT_3_5_Turbo),
        ("GPT‑4 (classic strong model)",          OpenAIModel.GPT_4),
        ("GPT‑4 Turbo (faster + cheaper)",        OpenAIModel.GPT_4_Turbo),
        ("GPT‑4.1 (flagship Apr 2025)",           OpenAIModel.GPT_4_1),

        // 🔹 GPT-4o optimized family
        ("GPT‑4o‑Nano (ultra‑lightweight)",       OpenAIModel.GPT_4o_Nano),
        ("GPT‑4o‑Mini (fast & affordable)",       OpenAIModel.GPT_4o_Mini),
        ("GPT‑4o (balanced multimodal)",          OpenAIModel.GPT_4o),

        // 🔹 O-Series lightweight models
        ("O1 (lightweight new model)",            OpenAIModel.O1),
        ("O1 Preview",                            OpenAIModel.O1_Preview),
        ("O1 Mini",                               OpenAIModel.O1_Mini),
        ("O3 Mini (better reasoning)",            OpenAIModel.O3_Mini),
        ("O4 Mini (early next gen preview)",      OpenAIModel.O4_Mini),
        };

        Console.WriteLine("\nSelect OpenAI Model:");
        for (var i = 0; i < options.Length; i++)
            Console.WriteLine($"{i + 1}. {options[i].Label}");

        Console.Write($"Enter your choice (1-{options.Length}): ");
        if (!int.TryParse(Console.ReadLine()?.Trim(), out var choice) || choice < 1 || choice > options.Length)
        {
            DisplayError("Invalid selection — defaulting to GPT‑3.5‑Turbo.");
            choice = 1;
        }

        var model = options[choice - 1].Model;

        // ── Build configuration ─────────────────────────────────────────────────
        var config = new OpenAIConfiguration
        {
            Language = Language.English,
            DefaultModel = model,
            MaxTokens = 500,
            Temperature = 0.7,
            TopP = 1.0,
            RequestTimeoutMs = 15_000,
            Instructions = "You are an AI assistant.",
            Tools = new() { "code_interpreter" },
            MaxRetryCount = 3,
            RetryDelayMs = 2_000,
            EnableLogging = true
        };

        try
        {
            var factory = new AIProviderFactory();
            return factory.CreateClient(apiKey, config);
        }
        catch (Exception ex)
        {
            DisplayError($"Failed to create OpenAI client: {ex.Message}");
            return null;
        }
    }


    private static IAIClient? CreateClaudeClient()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║          ANTHROPIC CONFIGURATION         ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        Console.Write("Enter your Anthropic API Key: ");
        var apiKey = ReadPasswordMasked();

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            DisplayError("API Key cannot be null or empty.");
            return null;
        }

        Console.WriteLine("\nSelect Claude Model:");
        Console.WriteLine("1. Claude 3 Opus (Most Capable)");
        Console.WriteLine("2. Claude 3 Sonnet (Balanced)");
        Console.WriteLine("3. Claude 3 Haiku (Fastest)");
        Console.WriteLine("4. Claude 3.5 Sonnet (Latest)");
        Console.Write("Enter your choice (1-4): ");

        AnthropicModel model = AnthropicModel.Claude3Sonnet;
        var modelChoice = Console.ReadLine()?.Trim();

        switch (modelChoice)
        {
            case "1":
                model = AnthropicModel.Claude3Opus;
                break;
            case "2":
                model = AnthropicModel.Claude3Sonnet;
                break;
            case "3":
                model = AnthropicModel.Claude3Haiku;
                break;
            case "4":
                model = AnthropicModel.Claude3_5_Sonnet;
                break;
        }

        // Set up Claude configuration
        var config = new AnthropicConfiguration
        {
            DefaultModel = model,
            MaxTokens = 500,
            Temperature = 0.7,
            TopP = 1.0,
            RequestTimeoutMs = 15000,
            SystemPrompt = "You are Claude, an AI assistant created by Anthropic. Provide helpful, accurate, and thoughtful responses.",
            MaxRetryCount = 3,
            RetryDelayMs = 2000,
            EnableLogging = true
        };

        try
        {
            var factory = new AIProviderFactory();
            return factory.CreateClient(apiKey, config);
        }
        catch (Exception ex)
        {
            DisplayError($"Failed to create Claude client: {ex.Message}");
            return null;
        }
    }

    #endregion

    #region Test Functions

    private static async Task RunCustomPromptTest()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║            CUSTOM PROMPT TEST            ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        Console.Write("Enter your custom prompt: ");
        var prompt = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(prompt))
        {
            DisplayError("Prompt cannot be null or empty.");
            return;
        }

        Console.WriteLine("\nGenerating response...");
        try
        {
            Console.WriteLine(new string('-', 50));
            var startTime = DateTime.Now;
            var result = await _client!.GenerateTextAsync(prompt);
            var elapsedTime = DateTime.Now - startTime;

            Console.WriteLine(result);
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Response generated in {elapsedTime.TotalSeconds:F2} seconds.");

            // Add to history
            _history.Add(new HistoryItem
            {
                Timestamp = DateTime.Now,
                PromptType = "Custom",
                Prompt = prompt,
                Response = result,
                ElapsedTime = elapsedTime
            });

            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            DisplayError($"Error generating response: {ex.Message}");
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }
    }

    private static async Task RunPredefinedPromptTest()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║          PREDEFINED PROMPT TEST          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        Console.WriteLine("Available Predefined Prompts:");
        Console.WriteLine("1. Summarize Text");
        Console.WriteLine("2. Explain Concept");
        Console.WriteLine("3. Blog Post");
        Console.WriteLine("4. Code Review");
        Console.Write("Select prompt type (1-4): ");

        var promptTypeChoice = Console.ReadLine()?.Trim();
        PromptType promptType;

        switch (promptTypeChoice)
        {
            case "1":
                promptType = PromptType.Summarize;
                break;
            case "2":
                promptType = PromptType.Explain;
                break;
            case "3":
                promptType = PromptType.BlogPost;
                break;
            case "4":
                promptType = PromptType.CodeReview;
                break;
            default:
                DisplayError("Invalid choice.");
                return;
        }

        Console.WriteLine($"\nSelected: {promptType}");
        Console.Write("Enter your input text: ");
        var userInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userInput))
        {
            DisplayError("Input text cannot be null or empty.");
            return;
        }

        Console.WriteLine("\nGenerating response...");
        try
        {
            var predefinedPrompt = PromptManager.GetPrompt(promptType);
            Console.WriteLine(new string('-', 50));
            var startTime = DateTime.Now;
            var result = await _client!.GenerateTextWithPredefinedPromptAsync(predefinedPrompt, userInput);
            var elapsedTime = DateTime.Now - startTime;

            Console.WriteLine(result);
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Response generated in {elapsedTime.TotalSeconds:F2} seconds.");

            // Add to history
            _history.Add(new HistoryItem
            {
                Timestamp = DateTime.Now,
                PromptType = $"Predefined: {promptType}",
                Prompt = $"{predefinedPrompt}\n{userInput}",
                Response = result,
                ElapsedTime = elapsedTime
            });

            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            DisplayError($"Error generating response: {ex.Message}");
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }
    }

    private static async Task RunDynamicPromptTest()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║           DYNAMIC PROMPT TEST            ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        Console.Write("Enter a key (name) for your dynamic prompt: ");
        var key = Console.ReadLine()?.Trim();

        Console.Write("Enter the content of your dynamic prompt: ");
        var dynamicPromptContent = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(dynamicPromptContent))
        {
            DisplayError("Key and dynamic prompt content cannot be null or empty.");
            return;
        }

        var promptManager = new DynamicPromptManager();
        promptManager.AddPrompt(key, dynamicPromptContent);

        Console.Write("Enter the input for your dynamic prompt: ");
        var userInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userInput))
        {
            DisplayError("User input cannot be null or empty.");
            return;
        }

        Console.WriteLine("\nGenerating response...");
        try
        {
            Console.WriteLine(new string('-', 50));
            var startTime = DateTime.Now;
            var result = await _client!.GenerateTextWithDynamicPromptAsync(promptManager, key, userInput);
            var elapsedTime = DateTime.Now - startTime;

            Console.WriteLine(result);
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Response generated in {elapsedTime.TotalSeconds:F2} seconds.");

            // Add to history
            _history.Add(new HistoryItem
            {
                Timestamp = DateTime.Now,
                PromptType = "Dynamic",
                Prompt = $"Template: {dynamicPromptContent}\nInput: {userInput}",
                Response = result,
                ElapsedTime = elapsedTime
            });

            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            DisplayError($"Error generating response: {ex.Message}");
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }
    }

    private static async Task RunChatTest()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║                CHAT TEST                 ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.WriteLine("Type 'exit' to return to the main menu.");
        Console.WriteLine(new string('-', 50));

        // Set up initial prompt based on provider
        string initialPrompt = _selectedProvider switch
        {
            AIProvider.OpenAI => "You are a helpful AI assistant. Provide concise, accurate responses.",
            AIProvider.Anthropic => "You are Claude, an AI assistant created by Anthropic. Be helpful, harmless, and honest.",
            _ => "You are an AI assistant."
        };

        Console.WriteLine($"System: {initialPrompt}");
        Console.WriteLine(new string('-', 50));

        while (true)
        {
            Console.Write("You: ");
            var userMessage = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userMessage))
                continue;

            if (userMessage.ToLower() == "exit")
                break;

            Console.WriteLine("AI: ");
            try
            {
                var startTime = DateTime.Now;
                var response = await _client!.GenerateChatResponseAsync(_sessionId, userMessage, initialPrompt);
                var elapsedTime = DateTime.Now - startTime;

                Console.WriteLine(response);
                Console.WriteLine($"[Generated in {elapsedTime.TotalSeconds:F2}s]");
                Console.WriteLine(new string('-', 50));

                // Add to history
                _history.Add(new HistoryItem
                {
                    Timestamp = DateTime.Now,
                    PromptType = "Chat",
                    Prompt = userMessage,
                    Response = response,
                    ElapsedTime = elapsedTime
                });
            }
            catch (Exception ex)
            {
                DisplayError($"Error: {ex.Message}");
                Console.WriteLine(new string('-', 50));
            }
        }
    }

    private static void ModifyConfiguration()
    {
        // This would allow modifying configuration settings at runtime
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║        CONFIGURATION MANAGEMENT          ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        Console.WriteLine("This feature is not yet implemented.");
        Console.WriteLine("Press any key to return to the main menu...");
        Console.ReadKey();
    }

    private static void ViewHistory()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║             SESSION HISTORY              ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        if (_history.Count == 0)
        {
            Console.WriteLine("No interactions recorded in this session.");
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Total interactions: {_history.Count}\n");

        for (int i = 0; i < _history.Count; i++)
        {
            var item = _history[i];
            Console.WriteLine($"[{i + 1}] {item.Timestamp:HH:mm:ss} - {item.PromptType} - {item.ElapsedTime.TotalSeconds:F2}s");
        }

        Console.Write("\nEnter item number to view details (or press Enter to return): ");
        var input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int index) && index > 0 && index <= _history.Count)
        {
            var item = _history[index - 1];
            Console.Clear();
            Console.WriteLine($"Interaction Details [{index}]:");
            Console.WriteLine($"Time: {item.Timestamp:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Type: {item.PromptType}");
            Console.WriteLine($"Duration: {item.ElapsedTime.TotalSeconds:F2} seconds");
            Console.WriteLine("\nPrompt:");
            Console.WriteLine(new string('-', 50));
            Console.WriteLine(item.Prompt);
            Console.WriteLine(new string('-', 50));
            Console.WriteLine("\nResponse:");
            Console.WriteLine(new string('-', 50));
            Console.WriteLine(item.Response);
            Console.WriteLine(new string('-', 50));
        }

        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    #endregion

    #region Helper Functions

    private static void DisplayWelcome()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║           AI HELPER LIBRARY              ║");
        Console.WriteLine("║             TEST CONSOLE                 ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.WriteLine("Version 1.0.1");
        Console.WriteLine("(c) 2025 Nathan Sanchez");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void DisplayMainMenu()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║                MAIN MENU                 ║");
        Console.WriteLine("╠══════════════════════════════════════════╣");
        Console.WriteLine($"║  Provider: {_selectedProvider,-28} ║");
        Console.WriteLine("╠══════════════════════════════════════════╣");
        Console.WriteLine("║  1. Custom Prompt Test                   ║");
        Console.WriteLine("║  2. Predefined Prompt Test               ║");
        Console.WriteLine("║  3. Dynamic Prompt Test                  ║");
        Console.WriteLine("║  4. Interactive Chat Test                ║");
        Console.WriteLine("║  5. Configuration Settings               ║");
        Console.WriteLine("║  6. View Session History                 ║");
        Console.WriteLine("║  7. Help & Documentation                 ║");
        Console.WriteLine("║  8. Exit                                 ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");
        Console.Write("Enter your choice (1-8): ");
    }

    private static void DisplayHelp()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════════╗");
        Console.WriteLine("║         HELP & DOCUMENTATION             ║");
        Console.WriteLine("╚══════════════════════════════════════════╝");

        Console.WriteLine("AI Helper Library Test Console provides a comprehensive");
        Console.WriteLine("testing environment for the multi-provider AI library.");
        Console.WriteLine();
        Console.WriteLine("Available Test Options:");
        Console.WriteLine();
        Console.WriteLine("1. Custom Prompt Test");
        Console.WriteLine("   Send a direct prompt to the AI model and view the response.");
        Console.WriteLine();
        Console.WriteLine("2. Predefined Prompt Test");
        Console.WriteLine("   Use predefined prompt templates for common tasks like");
        Console.WriteLine("   summarization, explanation, etc.");
        Console.WriteLine();
        Console.WriteLine("3. Dynamic Prompt Test");
        Console.WriteLine("   Create and test your own reusable prompt templates.");
        Console.WriteLine();
        Console.WriteLine("4. Interactive Chat Test");
        Console.WriteLine("   Test multi-turn conversations with context retention.");
        Console.WriteLine();
        Console.WriteLine("5. Configuration Settings");
        Console.WriteLine("   Modify model settings, tokens, temperature, etc.");
        Console.WriteLine();
        Console.WriteLine("6. View Session History");
        Console.WriteLine("   Review past interactions from the current session.");
        Console.WriteLine();
        Console.WriteLine("For more information, visit the GitHub repository at:");
        Console.WriteLine("https://github.com/NathanSanchezDev/ai-helper-library");

        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    private static void DisplayError(string message)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: {message}");
        Console.ForegroundColor = prevColor;
    }

    private static bool ConfirmExit()
    {
        Console.Write("Are you sure you want to exit? (y/n): ");
        var response = Console.ReadLine()?.Trim().ToLower();
        return response == "y" || response == "yes";
    }

    private static string ReadPasswordMasked()
    {
        var password = new StringBuilder();
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password.ToString();
    }

    #endregion
}

class HistoryItem
{
    public DateTime Timestamp { get; set; }
    public string PromptType { get; set; } = "";
    public string Prompt { get; set; } = "";
    public string Response { get; set; } = "";
    public TimeSpan ElapsedTime { get; set; }
}