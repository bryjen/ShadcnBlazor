# UI Component Library Reference

This document maps ShadcnBlazor components to their source implementations in shadcn/ui and MudBlazor.

## Base Paths

| Library | Component Source | Documentation Source |
|---------|---|---|
| **shadcn/ui** | `./context/ui/apps/v4/registry/new-york-v4/ui/` | `./context/ui/apps/v4/content/docs/components/radix/` |
| **MudBlazor** | `./context/mudblazor/src/MudBlazor/Components/` | Online: https://mudblazor.com/docs |

**Note:** shadcn/ui component files are individual `.tsx` files in the component base path. MudBlazor components are organized in subdirectories by component name.

## ShadcnBlazor Components → shadcn/ui Source & MudBlazor Equivalents

| ShadcnBlazor Component | shadcn/ui Source File | MudBlazor Equivalent |
|-----------|---|---|
| **Accordion** | `accordion.tsx` | `ExpansionPanel` |
| **Alert** | `alert.tsx` | `Alert` |
| **Avatar** | `avatar.tsx` | `Avatar` |
| **Badge** | `badge.tsx` | `Chip` |
| **Button** | `button.tsx` | `Button` |
| **Card** | `card.tsx` | `Card` / `Paper` |
| **Checkbox** | `checkbox.tsx` | `CheckBox` |
| **Combobox** | `combobox.tsx` | `Autocomplete` |
| **ContextMenu** | `context-menu.tsx` | `Menu` |
| **DataTable** | `table.tsx` | `DataGrid` / `Table` |
| **Dialog** | `dialog.tsx` | `Dialog` |
| **Drawer** | `drawer.tsx` | `Drawer` |
| **DropdownMenu** | `dropdown-menu.tsx` | `Menu` |
| **Field** | `field.tsx` | `Field` / `TextField` |
| **FocusTrap** | *(not in registry - likely custom)* | `FocusTrap` |
| **Input** | `input.tsx` | `TextField` / `Input` |
| **Label** | `label.tsx` | `Typography` *(conceptual)* |
| **MultiSelect** | `combobox.tsx` *(variant)* | `Select` *(with multiple)* / `Autocomplete` |
| **Popover** | `popover.tsx` | `Popover` |
| **Radio** | `radio-group.tsx` | `Radio` / `RadioGroup` |
| **Select** | `select.tsx` | `Select` |
| **Skeleton** | `skeleton.tsx` | `Skeleton` |
| **Slider** | `slider.tsx` | `Slider` |
| **Sonner** | `sonner.tsx` | `Snackbar` |
| **Switch** | `switch.tsx` | `Switch` |
| **Tabs** | `tabs.tsx` | `Tabs` |
| **Textarea** | `textarea.tsx` | `TextField` *(multiline)* |
| **ToggleButton** | `toggle.tsx` | `Toggle` |
| **Tooltip** | `tooltip.tsx` | `Tooltip` |

### Additional shadcn/ui components (not yet ported to ShadcnBlazor)

- `alert-dialog`, `aspect-ratio`, `breadcrumb`, `button-group`, `calendar`, `carousel`, `chart`, `collapsible`, `command`, `direction`, `empty`, `form`, `hover-card`, `input-group`, `input-otp`, `item`, `kbd`, `menubar`, `native-select`, `navigation-menu`, `pagination`, `progress`, `resizable`, `scroll-area`, `separator`, `sheet`, `sidebar`, `spinner`, `table`, `toggle-group`

### Additional MudBlazor components (not yet ported to ShadcnBlazor)

- `AppBar`, `Breadcrumbs`, `BreakpointProvider`, `ButtonGroup`, `Carousel`, `Chart`, `ChipSet`, `Collapse`, `ColorPicker`, `Container`, `DatePicker`, `Divider`, `DropZone`, `Element`, `ExitPrompt`, `FileUpload`, `Form`, `Grid`, `Hidden`, `Highlighter`, `Hotkey`, `Icon`, `Image`, `InputControl`, `Layout`, `Link`, `List`, `Main`, `Mask`, `MessageBox`, `NavMenu`, `NumericField`, `Overlay`, `PageContentNavigation`, `Pagination`, `Picker`, `Progress`, `Rating`, `Render`, `RTLProvider`, `ScrollToTop`, `Spacer`, `SplitPanel`, `Stack`, `Stepper`, `SwipeArea`, `Table`, `TableSimple`, `ThemeProvider`, `TimePicker`, `Timeline`, `ToolBar`, `TreeView`, `Typography`, `Virtualize`