# AI Helper Library Documentation

## Overview
The **AI Helper Library** is a modular and reusable library built in C# for interacting with OpenAI's API. It offers robust functionality for managing prompts, generating responses, and creating chatbot-like experiences with persistent context. The library is designed to be developer-friendly and highly customizable, making it easy to integrate AI-driven capabilities into .NET applications.

This documentation serves as a comprehensive guide to understanding, configuring, and extending the capabilities of the library.

---

## Features Overview

### Core Functionality
1. **OpenAIClient**
   - Manages communication with OpenAI's API endpoints.
   - Configurable support for models such as `GPT-3.5-Turbo` and `GPT-4`.
   - Single-turn and multi-turn conversations with retained context.
   - Automatically manages chat history with configurable limits.

2. **Prompt Management**
   - **Predefined Prompts**: Use built-in templates for tasks like summarization or Q&A.
   - **Dynamic Prompts**: Programmatically create, store, and reuse custom prompts.

3. **Configuration Options**
   - Allows fine-tuning via `AIExtensionHelperConfiguration`, including token limits, temperature, and logging.

4. **Chatbot Instances**
   - Persistent context for multiple chatbot sessions.
   - Maintain conversation history for enhanced user experience.

5. **Error Handling**
   - Validates inputs and manages API errors gracefully.
   - Logs detailed error messages when enabled.

6. **Sample Console Application**
   - Demonstrates key features with a lightweight CLI for testing and integration.

---

## Installation Guide

### Prerequisites
1. Install the .NET SDK (version 6.0 or later).
2. Obtain an OpenAI API key from the [OpenAI Platform](https://platform.openai.com/).

### Setting Up the Project

#### Clone the Repository
```bash
git clone https://github.com/NathanSanchezDev/ai-helper-library.git
cd ai-helper-library
```

#### Build the Project
```bash
dotnet build
```

#### Run the Console Application
To test the library functionality:
```bash
dotnet run --project AIHelperConsole
```

---

## Usage Instructions

### 1. Initialize the Library

#### Configuration Example
Set up `AIExtensionHelperConfiguration` to define default behaviors:

```csharp
var config = new AIExtensionHelperConfiguration
{
    DefaultModel = AIModel.GPT_3_5_Turbo,
    MaxTokens = 200,
    Temperature = 0.7,
    TopP = 1.0,
    RequestTimeoutMs = 10000,
    EnableLogging = true
    MaxChatHistorySize = 10
};

var client = new OpenAIClient("your-api-key", config);
```

### 2. Generate AI Responses

#### Freeform Prompt
Send a custom prompt to the AI:
```csharp
var response = await client.GenerateTextAsync("Explain quantum computing in simple terms.");
Console.WriteLine(response);
```

### 3. Predefined Prompts
Use built-in prompts for specific tasks:
```csharp
var result = await client.GenerateTextWithPredefinedPromptAsync(PromptManager.Summarize, "Artificial intelligence is revolutionizing industries...");
Console.WriteLine(result);
```

### 4. Dynamic Prompts
Create and reuse custom prompts programmatically:
```csharp
var promptManager = new DynamicPromptManager();
promptManager.AddPrompt("WelcomeMessage", "Greet the user warmly and politely.");

var response = await client.GenerateTextWithDynamicPromptAsync(promptManager, "WelcomeMessage", "Hello!");
Console.WriteLine(response);
```

### 5. Persistent Chatbot Conversations
Maintain context for multi-turn conversations:
```csharp
var chatResponse = await client.GenerateChatResponseAsync(
    "SupportBot",
    "What are your working hours?",
    "You are a polite customer service assistant."
);
Console.WriteLine(chatResponse);
```

---

## Advanced Features

### Configuration Details
The `AIExtensionHelperConfiguration` class supports the following properties:

| Property           | Description                                     | Default Value        |
|--------------------|-------------------------------------------------|----------------------|
| `DefaultModel`     | AI model to use (`GPT-3.5-Turbo`, `GPT-4`).     | `GPT-3.5-Turbo`      |
| `MaxTokens`        | Maximum tokens for each response.              | `150`                |
| `Temperature`      | Controls randomness (0.0 = deterministic).     | `0.7`                |
| `TopP`             | Sampling parameter for diversity.              | `1.0`                |
| `RequestTimeoutMs` | API request timeout in milliseconds.            | `10000` (10 seconds) |
| `EnableLogging`    | Enable or disable logging.                     | `false`              |

### Chatbot Instances
Chatbot sessions maintain conversation histories for context-aware responses. Each instance is identified by a unique key, allowing multiple chatbots to function independently.

---

## Project Structure

```plaintext
ai-helper-library/
├── AIHelper.sln                # Solution file
├── AIHelperLibrary/            # Core library
│   ├── Configurations/         # Configuration models
│   ├── Models/                 # Enums and data models
│   ├── Prompts/                # Predefined and dynamic prompts
│   ├── Services/               # OpenAIClient implementation
│   └── AIHelperLibrary.csproj
├── AIHelperConsole/            # Console application
│   ├── Program.cs              # Entry point for the sample app
│   └── AIHelperConsole.csproj
└── docs/                       # Documentation folder
    └── AIHelperLibraryDocumentation.md
```

---

## Future Enhancements

### Short-Term
1. **Retry Logic**: Add exponential backoff for API calls.
2. **Prompt Persistence**: Save and load dynamic prompts from JSON or database.
3. **Enhanced Error Handling**: Provide more descriptive error messages.

### Long-Term
1. **Multi-Model Support**: Integrate additional AI models like Azure OpenAI.
2. **Stateful Management**: Build advanced session management for chatbots.
3. **Developer Tools**: Provide Visual Studio extensions for scaffolding projects.
4. **Community Engagement**: Publish detailed tutorials and encourage contributions.

---

## Contribution Guidelines

1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add feature: your-feature-name"
   ```
4. Push your branch:
   ```bash
   git push origin feature/your-feature-name
   ```
5. Open a pull request.

---

## Contact Information
For support or inquiries, contact **Nathan Sanchez** via GitHub or email at `ns.dev.contact@gmail.com`.