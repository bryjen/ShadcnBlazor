# Sonner & Vaul Inner Workings

## Overview

Sonner and Vaul are React component libraries for notifications and drawers respectively. This document covers how they're integrated into ShadcnBlazor with Blazor components, the architecture, and key patterns.

---

## Sonner Architecture

### Core Concepts

**Sonner** is a toast notification library built on React. Key characteristics:
- Renders a `<Toaster>` component that manages all toasts
- Toasts are created via `toast()`, `toast.success()`, `toast.error()`, `toast.custom()`, etc.
- Each toast is an `<li>` element within the Toaster's `<ol>` container
- Toasts have `data-styled="true"` or `data-styled="false"` attribute controlling CSS styling
- CSS variables control appearance: `--normal-bg`, `--normal-border`, `--normal-text`, etc.

### Toast Structure

**Styled Toast (`data-styled="true"`):**
```html
<li data-sonner-toast="true" data-styled="true">
  <div data-icon="">...</div>
  <div data-content="">
    <div data-title="">Title text</div>
    <div data-description="">Description text</div>
  </div>
  <div data-button="">...</div>
</li>
```

**Custom Toast (`data-styled="false"`):**
```html
<li data-sonner-toast="true" data-styled="false">
  <div data-content="">
    [Custom React component rendered here]
  </div>
</li>
```

### Toast Creation Methods

1. **`toast(message, options)`** - Standard toast with icon, message, buttons
   - Creates `data-styled="true"`
   - Renders in Sonner's structured format
   - Supports title, description, action buttons

2. **`toast.success(message, options)`** - Success toast with checkmark icon
   - `data-styled="true"`

3. **`toast.error(message, options)`** - Error toast with X icon
   - `data-styled="true"`

4. **`toast.custom(component, options)`** - Custom React component
   - Creates `data-styled="false"`
   - No default styling applied
   - Responsibility on the component for all styling
   - **Important:** Sonner doesn't know the component's final height until it renders

### Styling System

Sonner uses CSS variables that are set on the `<ol data-sonner-toaster>` element:
```css
[data-sonner-toaster] {
  --normal-bg: ...;
  --normal-border: ...;
  --normal-text: ...;
  --border-radius: 8px;
  --gap: 14px;
  --width: 356px;
  --offset-top: 24px;
  --offset-right: 24px;
  --offset-bottom: 24px;
  --offset-left: 24px;
}
```

**Key CSS Rules for Styled Toasts:**
```css
[data-sonner-toast][data-styled='true'] {
  padding: 16px;
  background: var(--normal-bg);
  border: 1px solid var(--normal-border);
  color: var(--normal-text);
  border-radius: 8px;
  box-shadow: 0px 4px 12px rgba(0, 0, 0, 0.1);
  width: 356px;
  display: flex;
  align-items: center;
  gap: 6px;
}

[data-sonner-toast][data-styled='true'] [data-content] {
  display: flex;
  flex-direction: column;
  gap: 2px;
}
```

### Multi-Toast Stacking

When multiple toasts exist:
1. Sonner measures each toast's height via `--initial-height` CSS variable
2. Sets `--index` for z-index ordering
3. Uses `--toasts-before` to calculate `--offset` for positioning
4. On hover, scales/moves background toasts via `transform: scaleY(...)`

**Critical:** Sonner must have accurate height measurements for proper stacking. If `toast.custom()` is called before content renders, Sonner measures an empty component and heights are wrong.

---

## Vaul Architecture

### Core Concepts

**Vaul** is a drawer/sidebar library. Key characteristics:
- Renders a `<Drawer.Root>` component containing the drawer state
- Drawer content is in `<Drawer.Content>` which creates a sheet/modal
- Supports snap points for drawer positioning
- Provides overlay and animations
- Built on Radix UI primitives

### Drawer Structure

```html
<div data-vaul-drawer-root="">
  <div data-vaul-overlay=""></div>
  <div data-vaul-drawer="">
    <div data-vaul-content="">
      [Drawer content]
    </div>
  </div>
</div>
```

### Key Components

1. **`<Drawer.Root>`** - Root container, manages state
2. **`<Drawer.Overlay>`** - Semi-transparent overlay
3. **`<Drawer.Content>`** - Main drawer panel, contains content
4. **`<Drawer.Title>`** - Accessibility title
5. **`<Drawer.Description>`** - Accessibility description

### Drawer Behavior

- **Direction:** Can open from any direction (top, bottom, left, right)
- **Dismissible:** Can be dismissed by overlay click, escape key, etc.
- **Modal:** Can be modal (overlay blocks interaction) or not
- **Snap Points:** Drawer can snap to specific heights (mobile drawer use cases)
- **Animations:** Smooth slide-in/slide-out with configurable timing

---

## Blazor Integration Pattern

### Architecture Overview

Both Sonner and Vaul run in a separate **React root container** in the DOM, separate from Blazor. To render Blazor components inside them:

1. **Blazor components are registered in a service registry** (fragment lookup table)
2. **Host components (SonnerHost, DrawerHost)** in the Blazor tree manage rendering
3. **JS interop bridges** between React and Blazor using `DotNetObjectReference`
4. **Hidden containers** are created for Blazor-rendered content
5. **React components** (BlazorHost) move these containers into React's DOM

### Flow Diagram

```
User Code
    ↓
SonnerService.ShowCustomAsync(fragment)
    ↓
Register fragment in registry
    ↓
Call JS: window.Sonner.showComponentStyled(fragmentId)
    ↓
JS: Create hidden container <div id="sonner-component-xyz">
    ↓
JS: Call C#: sonnerHostRef.ShowComponent(fragmentId, containerId)
    ↓
C#: SonnerHost adds fragmentId to _activeFragments, calls StateHasChanged()
    ↓
Blazor: Re-renders SonnerHost
    ↓
SonnerHost: Renders <SonnerComponentHost FragmentId="xyz" />
    ↓
SonnerComponentHost: Looks up fragment from registry, renders it in hidden div
    ↓
Blazor: Completes render, calls OnAfterRender
    ↓
C#: TaskCompletionSource signals render is complete, returns to JS
    ↓
JS: Now content is guaranteed to be in DOM
    ↓
JS: Call toast(React.createElement(BlazorHost))
    ↓
BlazorHost component mounts in React, its useLayoutEffect moves the div into React DOM
    ↓
Toast displays with proper styling and accurate height
```

### Key Components

#### 1. **SonnerHost.razor** / **DrawerHost.razor**
- Persistent Blazor components in App.razor
- Manage a set of active fragments
- Use `TaskCompletionSource` to signal when renders are complete
- Provide JS-invokable methods: `ShowComponent()`, `RemoveComponent()`

```csharp
[JSInvokable]
public async Task ShowComponent(string fragmentId, string containerId)
{
    _renderComplete = new TaskCompletionSource<bool>();
    _activeFragments.Add(fragmentId);
    StateHasChanged();

    // Wait for OnAfterRender to signal render is complete
    await _renderComplete.Task;
}

protected override void OnAfterRender(bool firstRender)
{
    if (!firstRender && _renderComplete != null)
    {
        _renderComplete.TrySetResult(true);
        _renderComplete = null;
    }
}
```

#### 2. **SonnerComponentHost.razor** / **DrawerComponentHost.razor**
- Instantiated by Host component for each active fragment
- Simple wrapper that renders the fragment
- Hidden div with unique ID allows JS to locate content

```razor
<div id="sonner-component-@FragmentId" style="display:none">
    @if (_fragment is not null)
    {
        @_fragment
    }
</div>

protected override void OnParametersSet()
{
    _fragment = Registry.TryGet(FragmentId);
}
```

#### 3. **BlazorHost (JS React component)**
- Created per toast/drawer
- Uses `useLayoutEffect` to move hidden container into React DOM
- Critical: `display: contents` allows nested Blazor content to participate in parent flexbox

```javascript
function createBlazorHost(containerId) {
  return function BlazorHost() {
    const hostRef = React.useRef(null);

    React.useLayoutEffect(() => {
      const host = hostRef.current;
      const node = document.getElementById(containerId);
      if (!host || !node) return;

      if (node.parentNode !== host) {
        host.appendChild(node);
        node.style.display = '';  // Unhide
      }
    }, [containerId]);

    return React.createElement('div', {
      ref: hostRef,
      style: { display: 'contents' }  // Don't create extra layout layer
    });
  };
}
```

#### 4. **Registry Services**
- `SonnerComponentRegistry` / `DrawerComponentRegistry`
- Simple dict: `fragmentId → RenderFragment`
- Thread-safe

---

## Two Approaches: Headless vs Custom

### **ShowHeadlessAsync** (Truly Headless)

```csharp
public async ValueTask<string?> ShowHeadlessAsync(
    RenderFragment fragment,
    SonnerOptions? options = null,
    CancellationToken cancellationToken = default) =>
    await ShowComponentAsync("showComponent", fragment, options, cancellationToken);
```

**JS:** Uses `toast.custom()` with NO inline styling
- Creates `data-styled="false"` toast
- Component is fully responsible for all styling
- No structure, no padding, no shadows - truly custom

**Use when:** You want complete control, minimal Sonner assumptions

### **ShowCustomAsync** (Styled Custom)

```csharp
public async ValueTask<string?> ShowCustomAsync(
    RenderFragment fragment,
    SonnerOptions? options = null,
    CancellationToken cancellationToken = default) =>
    await ShowComponentAsync("showComponentStyled", fragment, options, cancellationToken);
```

**JS:** Uses `toast()` (not `toast.custom()`) with inline styling
- Creates `data-styled="true"` toast
- Sonner applies standard structure and styling
- Proper multi-toast stacking and hover behavior
- Component receives styled backdrop

**Use when:** You want Sonner's styling and proper multi-toast behavior

**Key Difference:**
| Aspect | Headless | Custom |
|--------|----------|--------|
| Toast method | `toast.custom()` | `toast()` |
| data-styled | false | true |
| Default styling | None | Full |
| Multi-toast stacking | ⚠️ May break | ✓ Works perfectly |
| Hover behavior | ⚠️ May break | ✓ Works perfectly |
| Component control | 100% | Follows Sonner structure |

---

## Timing Critical Pattern: The Render Wait

**Problem:** If you call `toast()` before Blazor finishes rendering, heights are wrong.

**Solution:** `TaskCompletionSource` pattern

```csharp
// C#: ShowComponent waits for render
await _renderComplete.Task;  // Blocks until OnAfterRender fires

// JS: Only after C# returns does toast get created
await sonnerHostRef.invokeMethodAsync('ShowComponent', ...);
// Content is GUARANTEED to be in DOM here
toast(...);  // Safe to create toast
```

This ensures:
1. Fragment is registered
2. Blazor renders it
3. Content is in DOM
4. Toast is created with accurate measurements

---

## Drawer Integration (Vaul)

### Flow

Similar to Sonner, but:
1. Drawer opening is controlled by C# (open state)
2. Multiple drawers not supported simultaneously (usually)
3. Uses `MutationObserver` pattern to detect when content appears

```javascript
// Wait for drawer content container to appear
const observer = new MutationObserver(() => {
  if (document.getElementById(containerId)) {
    observer.disconnect();
    resolve();
  }
});

observer.observe(document.body, {
  childList: true,
  subtree: true
});
```

### DrawerHost.razor

```csharp
[JSInvokable]
public async Task ShowComponentAsync(string fragmentId, string containerId)
{
    var fragment = Registry.TryGet(fragmentId);
    if (fragment is null) return;

    _fragments[containerId] = fragment;
    StateHasChanged();

    // Give Blazor a chance to render
    await Task.Delay(1);
}
```

---

## CSS Custom Properties & Theming

Sonner and Vaul both respect CSS custom properties set at the Toaster/Root level:

```javascript
// sonner-interop.js
const styleEl = document.createElement('style');
styleEl.textContent = `
  #sonner-container [data-sonner-toaster] {
    --normal-bg: var(--card);
    --normal-border: var(--border);
    --normal-text: var(--foreground);
    /* ... theme colors ... */
  }
`;
document.head.appendChild(styleEl);
```

This allows Sonner/Vaul to inherit design system tokens from the Blazor app's CSS variables.

---

## Performance Considerations

1. **React Root Creation:** Done once per app lifecycle
2. **Component Rendering:** Goes through Blazor's normal render cycle (fast, optimized)
3. **DOM Operations:** Minimal - just moving pre-rendered divs
4. **Memory:** Toasts cleaned up when dismissed, registries cleared
5. **Stacking:** Sonner does height calculations in CSS (no JS layout recalcs)

---

## Server Compatibility

This architecture works on **both Blazor WASM and Blazor Server** because:

1. ✓ No `Blazor.rootComponents` API (WASM-only)
2. ✓ Uses `DotNetObjectReference` (Server-compatible)
3. ✓ Renders via normal Blazor component tree
4. ✓ JS interop via standard `invokeMethodAsync`

---

## Known Issues & Workarounds

### Issue: Multi-Toast Heights Wrong
**Cause:** Sonner measured toast before content rendered
**Fix:** Always wait for `ShowComponent()` to complete before creating toast

### Issue: Hover Stacking Broken
**Cause:** Using `toast.custom()` with `data-styled="false"`
**Fix:** Use `toast()` instead, use `ShowCustomAsync()`

### Issue: Content Not Appearing
**Cause:** Fragment not registered or wrong fragmentId
**Fix:** Verify registry has fragment, check fragmentId matches

### Issue: Memory Leak
**Cause:** Not calling `RemoveComponent()` on cleanup
**Fix:** Always provide `onDismiss` callback that cleans up

---

## Future Improvements

1. **Batching:** Multiple toasts created at once could batch renders
2. **Caching:** Pre-render common fragments
3. **Pooling:** Reuse DOM containers instead of creating new ones
4. **Animation:** Add transition timing customization
5. **Accessibility:** Enhance ARIA labels and keyboard nav
