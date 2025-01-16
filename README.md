# AI Helper Library

The **AI Helper Library** is a modular, reusable C# library designed to simplify interactions with the OpenAI API. This project supports a wide range of AI-driven functionalities, such as generating text responses, handling predefined and dynamic prompts, and managing multi-turn conversations with chatbot-like instances.

The library is complemented by a sample console application to demonstrate its core capabilities and provide a foundation for real-world usage scenarios.

---

## **Features**

### **Core Functionality**

- **OpenAIClient**
  - Handles API requests to OpenAI's `chat/completions` endpoint.
  - Supports dynamic configuration via `AIExtensionHelperConfiguration`.
  - Enables single-turn and multi-turn conversations with context retention.

### **Dynamic and Predefined Prompts**
- **Predefined Prompts**:
  - Preloaded, reusable prompts for tasks like summarization and Q&A.
- **Dynamic Prompts**:
  - Create, store, and reuse prompts programmatically, allowing for custom behaviors tailored to specific use cases.

### **Chatbot Instance Management**
- Maintain distinct conversation histories for multiple chatbot instances.
- Context-aware responses for multi-turn conversations using a `system` role prompt.

### **Configuration Support**
- Configurable options via `AIExtensionHelperConfiguration`, including:
  - Default AI model (`GPT-3.5-Turbo`, `GPT-4`).
  - Token limits (`max_tokens`).
  - Temperature and `top_p` for response creativity.
  - Custom HTTP headers for advanced scenarios.
  - Request timeout settings.

### **Error Handling**
- Validates inputs to prevent invalid API calls.
- Includes detailed error messages for debugging.

### **Sample Console Application**
- A lightweight CLI tool showcasing key library features, including:
  - Custom prompt submission.
  - Predefined summarization prompts.
  - Dynamic prompt creation and usage.
  - Persistent chatbot instances with strict behavior definitions.

---

## **Setup**

### **Prerequisites**

1. **.NET SDK**: Ensure you have .NET SDK 6.0 or later installed.
2. **OpenAI API Key**: Obtain an API key from the [OpenAI platform](https://platform.openai.com/).

### **Installation**

1. Clone the repository:
   ```bash
   git clone https://github.com/NathanSanchezDev/ai-helper-library.git
   cd ai-helper-library
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the console application:
   ```bash
   dotnet run --project AIHelperConsole
   ```

---

## **Usage**

### **Library Integration**

Add the library project as a dependency in your solution or include the compiled DLL in your application.

### **Initialization**

#### Configure the Client:
```csharp
var config = new AIExtensionHelperConfiguration
{
    DefaultModel = AIModel.GPT_3_5_Turbo,
    MaxTokens = 200,
    Temperature = 0.7,
    TopP = 1.0,
    RequestTimeoutMs = 10000,
    EnableLogging = true
};

var client = new OpenAIClient("your-api-key", config);
```

#### Generate a Text Response:
```csharp
var response = await client.GenerateTextAsync("Explain the benefits of renewable energy.");
Console.WriteLine(response);
```

#### Use a Predefined Prompt:
```csharp
var result = await client.GenerateTextWithPredefinedPromptAsync(PromptManager.Summarize, "Artificial intelligence is transforming industries...");
Console.WriteLine(result);
```

#### Create and Use a Dynamic Prompt:
```csharp
var promptManager = new DynamicPromptManager();
promptManager.AddPrompt("Greeting", "Greet the user warmly and politely.");

var response = await client.GenerateTextWithDynamicPromptAsync(promptManager, "Greeting", "Hello there!");
Console.WriteLine(response);
```

#### Multi-Turn Chatbot:
```csharp
var chatResponse = await client.GenerateChatResponseAsync(
    "CustomerSupportBot",
    "How do I reset my password?",
    "You are a helpful customer service assistant."
);
Console.WriteLine(chatResponse);
```

---

## **Project Structure**

```
ai-helper-library/
├── AIHelper.sln                # Solution file
├── AIHelperLibrary/            # Core library
│   ├── Configurations/         # Configuration models
│   ├── Models/                 # Enum and data models
│   ├── Prompts/                # Predefined and dynamic prompts
│   ├── Services/               # OpenAIClient implementation
│   └── AIHelperLibrary.csproj
├── AIHelperConsole/            # Console application
│   ├── Program.cs              # Entry point for the sample app
│   └── AIHelperConsole.csproj
├── .gitignore                  # Git ignore rules
└── README.md                   # Project documentation
```

---

## **Future Plans**

### **Short-Term Goals**
1. **Enhanced Error Handling**:
   - Improve resilience against API rate limits and transient network issues.
   - Add retry logic with exponential backoff.

2. **Advanced Prompt Management**:
   - Enable persistence of dynamic prompts to local storage or a database.
   - Add support for hierarchical prompts (e.g., nested contexts).

3. **Custom Response Parsing**:
   - Allow users to define custom response parsers for advanced scenarios.

### **Long-Term Goals**
1. **SaaS Integration**:
   - Deploy the library as a service with RESTful APIs for easier adoption by non-.NET applications.

2. **Support for More AI Models**:
   - Expand support to include other popular models (e.g., Azure OpenAI, Anthropic Claude).

3. **State Management Enhancements**:
   - Build robust session management for advanced multi-turn conversations.

4. **Developer Experience Improvements**:
   - Provide a Visual Studio extension to scaffold new projects that integrate the library.
   - Add comprehensive logging and debugging tools.

5. **Community Engagement**:
   - Create detailed tutorials and video walkthroughs for using the library.
   - Encourage contributions by adding clear guidelines and first-timer issues.

---

## **License**

This project is licensed under the MIT License.

---

## **Contact**

For questions or feedback, feel free to reach out via GitHub or email: **ns.dev.contact@gmail.com**.
