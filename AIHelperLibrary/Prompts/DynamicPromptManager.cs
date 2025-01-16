using System.Collections.Generic;

namespace AIHelperLibrary.Prompts
{
    public class DynamicPromptManager
    {
        private readonly Dictionary<string, string> _prompts = new();

        public void AddPrompt(string key, string prompt)
        {
            _prompts[key] = prompt;
        }

        public string GetPrompt(string key)
        {
            if (_prompts.TryGetValue(key, out var prompt))
            {
                return prompt;
            }
            throw new KeyNotFoundException($"Prompt with key '{key}' not found.");
        }
    }
}