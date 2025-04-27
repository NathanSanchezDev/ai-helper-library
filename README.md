# 🚀 AI Helper Library

The **AI Helper Library** is a modern, modular, and provider-agnostic C# SDK for interacting with today's leading AI services — including OpenAI (GPT models) and Anthropic (Claude models).  
It provides robust tools for dynamic prompts, chat histories, retry handling, multi-provider configuration, and powerful customization — designed for **production-grade** .NET apps.

---

## ✨ Features

- **Unified API** for OpenAI (GPT, O-series) and Anthropic Claude (3.x, 3.5, 3.7) models.
- **Predefined and dynamic prompts** for flexible workflows.
- **Multi-turn conversations** with persistent chat history.
- **Smart configuration system** for tokens, retries, proxies, system prompts, and more.
- **Automatic provider-specific handling** (e.g., OpenAI o-model differences).
- **Built-in retry logic** for rate limiting and transient errors.
- **Ready-to-run sample console application**.
- **Future-proof architecture** for easy extension to Cohere, Gemini, Mistral, etc.

---

## 📦 Installation

### Clone the Repository
```bash
git clone https://github.com/NathanSanchezDev/ai-helper-library.git
cd ai-helper-library
```

### Build the Project
```bash
dotnet build
```

### Run the Sample Console Application
```bash
dotnet run --project AIHelperConsole
```

---

## 🔥 NuGet Package

The **AI Helper Library** is published on [NuGet.org](https://www.nuget.org/packages/AIHelperLibrary/).

### Install via .NET CLI
```bash
dotnet add package AIHelperLibrary --version 1.1.0
```

### Install via Package Manager
```powershell
Install-Package AIHelperLibrary -Version 1.1.0
```

### Using PackageReference
```xml
<PackageReference Include="AIHelperLibrary" Version="1.1.0" />
```

---

## 📚 Documentation

**[→ Full Documentation Here](docs/AIHelperLibraryDocumentation.md)**

Includes:
- Setup & Configuration
- Multi-provider usage (OpenAI vs Anthropic)
- Advanced Prompt Management
- Handling OpenAI o-series (`o1`, `o3-mini`, `o4-mini`) differences
- Retry Strategies
- Custom Proxy & Header Support
- Future Plans & Roadmap

---

## 🛡 License

Licensed under the **MIT License**.

---

## 👨‍💼 Contact

Created by **Nathan Sanchez**  
📧 Email: `ns.dev.contact@gmail.com`  
🔗 GitHub: [NathanSanchezDev](https://github.com/NathanSanchezDev)

---

# 📢 Quick Note

⚡ The library intelligently handles OpenAI’s special **o-series models** (o1, o3-mini, o4-mini) —  
**`temperature` and `top_p` are NOT sent for o-models** to match OpenAI’s requirements.  
Only **`max_completion_tokens`** is sent for these models.

This is **automatic** — no need for manual adjustments.