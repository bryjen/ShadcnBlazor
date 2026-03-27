# ShadcnBlazor Docs Styling & Page Architecture Guide

This guide defines how to create and style documentation pages and examples within the `src/ShadcnBlazor.Docs` project.

---

## 1. Page Architecture

All component documentation pages should follow a standard layout to ensure a consistent user experience.

### Main Page Structure
- **File**: `src/ShadcnBlazor.Docs/Pages/Components/[ComponentName]/[ComponentName]Page.razor`
- **Route**: `@page "/components/[component-name]"`
- **SEO**: Use `<PageTitle>` and `<HeadContent>` for metadata.

### Standard Layout Wrapper: `<ComponentDocPage>`
Wrap the entire page content in this component. It handles the title, description, and automatic TOC (Table of Contents).

| Parameter | Description |
|-----------|-------------|
| `Metadata` | Fetched via `ComponentRegistryService.GetMetadata("Name")` |
| `InstallCommand` | The CLI command to install the component |
| `ApiDoc` | API documentation data from `ApiDocumentation.[Name]` |
| `UnderHeader` | RenderFragment for badges or status indicators below the title |
| `ChildContent` | The main body of the documentation |

---

## 2. Page Sections

### `<FeaturesSection>`
Use this at the top of the page (within `ChildContent`) to highlight key capabilities.

```razor
<FeaturesSection Title="Features" Id="features" Features="@Features" SourceLink="[ComponentName]" />
```

### `<DocsSection>` and `<DocsSubSection>`
Use these for organizing content. They automatically generate IDs for the "On This Page" navigation.

```razor
<DocsSection Title="Usage" Id="usage">
    <p>Describe how to use the component here.</p>
</DocsSection>

<DocsSection Title="Examples" Id="examples">
    <DocsSubSection Title="Basic" Id="basic">
        <!-- Content -->
    </DocsSubSection>
</DocsSection>
```

---

## 3. Interactive Examples

### `<ComponentPreview>`
The standard way to show a component in action with a "Show Code" toggle.

```razor
<ComponentPreview CodeFiles="@(new[] { Snippets.Components_[Name]_Examples_[ExampleName] })">
    <div class="w-full lg:mx-[clamp(5%,10%,10%)]">
        <MyExampleComponent />
    </div>
</ComponentPreview>
```

### Directory Convention for Examples
- Store examples in `src/ShadcnBlazor.Docs/Pages/Components/[ComponentName]/Examples/`.
- Name them `[ComponentName][Variant]Example.razor`.
- The `ShadcnBlazor.Docs.Compiler` automatically converts these files into snippets available via the `Snippets` class.

---

## 4. Design Guidelines for Docs

- **Prose**: Use standard `<p>` tags with `mb-4` or `mb-3` for spacing.
- **Code**: Use `<code>` tags for inline property/method names.
- **Spacing**: Use `flex flex-col gap-8` for the main page layout (handled by `ComponentDocPage`).
- **Aesthetic**:
    - Avoid excessive borders; use the existing `border-border` token.
    - Use `text-muted-foreground` for secondary descriptions.
    - Ensure all examples are centered within the preview container (using `flex justify-center items-center`).

## 5. Metadata Integration
Ensure you register the component in `src/ShadcnBlazor/Services/ComponentRegistry.cs` so that `ComponentRegistryService` can find it and provide the correct metadata to the doc pages.
