# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ShadcnBlazor is a .NET 9 Blazor component library inspired by shadcn/ui, using Tailwind CSS v4. It includes:
- **ShadcnBlazor**: Main component library (Razor SDK)
- **ShadcnBlazor.Cli**: CLI tool for initializing and adding components to Blazor projects
- **ShadcnBlazor.Docs**: Documentation/demo site (Blazor WebAssembly)
- **SampleBlazorProject**: Test project for CLI functionality

## Build and Run Commands

### Build the solution
```bash
dotnet build
```

### Run the documentation site
```bash
cd src/ShadcnBlazor.Docs
dotnet run
```

### Test CLI locally
```bash
# Build CLI
cd src/ShadcnBlazor.Cli
dotnet build

# Test in SampleBlazorProject
cd ../../tests/SampleBlazorProject
dotnet run --project ../../src/ShadcnBlazor.Cli/ShadcnBlazor.Cli.csproj -- init
dotnet run --project ../../src/ShadcnBlazor.Cli/ShadcnBlazor.Cli.csproj -- component add button
```

### Reset test project
```powershell
cd tests/SampleBlazorProject
./reset.ps1
```

## Architecture

### Component System

**Base Class:** All components inherit from `ShadcnComponentBase` (located in `src/ShadcnBlazor/Shared/ShadcnComponentBase.cs`):
- Provides `TwMerge` injection for Tailwind class merging
- Provides `Class` parameter for custom classes
- Provides `AdditionalAttributes` for arbitrary HTML attributes
- `MergeCss()` helper merges component classes with user-provided classes

**Component Metadata:** Components are decorated with `ComponentMetadataAttribute`:
```csharp
[ComponentMetadata(Name = "Button", Description = "...", Dependencies = ["alert"])]
```
This metadata is used by the CLI to:
- Discover available components
- Resolve dependency trees
- Display component information

**Component Structure:**
- Each component lives in `src/ShadcnBlazor/Components/{ComponentName}/`
- Typically includes `{ComponentName}.razor` and `{ComponentName}.razor.cs`
- Components use Tailwind CSS classes extensively
- Styling is handled via CSS custom properties (e.g., `--background`, `--foreground`)

### CLI Tool Architecture

**Command Structure:**
- Built with Spectre.Console.Cli
- Commands defined in `src/ShadcnBlazor.Cli/Commands/`
- Main commands:
  - `InitCommand`: Sets up a Blazor project with ShadcnBlazor
  - `AddCommand`: Adds components with dependency resolution
  - `ListCommand`: Lists available components
  - `InfoCommand`: Shows component details

**How Component Addition Works:**
1. CLI reads `ShadcnBlazor.dll` to discover components via reflection
2. Reads component metadata to build dependency tree
3. Copies component files from `{assembly_dir}/components/{ComponentName}` to target project
4. Updates namespaces in copied files from `ShadcnBlazor.*` to target project namespace
5. Updates `using` statements to reference target project namespaces

**Key Services:**
- `ComponentService`: Loads and finds components from assembly
- `ConfigService`: Manages `shadcn-blazor.yaml` configuration file
- `NamespaceService`: Updates namespace declarations in copied files
- `UsingService`: Updates using statements in copied files
- `ProjectValidator`: Validates Blazor project structure
- `CsprojService`: Modifies .csproj files (e.g., add package references)

**Configuration File (`shadcn-blazor.yaml`):**
```yaml
componentsOutputDir: ./Components/Core
rootNamespace: YourProject
```

### Styling System

**Tailwind CSS v4:**
- Uses new `@import "tailwindcss"` syntax
- Source CSS: `src/ShadcnBlazor.Docs/wwwroot/css/input.css`
- `@source` directives point to component directories for class scanning
- Theme tokens defined as CSS custom properties in `:root` and `.dark`
- Pre-built CSS files: `shadcn_blazor_in.css` (source) and `shadcn_blazor_out.css` (compiled)

**CSS Custom Properties:**
- Semantic tokens: `--background`, `--foreground`, `--primary`, `--destructive`, etc.
- Extended tokens: `--background-dark`, `--chat-sidebar`, `--sidebar-text`, etc.
- Both light and dark themes supported

**TailwindMerge Integration:**
- Injected via DI: `builder.Services.AddTailwindMerge();`
- Used in components to merge user-provided classes with component defaults
- Package: `TailwindMerge.NET` v1.2.0

## Component File Output

Components are copied to the output directory during build via `.csproj` configuration:
```xml
<None Include="Components/**/*" CopyToOutputDirectory="Always" />
<None Include="Shared/**/*" CopyToOutputDirectory="Always" />
```

The CLI expects to find component files in the assembly output directory under `components/{ComponentName}/`.

## Important Patterns

### Adding New Components

1. Create component directory under `src/ShadcnBlazor/Components/{ComponentName}/`
2. Create `.razor` and `.razor.cs` files
3. Inherit from `ShadcnComponentBase`
4. Add `ComponentMetadataAttribute` with name, description, and dependencies
5. Use `MergeCss()` to merge classes properly
6. Component files will automatically copy to output directory

### Testing CLI Changes

Always test CLI changes against `tests/SampleBlazorProject`:
1. Reset the test project: `./reset.ps1`
2. Run CLI commands to verify functionality
3. Check that files are copied correctly
4. Verify namespaces are updated properly

### Namespace Transformation

When components are added to a target project:
- `ShadcnBlazor.Components.Button` → `{TargetNamespace}.Components.Core.Button`
- `using ShadcnBlazor.Shared` → `using {TargetNamespace}.Shared`
- `@namespace ShadcnBlazor.Components.Alert` → `@namespace {TargetNamespace}.Components.Core.Alert`

## Context Folder

The `context/MudBlazor/` folder contains a reference copy of the MudBlazor library for architectural inspiration and patterns. This is not part of the build output.

## Dependencies

- .NET 9.0
- Microsoft.AspNetCore.Components.Web 9.0.6
- TailwindMerge.NET 1.2.0
- Spectre.Console.Cli 0.53.1 (CLI only)
- YamlDotNet 16.3.0 (CLI only)
- EasyAppDev.Blazor.Icons.Lucide 2.0.4 (Docs only)
