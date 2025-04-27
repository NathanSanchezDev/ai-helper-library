# AI Helper Library Documentation

## Overview
The **AI Helper Library** is a comprehensive C# SDK for seamless integration with AI providers such as OpenAI and Anthropic Claude.
It provides robust support for generating responses, managing prompts, and creating stateful chatbots with flexible configuration.
With built-in retry logic, proxy support, and multi-provider compatibility, it empowers .NET developers to integrate AI easily.

---

## Features Overview

### Core Functionality
1. **OpenAIClient**:
   - Fully customizable interaction with OpenAI models (e.g., `GPT-4o`, `o1`, `GPT-3.5-Turbo`).
   - Smart handling of `max_tokens` vs `max_completion_tokens`.
   - Built-in retry and logging system.

2. **ClaudeClient**:
   - Supports Anthropic's Claude 3.x models (e.g., `Claude-3.5-Sonnet`, `Claude-3.7-Sonnet`).
   - Handles Claude-specific fields like `stop_sequences`, `system` prompt.
   - Seamless retry, backoff, and system messaging.

3. **Dynamic Prompt Management**:
   - Define, store, and reuse prompts programmatically.

4. **Multi-Turn Conversations**:
   - Persistent memory for each chatbot session, with message limit trimming.

5. **Flexible Configuration**:
   - `OpenAIConfiguration` for OpenAI models.
   - `AnthropicConfiguration` for Claude models.

---

## Installation Guide

### Prerequisites
- .NET SDK (6.0 or higher)
- OpenAI API key or Anthropic Claude API key

### Setup

```bash
git clone https://github.com/NathanSanchezDev/ai-helper-library.git
cd ai-helper-library
dotnet build
dotnet run --project AIHelperConsole
```

Or install from NuGet:

```bash
dotnet add package AIHelperLibrary --version 1.1.0
```

---

## Usage Instructions

### 1. Initialize a Client

#### For OpenAI
```csharp
var config = new OpenAIConfiguration
{
    DefaultModel = OpenAIModel.GPT_4o,
    MaxTokens = 500,
    Temperature = 0.7,
    TopP = 1.0,
    EnableLogging = true
};

var client = new OpenAIClient("your-openai-api-key", config);
```

#### For Anthropic (Claude)
```csharp
var config = new AnthropicConfiguration
{
    DefaultModel = AnthropicModel.Claude3Sonnet,
    MaxTokens = 500,
    SystemPrompt = "You are a helpful assistant.",
    EnableLogging = true
};

var client = new ClaudeClient("your-anthropic-api-key", config);
```

---

### 2. Generate Responses

#### Single Prompt
```csharp
var response = await client.GenerateTextAsync("Explain quantum computing simply.");
Console.WriteLine(response);
```

#### Dynamic Prompting
```csharp
var promptManager = new DynamicPromptManager();
promptManager.AddPrompt("FriendlyGreeting", "Please greet the user warmly.");

var response = await client.GenerateTextWithDynamicPromptAsync(promptManager, "FriendlyGreeting", "Hello there!");
Console.WriteLine(response);
```

#### Chatbot with Memory
```csharp
var reply = await client.GenerateChatResponseAsync(
    "SupportBotSession",
    "When are you open?",
    "You are a friendly support chatbot."
);
Console.WriteLine(reply);
```

---

## Supported AI Models

| Provider | Models |
|:--------|:------|
| OpenAI  | `gpt-3.5-turbo`, `gpt-4`, `gpt-4-turbo`, `gpt-4o`, `o1`, `o3-mini`, `o4-mini`, etc. |
| Anthropic | `claude-3-sonnet`, `claude-3-haiku`, `claude-3-opus`, `claude-3.5-sonnet`, `claude-3.7-sonnet` |

---

## Configuration Options

### OpenAIConfiguration

| Property | Description | Default |
|:---------|:------------|:-------|
| `DefaultModel` | OpenAI model | `gpt-3.5-turbo` |
| `MaxTokens` | Max tokens allowed | 150 |
| `Temperature` | Creativity level | 0.7 |
| `TopP` | Alternative sampling | 1.0 |
| `Instructions` | System instructions | "You are an AI assistant." |
| `Tools` | Enabled tools like "code_interpreter" | none |
| `EnableLogging` | Enable debug logs | false |
| `MaxRetryCount` | API retry attempts | 3 |
| `RetryDelayMs` | Retry delay (ms) | 2000 |
| `ProxyUrl` | Optional proxy | "" |
| `ProxyPort` | Optional proxy port | 0 |

> **Note**: For `o1`, `o3`, and `o4` models, only `max_completion_tokens` is sent (not `temperature`/`top_p`).

### AnthropicConfiguration

| Property | Description | Default |
|:---------|:------------|:--------|
| `DefaultModel` | Claude model | `claude-3-sonnet` |
| `MaxTokens` | Max tokens | 150 |
| `SystemPrompt` | Instructions to assistant | "You are Claude, an AI assistant." |
| `TopP` | Diversity control | 1.0 |
| `StopSequences` | Stop tokens | none |
| `EnableLogging` | Enable logs | false |

---

## Testing with Console Application

- Run the console project to:
  - Test different models.
  - Observe chat history behavior.
  - Verify retry and proxy handling.

---

## Future Plans

- **Azure OpenAI integration**
- **Streaming token support**
- **External prompt storage (e.g., database or file)**
- **Dynamic multi-provider fallback (OpenAI to Claude fallback if needed)**

---

## Contact
For support or inquiries:
- GitHub Issues
- Email: `ns.dev.contact@gmail.com`

---

# End of Documentation

---