# Building ShadcnBlazor

## Prerendering

The docs site (`ShadcnBlazor.Docs`) uses Blazor WebAssembly prerendering for static site generation. Prerendering is **enabled by default** during builds and publishes.

### Disabling Prerendering

If you encounter issues with prerendering or want to skip it during publish, disable it with the `EnableBlazorWasmPrerendering` property:

```bash
dotnet publish -c Release -p:EnableBlazorWasmPrerendering=false
```

This is useful for:
- Debugging prerendering issues
- Faster CI/CD builds when static generation isn't needed
- Deployment scenarios where runtime rendering is preferred

### Configuration

Prerendering settings are located in `src/ShadcnBlazor.Docs/ShadcnBlazor.Docs.csproj`:

- `BlazorWasmPrerenderingBaseHref`: Base path for prerendered routes (`/`)
- `BlazorWasmPrerenderingUrlPathToExplicitFetch`: List of routes to prerender
- `BlazorWasmPrerenderingServerPort`: Port range for the prerenderer (5050-6000)
