using AIHelperLibrary.Services;
using AIHelperLibrary.Prompts;
using AIHelperLibrary.Models;
using AIHelperLibrary.Configurations;

class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to the AI Helper Test Program!");
        Console.Write("Enter your OpenAI API Key: ");
        var apiKey = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("API Key cannot be null or empty. Exiting...");
            return;
        }

        // Set up configuration
        var config = new AIExtensionHelperConfiguration
        {
            Language = Language.English,
            DefaultModel = AIModel.GPT_4o_Mini,
            MaxTokens = 150,
            Temperature = 0.7,
            TopP = 1.0,
            RequestTimeoutMs = 10000,
            Instructions = "You are an AI assistant.",
            Tools = new() { "code_interpreter" },
            MaxRetryCount = 3,
            RetryDelayMs = 2000
        };

        var client = new OpenAIClient(apiKey, config);

        while (true)
        {
            DisplayMenu();

            var choice = Console.ReadLine();

            if (choice == "4")
            {
                Console.WriteLine("Thank you for using AI Helper. Goodbye!");
                break;
            }

            await HandleUserChoice(choice, client);
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("\n--- Main Menu ---");
        Console.WriteLine("Choose an option to test:");
        Console.WriteLine("1. Custom Prompt: Directly input your own prompt for the AI.");
        Console.WriteLine("2. Predefined Prompt: Summarize a piece of text.");
        Console.WriteLine("3. Dynamic Prompt: Create and test your own reusable prompt.");
        Console.WriteLine("4. Exit: Quit the program.");
        Console.Write("Enter your choice (1-4): ");
    }

    private static async Task HandleUserChoice(string choice, OpenAIClient client)
    {
        try
        {
            switch (choice)
            {
                case "1":
                    await TestCustomPrompt(client);
                    break;
                case "2":
                    await TestPredefinedPrompt(client);
                    break;
                case "3":
                    await TestDynamicPrompt(client);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static async Task TestCustomPrompt(OpenAIClient client)
    {
        Console.WriteLine("\n--- Custom Prompt Test ---");
        Console.Write("Enter your custom prompt (e.g., 'Explain the benefits of using AI in education'): ");
        var prompt = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(prompt))
        {
            Console.WriteLine("Prompt cannot be null or empty.");
            return;
        }

        try
        {
            var result = await client.GenerateTextAsync(prompt);
            Console.WriteLine("\n--- AI Response ---");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating text: {ex.Message}");
        }
    }

    private static async Task TestPredefinedPrompt(OpenAIClient client)
    {
        Console.WriteLine("\n--- Predefined Prompt Test: Summarization ---");
        Console.Write("Enter the text you want to summarize: ");
        var userInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userInput))
        {
            Console.WriteLine("Input text cannot be null or empty.");
            return;
        }

        try
        {
            var result = await client.GenerateTextWithPredefinedPromptAsync(PromptManager.GetPrompt(PromptType.Summarize), userInput);
            Console.WriteLine("\n--- AI Response ---");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating predefined prompt: {ex.Message}");
        }
    }

    private static async Task TestDynamicPrompt(OpenAIClient client)
    {
        Console.WriteLine("\n--- Dynamic Prompt Test ---");
        Console.Write("Enter a key (name) for your dynamic prompt (e.g., 'Greeting'): ");
        var key = Console.ReadLine();
        Console.Write("Enter the content of your dynamic prompt (e.g., 'Greet the user warmly and enthusiastically'): ");
        var dynamicPromptContent = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(dynamicPromptContent))
        {
            Console.WriteLine("Key and dynamic prompt content cannot be null or empty.");
            return;
        }

        var promptManager = new DynamicPromptManager();
        promptManager.AddPrompt(key, dynamicPromptContent);

        Console.Write("Enter the input for your dynamic prompt (e.g., 'Hello'): ");
        var userInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userInput))
        {
            Console.WriteLine("User input cannot be null or empty.");
            return;
        }

        try
        {
            var result = await client.GenerateTextWithDynamicPromptAsync(promptManager, key, userInput);
            Console.WriteLine("\n--- AI Response ---");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating dynamic prompt: {ex.Message}");
        }
    }
}