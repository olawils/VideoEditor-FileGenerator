# vegen – Video Editor Boilerplate Generator

An autonomous, standalone native .NET 9.0 CLI tool designed to instantly scaffold a high-performance Apple Clean Architecture iOS SwiftUI application configured for video editing paradigms without requiring Xcode configuration dependencies.

## Architecture & Codebase Navigation
This tool is segregated into routing logic, structural generators, and raw templates.

- **`Program.cs`**: The main execution router. Evaluates positional arguments to intercept and fork generation cycles towards either a full project initialization or an isolated ad-hoc single file creation.
- **`src/Generators/ProjectGenerator.cs`**: Orchestrates deep, recursive folder synthesis. Pre-compiles the entire `Domain -> Data -> Presentation` tree mapping payload buffer dictionaries.
- **`src/Generators/PbxProjGenerator.cs`**: The proprietary Xcode configuration builder. Algorithmically synthesizes an MD5 UUID-deterministic Apple `.xcodeproj/project.pbxproj` map pointing instantly to your raw swift files, mitigating Xcode parsing errors and compilation locks.
- **`src/Generators/ResourceGenerator.cs`**: The isolated file handler utilized when triggering specific views and models dynamically via manual terminal calls. Bypasses structure constraints and prints immediately to local `cwd`.
- **`src/Templates/SwiftTemplates.cs`**: The static internal repository holding raw SwiftUI layout matrices, core Clean Architecture domain protocols, and core entity skeletons logic.

## Prerequisites
- Any Operating System (macOS, Windows, or Linux).
- `.NET 9.0 SDK` installed and reachable in path.

## Installation (Global Executable Setup)
1. **CD into the source map:**
   ```bash
   cd /Users/mac/Downloads/VideoEditorBoilerplateGen_source
   ```

2. **Trigger the NuGet Packager Engine:**
   ```bash
   dotnet pack
   ```

3. **Link to Global Terminals:**
   ```bash
   dotnet tool install --global --add-source ./nupkg VideoEditorBoilerplateGen
   ```

*(Ensure `$HOME/.dotnet/tools` is explicitly declared in `~/.zshrc` PATH bindings constraints).*

## Usage Specifications

### Full Initialization
Generates the core App structural container recursively pointing directly to `{Title}.xcodeproj`.
```bash
vegen --name AlphaCore 
```
*(Optionally include `--output ./path` and `--bundle com.your.domain` constraints)*

### Ad-hoc Modular Integrations
Generates autonomous SwiftUI scaffolding templates targeted at standalone configurations avoiding existing source-tree annihilation protocols (`File.Exists`).
```bash
# Generate standalone View Component
vegen view TimelineSlider

# Generate standalone Model Entity
vegen model AnalyticsData

# Generate Global Constants Dark-Thematic Setup
vegen theme BaseTheme

# Generate blank swift constraint
vegen file FallbackUtil
```
