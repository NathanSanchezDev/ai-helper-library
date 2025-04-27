# Changelog

All notable changes to **AIHelperLibrary** will be documented in this file.

This project adheres to [Semantic Versioning](https://semver.org/).

---

## [1.1.0] - 2025-04-27
### Added
- **Multi-Provider Support**: Now supports Anthropic Claude models (Claude 3, Claude 3.5, Claude 3.7 families).
- **Expanded Model Support**: Correct handling for OpenAI o-series models (`o1`, `o3-mini`, `o4-mini`) with special parameter behavior (`max_completion_tokens`).
- **Advanced Configuration Options**:
  - Proxy URL and port settings.
  - Retry logic for transient API failures.
  - Custom request headers.
- **Dynamic Prompt Manager Integration** for multi-provider chat and text generation.
- **API Client Factory** to create clients dynamically based on provider and configuration.

### Changed
- **Renamed** `AIExtensionHelperConfiguration` ➔ **`OpenAIConfiguration`** for clarity.
- OpenAI model handling now respects updated API behavior for o-series models (no `temperature` or `top_p` for o-series).
- Improved validation of chat-capable models (`OpenAIModelHelper`).

### Fixed
- Proper fallback handling for unexpected response formats across providers.

---

## [1.0.1] - 2025-04-24
### Fixed
- Minor internal optimizations.
- Early bugfixes after first open source release.

---

## [1.0.0] - 2025-04-23
### Added
- Initial public release of AIHelperLibrary.
- OpenAI GPT-3.5, GPT-4, GPT-4o support.
- Predefined prompt management and dynamic multi-turn conversations.