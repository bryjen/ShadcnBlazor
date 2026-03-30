# Tier 4 Test Plan: Dialog, Sheet, Select, Combobox, MultiSelect, DropdownMenu, ContextMenu

## Component Test Coverage Matrix

### Dialog
**API Surface:** Service-based, `DialogOptions` (MaxWidth, FullScreen, FullWidth, CloseButton), Title, Description, Content

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **DialogTest1** | Basic Dialog | Simple title, description, buttons, click to open/close |
| **DialogTest2** | Sizes & Options | Different MaxWidth options (xs, sm, md, lg, xl), FullScreen, FullWidth |
| **DialogTest3** | Complex Content | Dialog with form, nested components, scrollable content |

---

### Sheet
**API Surface:** Side panel variant (similar to Dialog but slides from side), Position (Left, Right, Top, Bottom), Size

#### Viewer Tests (2 tests)
| Test | Focus | Details |
|------|-------|---------|
| **SheetTest1** | Basic Sheet | Slide from right with title and close button |
| **SheetTest2** | Positions & Sizes | Different slide positions, width variations |

---

### Select
**API Surface:** Declarative items, `SelectItem`, `SelectGroup`, `SelectLabel`, Value/ValueChanged, Disabled, Multiple selection (if supported)

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **SelectTest1** | Basic Select | Dropdown with 5 items, click to select |
| **SelectTest2** | Grouped Items | SelectGroup with labels, separators, disabled items |
| **SelectTest3** | Sizes & States | Different sizes, disabled select, selected state display |

---

### Combobox
**API Surface:** Searchable select, filterable items, `SelectItem`, Value/ValueChanged, Keyboard navigation

#### Viewer Tests (2 tests)
| Test | Focus | Details |
|------|-------|---------|
| **ComboboxTest1** | Basic Search | Type to filter items, arrow keys to navigate, enter to select |
| **ComboboxTest2** | Advanced Features | Custom search logic, grouped items, keyboard shortcuts |

---

### MultiSelect
**API Surface:** Multiple value selection, Chip/tag display, Add/remove items, Value/ValueChanged

#### Viewer Tests (2 tests)
| Test | Focus | Details |
|------|-------|---------|
| **MultiSelectTest1** | Basic Multi | Select multiple items, display as chips/tags |
| **MultiSelectTest2** | Interactions | Remove chips, search while items selected, clear all |

---

### DropdownMenu
**API Surface:** Trigger-based menu, `DropdownMenuItem`, `DropdownMenuCheckboxItem`, `DropdownMenuRadioItem`, Groups, Separators, Shortcuts

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **DropdownMenuTest1** | Basic Menu | Click trigger, select items, click outside to close |
| **DropdownMenuTest2** | Advanced Items | Checkboxes, radio groups, separators, groups, disabled items |
| **DropdownMenuTest3** | Submenus | Nested menus, hover to open submenus, keyboard navigation |

---

### ContextMenu
**API Surface:** Right-click trigger, `ContextMenuItem`, `ContextMenuCheckboxItem`, `ContextMenuRadioItem`, Groups, Submenus

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **ContextMenuTest1** | Basic Context | Right-click to open menu, select item, menu closes |
| **ContextMenuTest2** | Menu Items | Checkboxes, radio groups, disabled items, separators |
| **ContextMenuTest3** | Submenus | Nested menus, keyboard navigation, complex hierarchies |

---

## Implementation Priority

**Tier 4 (Core - 7 components × 18 tests = estimated ~20 tests):**

1. Dialog (3) → Sheet (2) → Select (3) → Combobox (2) → MultiSelect (2) → DropdownMenu (3) → ContextMenu (3)

**Total: 18 tests minimum**

---

## Test Quality Checklist

- [ ] Dialog shows title, description, buttons
- [ ] Sheet slides from expected direction
- [ ] Select dropdown opens/closes on click
- [ ] Combobox filters on typing
- [ ] MultiSelect shows selected items as chips
- [ ] DropdownMenu items are clickable
- [ ] ContextMenu opens on right-click
- [ ] Keyboard navigation works (arrows, enter, escape)
- [ ] Disabled states are visually distinct
- [ ] Submenus open on hover/click
- [ ] All tests compile in Tests.Shared project

---

## Expected Test Count at Tier 4 Completion

- **Tier 1-3:** 43 viewer tests
- **Tier 4:** ~18 viewer tests
- **Total:** ~61 viewer tests in the sidebar explorer

---

## Notes

- Dialog and Sheet require service injection or manual state management
- Select and Combobox are complex with item registries
- DropdownMenu and ContextMenu are similar (menu vs context-menu trigger)
- MultiSelect is variation of Select with multiple values
- All share popover-based positioning for dropdowns
