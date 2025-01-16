using AIHelperLibrary.Services;
using AIHelperLibrary.Prompts;
using AIHelperLibrary.Models;
using AIHelperLibrary.Configurations;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to the AI Helper Test Program!");
        Console.Write("Enter your OpenAI API Key: ");
        var apiKey = Console.ReadLine();

        // Set up configuration
        var config = new AIExtensionHelperConfiguration
        {
            Language = Language.English,
            DefaultModel = AIModel.GPT_3_5_Turbo,
            MaxTokens = 150,
            Temperature = 0.7,
            TopP = 1.0,
            RequestTimeoutMs = 10000,
            EnableLogging = true
        };

        var client = new OpenAIClient(apiKey, config);

        while (true)
        {
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("Choose an option to test:");
            Console.WriteLine("1. Custom Prompt: Directly input your own prompt for the AI.");
            Console.WriteLine("2. Predefined Prompt: Summarize a piece of text.");
            Console.WriteLine("3. Dynamic Prompt: Create and test your own reusable prompt.");
            Console.WriteLine("4. Exit: Quit the program.");
            Console.Write("Enter your choice (1-4): ");
            var choice = Console.ReadLine();

            if (choice == "4")
            {
                Console.WriteLine("Thank you for using AI Helper. Goodbye!");
                break;
            }

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
    }

    private static async Task TestCustomPrompt(OpenAIClient client)
    {
        Console.WriteLine("\n--- Custom Prompt Test ---");
        Console.WriteLine("This option allows you to send a freeform prompt to the AI.");
        Console.Write("Enter your custom prompt (e.g., 'Explain the benefits of using AI in education'): ");
        var prompt = Console.ReadLine();

        try
        {
            var result = await client.GenerateTextAsync(prompt);
            Console.WriteLine("\n--- AI Response ---");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task TestPredefinedPrompt(OpenAIClient client)
    {
        Console.WriteLine("\n--- Predefined Prompt Test: Summarization ---");
        Console.WriteLine("This option summarizes a piece of text for you.");
        Console.WriteLine("For example, you might input a paragraph, and the AI will return a concise summary.");
        Console.Write("Enter the text you want to summarize: ");
        var userInput = Console.ReadLine();

        try
        {
            var result = await client.GenerateTextWithPredefinedPromptAsync(PromptManager.Summarize, userInput);
            Console.WriteLine("\n--- AI Response ---");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static async Task TestDynamicPrompt(OpenAIClient client)
    {
        Console.WriteLine("\n--- Dynamic Prompt Test ---");
        Console.WriteLine("This option lets you create your own prompt template that can be reused.");
        Console.Write("Enter a key (name) for your dynamic prompt (e.g., 'Greeting'): ");
        var key = Console.ReadLine();
        Console.Write("Enter the content of your dynamic prompt (e.g., 'Greet the user warmly and enthusiastically'): ");
        var dynamicPromptContent = Console.ReadLine();

        var promptManager = new DynamicPromptManager();
        promptManager.AddPrompt(key, dynamicPromptContent);

        Console.Write("Enter the input for your dynamic prompt (e.g., 'Hello'): ");
        var userInput = Console.ReadLine();

        try
        {
            var result = await client.GenerateTextWithDynamicPromptAsync(promptManager, key, userInput);
            Console.WriteLine("\n--- AI Response ---");
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}