# AI Helper Library Documentation

## Overview
The **AI Helper Library** is a comprehensive C# library for seamless integration with OpenAI's API. It provides robust support for generating responses, managing custom prompts, and creating stateful chatbot experiences. With built-in retry logic, proxy support, and flexible configuration, it is designed to empower .NET developers to integrate AI features efficiently.

---

## Features Overview

### Core Functionality
1. **OpenAIClient**:
   - Fully customizable interaction with OpenAI's endpoints.
   - Supports models such as `GPT-3.5-Turbo`, `GPT-4`, `GPT-4o`, and more.
   - Configurable retry logic and error handling.
   - Proxy support for secure connections in restricted environments.

2. **Dynamic Prompt Management**:
   - Define, store, and reuse dynamic prompts programmatically.
   - Add contextual system prompts for AI behavior customization.

3. **Configuration Options**:
   - Fine-tune behavior via `AIExtensionHelperConfiguration`, including proxy settings, retries, and instruction overrides.

4. **Chatbot Management**:
   - Maintain conversation histories for multiple independent chatbot sessions.
   - Configurable message limits for context management.

5. **Retry and Logging**:
   - Built-in retry logic with exponential backoff.
   - Optional request/response logging for debugging.

---

## Installation Guide

### Prerequisites
1. Install the .NET SDK (6.0 or higher).
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
Set up `AIExtensionHelperConfiguration` with your preferences:
```csharp
var config = new AIExtensionHelperConfiguration
{
    DefaultModel = AIModel.GPT_4,
    MaxTokens = 250,
    Temperature = 0.8,
    TopP = 0.9,
    RequestTimeoutMs = 15000,
    EnableLogging = true,
    ProxyUrl = "http://proxy.example.com",
    ProxyPort = 8080,
    Instructions = "You are a helpful assistant.",
    Tools = new List<string> { "code_interpreter" },
    MaxRetryCount = 5,
    RetryDelayMs = 3000
};

var client = new OpenAIClient("your-api-key", config);
```

---

### 2. Generate AI Responses

#### Custom Prompt
Send a simple custom prompt to the AI:
```csharp
var response = await client.GenerateTextAsync("Explain the difference between AI and machine learning.");
Console.WriteLine(response);
```

#### Predefined Prompt
Use predefined templates for specific tasks:
```csharp
var predefinedPrompt = PromptManager.GetPrompt(PromptType.Summarize);
var summary = await client.GenerateTextWithPredefinedPromptAsync(predefinedPrompt, "Artificial intelligence is transforming the tech industry...");
Console.WriteLine(summary);
```

---

### 3. Dynamic Prompts
Define and use custom prompts programmatically:
```csharp
var promptManager = new DynamicPromptManager();
promptManager.AddPrompt("FriendlyGreeting", "Please greet the user warmly.");

var response = await client.GenerateTextWithDynamicPromptAsync(promptManager, "FriendlyGreeting", "Hello, AI!");
Console.WriteLine(response);
```

---

### 4. Persistent Chatbot Conversations
Manage multi-turn conversations with context retention:
```csharp
var chatResponse = await client.GenerateChatResponseAsync(
    "CustomerSupportBot",
    "What are your working hours?",
    "You are a helpful and polite customer support assistant."
);
Console.WriteLine(chatResponse);
```

---

## Supported Models and Features

### Supported AI Models
The library supports the following OpenAI models:

| Model Name                           | Description                                                            |
|--------------------------------------|------------------------------------------------------------------------|
| `GPT-3.5-Turbo`                      | Standard model optimized for cost and speed.                           |
| `GPT-3.5-Turbo-16k`                  | Higher context window for larger inputs.                               |
| `GPT-3.5-Turbo-0125`                 | Slightly improved instruction-following capabilities.                  |
| `GPT-3.5-Turbo-Instruct`             | Enhanced for instructional and fine-grained tasks.                     |
| `GPT-3.5-Turbo-1106`                 | Latest refined variant of GPT-3.5.                                       |
| `GPT-4`                              | Advanced model for complex tasks.                                      |
| `GPT-4-Turbo`                        | Faster variant of GPT-4 optimized for efficiency.                      |
| `GPT-4o`                             | Optimized variant of GPT-4 for specific tasks.                         |
| `GPT-4o-Mini`                        | Lightweight and fast version of GPT-4o.                                |
| `GPT-4o-RealTime-Preview`            | Experimental real-time optimized version of GPT-4o.                    |
| `ChatGPT-4o-Latest`                  | Latest optimized model variant used in ChatGPT.                        |
| `o1`                                 | Optimized variant from the o1 series for improved performance.         |
| `o1-mini`                            | Lightweight mini variant from the o1 series for efficient usage.       |
| `o1-preview`                         | Preview variant from the o1 series with early access features.         |
| `o3-mini`                            | Next-generation mini variant from the o3 series offering high efficiency.|
| `GPT-4o-Mini-RealTime-Preview`       | Mini real-time preview variant of GPT-4o.                              |
| `GPT-4o-Audio-Preview`               | Audio-optimized preview variant of GPT-4o.                             |

### Excluded Features
Currently, the library does **not** support the following:
- Image generation models (`DALL-E 2`, `DALL-E 3`).
- Speech-to-text models (`Whisper-1`, `TTS-1`).
- Moderation and specialty models (`Omni-Moderation`).

### Supported Providers
The library is designed specifically for OpenAI’s API. Future updates may include:
- Integration with Azure OpenAI Service.
- Support for custom fine-tuned models.

---

## Advanced Features

### Configuration Properties

| Property            | Description                                          | Default Value        |
|---------------------|------------------------------------------------------|----------------------|
| `DefaultModel`      | AI model to use (`GPT-4`, `GPT-3.5-Turbo`).          | `GPT-3.5-Turbo`      |
| `MaxTokens`         | Maximum tokens per response.                        | `150`                |
| `Temperature`       | Controls creativity (0.0 = deterministic).          | `0.7`                |
| `TopP`              | Alternative to temperature for diversity.           | `1.0`                |
| `RequestTimeoutMs`  | API timeout in milliseconds.                        | `10000` (10 seconds) |
| `EnableLogging`     | Enables debug logging of requests/responses.        | `false`              |
| `ProxyUrl`          | Proxy server URL.                                   | `""`                 |
| `ProxyPort`         | Proxy server port.                                  | `0`                  |
| `Instructions`      | Behavior instructions for the assistant.            | `"You are an AI assistant."` |
| `Tools`             | Tools enabled for the assistant (e.g., "code_interpreter"). | `[]`          |
| `MaxRetryCount`     | Maximum retries for failed requests.                | `3`                  |
| `RetryDelayMs`      | Delay between retries in milliseconds.              | `2000`               |

---

## Testing with Console Application

### Features Demonstrated:
- Generate responses with custom, predefined, and dynamic prompts.
- Manage chatbot sessions with persistent context.
- Test retry logic and proxy settings.

---

## Future Enhancements

### Short-Term
1. **Prompt Persistence**: Save and load prompts from external storage.
2. **Advanced Error Reporting**: Return detailed error metadata.

### Long-Term
1. **Enhanced Model Support**: Extend support for OpenAI's latest capabilities.
2. **User Authentication**: Secure chatbot sessions with user authentication.

---

## Contact Information
For support or inquiries, contact **Nathan Sanchez** via GitHub or email at `ns.dev.contact@gmail.com`.