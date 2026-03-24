# Tier 3 Test Plan: Avatar, Accordion, Tabs, Tooltip, Popover

## Component Test Coverage Matrix

### Avatar
**API Surface:** `Src`, `Alt`, `ChildContent` (fallback), `Badge`, `Size` (Sm, Md, Lg), `AvatarGroup` for grouping

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **AvatarTest1** | Image & Fallback | Image display, broken image fallback, initials fallback |
| **AvatarTest2** | Sizes & Badge | All sizes (Sm, Md, Lg), with and without badge overlay |
| **AvatarTest3** | AvatarGroup | Multiple avatars with size-based overlap, visual stacking |

#### Viewer Test Details

**AvatarTest1.razor - Image & Fallback**
```
- Avatar with valid image (Src="..." Alt="...")
- Avatar with broken image src (falls back to initials)
- Avatar with text fallback (ChildContent with "JD" or initials)
- Show alt text usage
```

**AvatarTest2.razor - Sizes & Badge**
```
- Size.Sm with no badge
- Size.Md with badge (dot/circle overlay)
- Size.Lg with badge
- Compare size differences visually
```

**AvatarTest3.razor - AvatarGroup**
```
- AvatarGroup Size.Sm (tight overlap)
- AvatarGroup Size.Md (medium overlap)
- AvatarGroup Size.Lg (loose overlap)
- Show 4-5 avatars in each group with overlap visualization
```

---

### Accordion
**API Surface:** `ChildContent` (AccordionItems), Single/Multiple open modes (if supported), AccordionItem/Trigger/Content pattern

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **AccordionTest1** | Basic Expansion | Single accordion with 3 items, click to expand/collapse |
| **AccordionTest2** | Open/Closed States | Mix of pre-opened and pre-closed items |
| **AccordionTest3** | Content Variations | Items with simple text, with rich HTML content, with nested components |

#### Viewer Test Details

**AccordionTest1.razor - Basic Expansion**
```
- Accordion with 3 items:
  - Item 1 (default closed)
  - Item 2 (default closed)
  - Item 3 (default closed)
- Click each to expand/collapse
- Show trigger animation (chevron rotation if present)
```

**AccordionTest2.razor - Open/Closed States**
```
- Item 1 (pre-opened, showing content)
- Item 2 (closed)
- Item 3 (pre-opened, showing content)
- Demonstrate multiple items can be open simultaneously
```

**AccordionTest3.razor - Content Variations**
```
- Item with plain text content
- Item with formatted HTML (bold, links, code snippets)
- Item with nested components (buttons, inputs, etc.)
- Item with tall content (scrollable if needed)
```

---

### Tabs
**API Surface:** `Orientation` (Horizontal, Vertical), TabsTrigger/TabsContent pattern, Selected/Active tab

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **TabsTest1** | Horizontal Tabs | 4-5 tabs, click to switch content |
| **TabsTest2** | Vertical Tabs | Orientation="Vertical", tabs on left side |
| **TabsTest3** | Content & States | Pre-selected tab, tabs with icons, various content types |

#### Viewer Test Details

**TabsTest1.razor - Horizontal Tabs**
```
- Tabs (Orientation="Horizontal" or default):
  - Tab 1: Account (active by default)
  - Tab 2: Profile
  - Tab 3: Notifications
  - Tab 4: Settings
- Click each to show corresponding content
- Show visual indicator (underline/highlight) on active tab
```

**TabsTest2.razor - Vertical Tabs**
```
- Tabs Orientation="Vertical":
  - Tabs listed vertically on left
  - Content displayed on right
  - 3-4 tabs for comparison
```

**TabsTest3.razor - Content & States**
```
- Tab with pre-selected state (default open)
- Tab with icon + text trigger
- Tab with simple text content
- Tab with rich content (forms, components)
- Tab with disabled state (if supported)
```

---

### Tooltip
**API Surface:** `Content`, `Anchor` (trigger element), `Offset`, `Open` (controlled state), placement (implicit from PopoverPlacement)

#### Viewer Tests (2 tests)
| Test | Focus | Details |
|------|-------|---------|
| **TooltipTest1** | Basic Hover | Text tooltip on hover, click to dismiss or auto-hide |
| **TooltipTest2** | Positioning & Offset | Tooltips at different positions, with offset variations |

#### Viewer Test Details

**TooltipTest1.razor - Basic Hover**
```
- Button with tooltip "Click me for action"
- Hover to show tooltip
- Tooltip auto-hides on mouse leave
- Show tooltip text is readable
```

**TooltipTest2.razor - Positioning & Offset**
```
- Button with top-center tooltip
- Button with bottom-center tooltip
- Button with left tooltip (if supported)
- Button with right tooltip (if supported)
- Different Offset values to show spacing
- All should render without overflow issues
```

---

### Popover
**API Surface:** `Anchor` (trigger), `ChildContent` (popover content), `Open`, `OpenChanged` (controlled), AnchorOrigin/TransformOrigin, CloseOnOutsideClick, placement

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **PopoverTest1** | Click to Open | Button trigger, click opens popover, click outside closes |
| **PopoverTest2** | Controlled State | Toggle popover open/closed via callback |
| **PopoverTest3** | Content Variations | Popover with text, with buttons, with form inputs |

#### Viewer Test Details

**PopoverTest1.razor - Click to Open**
```
- Button "Open Popover"
- Click opens popover with content
- Popover displays near button
- Click outside to close (or click button again)
- Show proper placement/positioning
```

**PopoverTest2.razor - Controlled State**
```
- Button "Toggle Popover"
- Maintains _open state
- Click updates state (OpenChanged callback)
- Show current state (Open/Closed) in UI
- Multiple buttons can open/close same popover
```

**PopoverTest3.razor - Content Variations**
```
- Popover with text content
- Popover with button inside (action trigger)
- Popover with input field
- Popover with complex layout (header, body, footer)
```

---

## Implementation Priority

**Tier 3 (Core - 5 components × 3 tests = 13 tests):**

1. Avatar (3) → Accordion (3) → Tabs (3) → Tooltip (2) → Popover (3)
2. **Total: 14 tests** (Avatar has 1 extra test compared to base plan)

---

## Test Quality Checklist

- [ ] All test components display clear labels explaining what they demonstrate
- [ ] Interactive tests include state displays (accordion open/closed, tab active, popover visible)
- [ ] Tooltips show on hover without layout shift
- [ ] Avatar fallbacks render correctly (broken image → initials)
- [ ] AvatarGroup overlap is visually distinct by size
- [ ] Tabs switch content smoothly
- [ ] Accordion smooth expand/collapse (if animation present)
- [ ] Popover positioning doesn't overflow viewport
- [ ] Badge overlay on avatar is clearly visible
- [ ] All tests compile in Tests.Shared project

---

## Expected Test Count at Tier 3 Completion

- **Tier 1:** 9 viewer tests
- **Tier 2:** 20 viewer tests
- **Tier 3:** 14 viewer tests
- **Total:** 43 viewer tests in the sidebar explorer

---

## Notes

- Avatar and Tooltip are simpler components (straightforward APIs)
- Accordion/Tabs are compound components (Trigger + Content pattern)
- Popover is foundation for Tooltip, good to understand both
- All Tier 3 components are display-focused (no heavy input logic like Slider/Radio)
