# ShadcnBlazor

# shadcn/ui components for Blazor

[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/bryjen/ShadcnBlazor/dotnet-desktop.yml?branch=master&logo=github&style=flat-square)](https://github.com/bryjen/ShadcnBlazor/actions)
[![GitHub](https://img.shields.io/github/license/bryjen/ShadcnBlazor?color=594ae2&logo=github&style=flat-square)](https://github.com/bryjen/ShadcnBlazor/blob/master/LICENSE)
[![GitHub Repo stars](https://img.shields.io/github/stars/bryjen/ShadcnBlazor?color=594ae2&style=flat-square&logo=github)](https://github.com/bryjen/ShadcnBlazor/stargazers)
[![Contributors](https://img.shields.io/github/contributors/bryjen/ShadcnBlazor?color=594ae2&style=flat-square&logo=github)](https://github.com/bryjen/ShadcnBlazor/graphs/contributors)
[![NuGet](https://img.shields.io/nuget/v/ShadcnBlazor.Cli?color=ff4081&label=CLI%20tool&logo=nuget&style=flat-square)](https://www.nuget.org/packages/ShadcnBlazor.Cli/)

Build stunning, interactive web applications with ShadcnBlazor — the open-source shadcn/ui port for Blazor. Components are copied into your project so you own the code.

**🌐 [Documentation](https://bryjen.github.io/ShadcnBlazor/)**

## 💎 Why Choose ShadcnBlazor?

📖 **You own the code** — Components are copied into your project, not installed as a package  
🎨 **Beautiful design** — Tailwind CSS v4 and semantic CSS variables for theming  
💻 **Write in C#** — Minimal JavaScript, Blazor-native components  
✅ **Accessible & customizable** — Modify and extend components however you need  

## 🚀 Getting Started

### Install the CLI

```bash
dotnet tool install --global shadcnblazor
```

### From New Project

Create a Blazor project from a template, then add components:

```bash
# Create a new WebAssembly project
shadcnblazor new --wasm --proj MyApp --namespace MyApp

# Or create a Blazor Server project
shadcnblazor new --server --proj MyApp --namespace MyApp

# Navigate into the project and add components
cd MyApp
shadcnblazor component add button

# Add all components at once
shadcnblazor component add --all
```

New projects use **Tailwind.MSBuild** to automatically compile `wwwroot/css/input.css` to `wwwroot/css/app.css` on each build. No additional setup is required.

### From Existing Project

Initialize your existing Blazor project, then add components:

```bash
cd MyBlazorProject
shadcnblazor init
shadcnblazor component add button
```

> **Note:** Initializing an existing project does not add Tailwind compilation. Add `Tailwind.MSBuild` to your project, or use the standalone `tailwindcss` CLI to compile `input.css` to `app.css`.

### Example Usage

After adding the Button component:

```razor
<Button>Click Me</Button>

<Button Variant="@Variant.Outline" OnClick="HandleClick">
    Outline Button
</Button>

<Button Size="@Size.Sm">Small</Button>
<Button Size="@Size.Lg">Large</Button>

@code {
    private void HandleClick() { /* ... */ }
}
```

For more examples, see the [documentation](https://bryjen.github.io/ShadcnBlazor/).

## 📁 Project Structure

The CLI creates a `shadcn-blazor.yaml` configuration file and copies components into your project:

```
componentsOutputDir: ./Components/Core
rootNamespace: MyApp
```

Components are generated with proper namespaces. Example structure for the Button component:

```
Components/Core/Button/
├── Button.razor
├── ButtonGroup.razor
├── ButtonShared.cs
└── ToggleButton.razor
```

## 🤝 Contributing

Contributions from the community are welcome.

📚 Check out the codebase and open an issue or pull request.  
🧪 Test changes against `tests/SampleBlazorProject` using the reset script: `./reset.ps1`

## ⚙️ Version Support

| ShadcnBlazor | .NET | Support |
| :--- | :---: | :---: |
| 1.x | [.NET 9](https://dotnet.microsoft.com/download/dotnet/9.0) | ✅ Full Support |

## License

Distributed under the MIT License. See [LICENSE](LICENSE) for more information.
