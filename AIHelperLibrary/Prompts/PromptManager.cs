using System;
using System.Collections.Generic;

namespace AIHelperLibrary.Prompts
{
    public static class PromptManager
    {
        private static readonly Dictionary<PromptType, string> Prompts = new()
        {
            { PromptType.Summarize, "Summarize the following text:" },
            { PromptType.Explain, "Explain this concept in simple terms:" },
            { PromptType.BlogPost, "Write a blog post about the following topic:" },
            { PromptType.CodeReview, "Review the following code and suggest improvements:" }
        };

        public static string GetPrompt(PromptType type)
        {
            if (Prompts.TryGetValue(type, out var prompt))
            {
                return prompt;
            }

            throw new ArgumentException($"Prompt for type '{type}' not found.");
        }
    }
}