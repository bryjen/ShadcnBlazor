# Tier 2 Test Plan: Switch, ToggleButton, Input, Textarea, Slider, Radio

## Component Test Coverage Matrix

### Switch
**API Surface:** `Checked`, `Size` (Sm, Md, Lg), `Disabled`, `CheckedChanged`, `ChildContent` (label)

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **SwitchTest1** | States & Binding | Default state, Checked state, two-way binding (@bind-Checked), label text |
| **SwitchTest2** | Sizes & Disabled | All sizes (Sm, Md, Lg), Disabled state, Disabled + Checked state |
| **SwitchTest3** | Interactivity | CheckedChanged callback with event log, rapid toggle testing, state persistence |

#### Viewer Test Details

**SwitchTest1.razor - States & Binding**
```
- Toggle switch in unchecked state (default)
- Toggle switch in checked state
- Two synced switches bound to same variable (like CheckboxTest2)
- Each with descriptive labels ("Off", "On", etc.)
```

**SwitchTest2.razor - Sizes & Disabled**
```
- Three sizes: Size.Sm, Size.Md (default), Size.Lg
- Disabled switch (unchecked)
- Disabled + Checked switch
- Side-by-side comparison rows
```

**SwitchTest3.razor - Interactivity**
```
- Switch with CheckedChanged callback logging timestamp + new state
- Show event log updating in real-time
- Include rapid toggle testing label
- Edge case: Verify CheckedChanged fires on enabled but NOT on disabled
```

---

### ToggleButton
**API Surface:** `Pressed`, `Size` (Sm, Md, Lg), `Variant` (Default, Secondary, Outline, Ghost, Link), `Disabled`, `PressedChanged`, `Type`, `ChildContent`

#### Viewer Tests (4 tests)
| Test | Focus | Details |
|------|-------|---------|
| **ToggleButtonTest1** | Variants & States | All 5 variants in pressed/unpressed states |
| **ToggleButtonTest2** | Sizes & Icons | All sizes with icon content, mixed icon+text |
| **ToggleButtonTest3** | Groups | Multiple toggle buttons as group (radio-like behavior) |
| **ToggleButtonTest4** | Disabled & Callback | Disabled state guard, PressedChanged event log |

#### Viewer Test Details

**ToggleButtonTest1.razor - Variants & States**
```
- Grid of 5 variants × 2 states (10 buttons total)
- Variants: Default, Secondary, Outline, Ghost, Link
- States: unpressed (default), pressed
- Each labeled with variant name + state
```

**ToggleButtonTest2.razor - Sizes & Icons**
```
- All sizes: Size.Sm, Size.Md, Size.Lg
- Icon-only buttons (SVG icon inside)
- Icon + text buttons
- Text-only buttons for comparison
- Visual size difference clear
```

**ToggleButtonTest3.razor - Groups**
```
- Standalone toggle button (can be pressed/unpressed independently)
- Group of 3 toggle buttons (mimics exclusive selection like radio)
- Demonstrate solo vs grouped behavior
```

**ToggleButtonTest4.razor - Disabled & Callback**
```
- PressedChanged callback with event log (timestamp + new pressed state)
- Disabled toggle button (cannot be pressed)
- Verify PressedChanged does NOT fire when disabled
- Counter: enabled toggles fire vs disabled toggles fire (should be 0)
```

---

### Input
**API Surface:** `Type` (text, password, email, number, url), `Value`, `Size` (Sm, Md, Lg), `Disabled`, `Placeholder`, `ValueChanged`, `Change`

#### Viewer Tests (4 tests)
| Test | Focus | Details |
|------|-------|---------|
| **InputTest1** | Types & Placeholders | All 5 input types with placeholder text |
| **InputTest2** | Sizes & States | All sizes, Disabled, ReadOnly (if supported) |
| **InputTest3** | Binding & Callbacks | Two-way binding (@bind-Value), ValueChanged event log |
| **InputTest4** | Edge Cases | Empty value, very long text, numeric validation (number type) |

#### Viewer Test Details

**InputTest1.razor - Types & Placeholders**
```
- Input Type.Text with placeholder "Enter text..."
- Input Type.Password with placeholder "Enter password..."
- Input Type.Email with placeholder "user@example.com"
- Input Type.Number with placeholder "0"
- Input Type.Url with placeholder "https://..."
```

**InputTest2.razor - Sizes & States**
```
- All sizes: Size.Sm, Size.Md, Size.Lg
- Disabled input (gray out)
- Each size labeled for clarity
```

**InputTest3.razor - Binding & Callbacks**
```
- Input with @bind-Value synced display below
- Separate input with ValueChanged callback logging (timestamp + new value)
- Show real-time updates in both cases
```

**InputTest4.razor - Edge Cases**
```
- Input Type.Text with max length (test overflow behavior)
- Input Type.Number with min/max (visual validation)
- Very long placeholder text
- Empty input (default)
```

---

### Textarea
**API Surface:** `Value`, `Rows`, `Size` (Sm, Md, Lg), `Disabled`, `Placeholder`, `Change`

#### Viewer Tests (3 tests)
| Test | Focus | Details |
|------|-------|---------|
| **TextareaTest1** | Rows & Sizes | Multiple row counts (2, 4, 8), all sizes |
| **TextareaTest2** | States & Binding | Disabled state, two-way binding (@bind-Value) |
| **TextareaTest3** | Callbacks | OnChange event log, multi-line text handling |

#### Viewer Test Details

**TextareaTest1.razor - Rows & Sizes**
```
- Textarea Rows.2, Rows.4, Rows.8 (or numeric if not enum)
- All sizes: Size.Sm, Size.Md, Size.Lg
- Grid layout showing different dimensions
```

**TextareaTest2.razor - States & Binding**
```
- Default textarea (editable)
- Disabled textarea
- Two synced textareas bound to same @bind-Value
- Show synced text display below
```

**TextareaTest3.razor - Callbacks**
```
- Textarea with OnChange callback
- Log timestamp + content length + first 50 chars
- Demonstrate handling of multi-line text
- Show word count or character count updating
```

---

### Slider
**API Surface:** `Value`, `Min`, `Max`, `Step`, `Size` (Sm, Md, Lg), `Disabled`, `ValueChanged`, `AriaLabel`

#### Viewer Tests (4 tests)
| Test | Focus | Details |
|------|-------|---------|
| **SliderTest1** | Basic Range | Min=0, Max=100, Step=1, default middle value |
| **SliderTest2** | Sizes & Steps | All sizes, different step values (1, 5, 10) |
| **SliderTest3** | Custom Ranges | Min=10, Max=1000 (large range), Min=-100, Max=100 (negative values) |
| **SliderTest4** | Interactivity | ValueChanged callback, two-way binding, disabled state |

#### Viewer Test Details

**SliderTest1.razor - Basic Range**
```
- Single slider: Min=0, Max=100, Step=1, Value=50 (default middle)
- Display: Current value below slider
- Range bar visual (colored active range)
```

**SliderTest2.razor - Sizes & Steps**
```
- Size.Sm with Step=1
- Size.Md with Step=5
- Size.Lg with Step=10
- Show that larger steps = larger jumps in value
```

**SliderTest3.razor - Custom Ranges**
```
- Slider: Min=10, Max=1000 (financial range, e.g., prices)
- Slider: Min=-100, Max=100 (temperature range with negatives)
- Slider: Min=0, Max=1, Step=0.1 (decimal range, e.g., opacity)
- Display current value with appropriate formatting
```

**SliderTest4.razor - Interactivity**
```
- Slider with ValueChanged callback (log timestamp + new value)
- Second slider with @bind-Value synced display
- Disabled slider (cannot be moved)
- Show that disabled slider is visually distinct
```

---

### RangeSlider (Tier 2 Bonus)
**API Surface:** `LowerValue`, `UpperValue`, `Min`, `Max`, `Step`, `Size`, `Disabled`, `LowerAriaLabel`, `UpperAriaLabel`

#### Viewer Tests (2 tests)
| Test | Focus | Details |
|------|-------|---------|
| **RangeSliderTest1** | Basic Range Picking | Min=0, Max=100, LowerValue=20, UpperValue=80 |
| **RangeSliderTest2** | Interactivity & Sizes | All sizes, custom ranges, disabled state, callback logging |

#### Viewer Test Details

**RangeSliderTest1.razor - Basic Range Picking**
```
- RangeSlider: Min=0, Max=100, LowerValue=20, UpperValue=80
- Display: "Selected range: 20 - 80"
- Two thumbs visible on track, highlighted active range
```

**RangeSliderTest2.razor - Interactivity & Sizes**
```
- All sizes: Size.Sm, Size.Md, Size.Lg
- Callback logging: When either thumb moves, log "Lower: X, Upper: Y"
- Custom range: Min=0, Max=1000 (price range)
- Disabled state (both thumbs locked)
```

---

### Radio
**API Surface:** `Value`, `Selected` or `Checked`, `Size` (Sm, Md, Lg), `Disabled`, `Alignment` (Top, Center, Bottom for labels), `Invalid`, `ValueChanged`, `RadioGroup`

#### Viewer Tests (4 tests)
| Test | Focus | Details |
|------|-------|---------|
| **RadioTest1** | Basic Group | 3-4 radio options, default selection, labels |
| **RadioTest2** | Sizes & Alignment | All sizes, multi-line labels with different alignments |
| **RadioTest3** | States & Disabled | Disabled radio item, disabled group, Invalid state |
| **RadioTest4** | Interactivity | ValueChanged callback logging selected value, two groups side-by-side |

#### Viewer Test Details

**RadioTest1.razor - Basic Group**
```
- RadioGroup with 3 radio options:
  - Option 1 (default selected)
  - Option 2
  - Option 3
- Each radio with label text
- Display: "Selected: Option 1"
```

**RadioTest2.razor - Sizes & Alignment**
```
- RadioGroup with Size.Sm items (short labels)
- RadioGroup with Size.Md items (multi-line labels)
- RadioGroup with Size.Lg items (multi-line labels)
- For multi-line, show alignment: Top, Center, Bottom
- Labels wrap across multiple lines to show alignment differences
```

**RadioTest3.razor - States & Disabled**
```
- RadioGroup with one item Disabled="true" (option 2 disabled)
- Try to click disabled option (nothing happens)
- Display visual distinction (grayed out)
- One radio with Invalid="true" state (show validation error styling)
```

**RadioTest4.razor - Interactivity**
```
- RadioGroup 1 (Colors: Red, Green, Blue) with ValueChanged callback
- RadioGroup 2 (Sizes: Small, Medium, Large) with ValueChanged callback
- Log selected values from both groups in real-time
- Show "Group 1: Green" + "Group 2: Large" updating as you click
```

---

## Implementation Priority

**Phase 1 (Core - 6 components × 3 tests = 18 tests):**
1. Switch (3) → Input (3) → Textarea (3) → Radio (3) → Slider (3) → ToggleButton (3)

**Phase 2 (Bonus):**
- RangeSlider (2 tests) — Added if time permits

---

## Test Quality Checklist

- [ ] All test components display clear labels explaining what they demonstrate
- [ ] Interactive tests include event logs or state displays
- [ ] Disabled states are visually distinct
- [ ] Sizes are visibly different (not just theoretical)
- [ ] Callbacks show timestamps for interactivity verification
- [ ] Multi-line content handles alignment/wrapping properly
- [ ] Edge cases (empty, extreme values, rapid clicks) are visible
- [ ] All tests compile in Tests.Shared project

---

## Expected Test Count at Tier 2 Completion

- **Tier 1:** 9 viewer tests (already done)
- **Tier 2 Phase 1:** 18 viewer tests
- **Tier 2 Phase 2:** +2 viewer tests (RangeSlider)
- **Total:** 29+ viewer tests in the sidebar explorer
