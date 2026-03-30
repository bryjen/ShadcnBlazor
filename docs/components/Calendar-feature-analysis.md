# Calendar Component: Feature Analysis & Gap Assessment

**Date:** 2026-03-26
**Comparison Base:** shadcn/ui Calendar (`calendar.tsx` + `calendar.mdx`)
**Status:** Feature gap tracking for ShadcnBlazor Calendar

---

## Architecture Comparison

### shadcn Approach
- **Wrapper library:** Thin styling layer around [`react-day-picker`](https://react-day-picker.js.org/)
- **Single component:** `<Calendar mode="single" | mode="range">` with unified API
- **Date logic:** Delegated to external headless library
- **Philosophy:** Styling + customization on top of battle-tested date logic
- **Code size:** ~220 LOC (TypeScript + TSX)

### Our Approach
- **Custom implementation:** Fully custom date logic, no external date library
- **Multiple components:** `DatePicker`, `DateRangePicker`, `CalendarView`, `MonthPicker`
- **Date logic:** Implemented in `CalendarView.razor.cs`
- **Philosophy:** Full control + tight Blazor integration
- **Code size:** Distributed across multiple files (more verbose)

---

## Feature Inventory

### ✅ Implemented
- Single date selection (`DatePicker` + `CalendarView`)
- Range selection (`DateRangePicker` + `CalendarView`)
- Month/year navigation (separate `MonthPicker` component)
- Disabled dates constraint
- Show/hide outside days
- Min/max date constraints
- Size variants (`DatePickerSize`: Sm, Md, Lg)
- Clearable flag
- Popover integration

### ❌ Not Implemented (Gaps)

| Feature | shadcn | Status | Priority | Notes |
|---------|--------|--------|----------|-------|
| **Locale/i18n support** | ✅ Multiple locales via `locale` prop | ❌ | Medium | Hardcoded English month labels. Need `locale` parameter + month label map |
| **RTL support** | ✅ Via CSS logical properties + `dir` prop | ❌ | Low | CSS uses physical properties (left/right). Need logical properties (start/end) |
| **Calendar system variants** | ✅ Persian/Hijri/Jalali built-in | ❌ | Low | No support for alternative calendar systems |
| **Timezone handling** | ✅ `timeZone` prop + Intl API | ❌ | Medium | `DateOnly` is timezone-naive. Need explicit timezone handling |
| **Quick presets** | ✅ Common ranges (Last 7 days, etc.) | ❌ | High | UI pattern for preset selections (needs example component) |
| **Week numbers** | ✅ `showWeekNumber` prop | ❌ | Low | Calendar doesn't render ISO week numbers |
| **Custom cell sizing** | ✅ `--cell-size` CSS variable | ❌ | Low | Fixed sizing, no responsive cell size control |
| **Time picker integration** | ✅ Date + time example | ❌ | Low | Currently dates-only, no time selection |
| **Month/year caption layouts** | ✅ `captionLayout="dropdown"` or `"label"` | ⚠️ Partial | Medium | We have separate `MonthPicker` (more composable) but no unified caption control |
| **Booked/unavailable dates styling** | ✅ Via modifiers pattern | ✅ Partial | Low | Disabled dates work; custom modifiers not exposed |
| **Custom classNames config** | ✅ Full classNames object override | ❌ | Low | We use `CalendarStyles.Build*()` pattern instead |

---

## Implementation Priority Tiers

### Tier 1: High Value (Consider Adding)
1. **Presets UI pattern** — Common date ranges (Last 7 days, This month, etc.)
   - High user-facing value
   - Relatively straightforward to add as companion component
   - Example: `<CalendarPresets>` with buttons for quick selection

2. **Locale/i18n** — At minimum, allow custom month labels
   - Medium effort
   - Current hardcoded English is limiting
   - Start with month names parameter

### Tier 2: Medium Value (Nice to Have)
1. **Timezone support** — Explicit timezone handling
   - Medium effort (need to wrap `DateOnly` with timezone context)
   - Reduces user confusion around date selection

2. **Month/year caption layout options** — `"dropdown"` vs `"label"` modes
   - Low effort (already have `MonthPicker`)
   - Better unified API with shadcn

### Tier 3: Low Value (Lower Priority)
1. **Week numbers** — Display ISO week numbers
   - Low effort, niche use case
   - Add `ShowWeekNumbers` parameter

2. **Custom cell sizing** — CSS variable for responsive cells
   - Low effort, nice for edge cases
   - Could add `--cell-size` support to container

3. **RTL support** — Logical CSS properties + `Dir` prop
   - Medium effort
   - Niche (English-first UI)
   - Defer unless RTL audience is priority

4. **Alternative calendars** — Persian/Hijri/Jalali systems
   - High effort, very niche
   - Unlikely to implement unless specifically requested

5. **Time picker integration** — Date + time selection
   - High effort, separate concern
   - Should be separate component or example

---

## Decision Log

### Decisions Made
- ✅ Custom implementation over `react-day-picker` wrapper (Blazor-first approach)
- ✅ Multiple picker components over single mode-based component (more composable)
- ✅ Separate `MonthPicker` component (better Blazor integration)
- ✅ `CalendarStyles.Build*()` pattern over classNames config object (simpler for C#)

### Deferred (Not Implementing Soon)
- ❌ RTL support — low priority, defer unless explicitly requested
- ❌ Alternative calendar systems — very niche, high effort
- ❌ Time picker — should be separate concern
- ❌ Custom cell sizing CSS variables — premature optimization

### TBD (Revisit Later)
- ⚠️ Locale/i18n — medium priority, revisit if user base expands
- ⚠️ Presets — medium priority, revisit for UX iteration
- ⚠️ Timezone handling — revisit based on user feedback

---

## Known Limitations & Workarounds

| Limitation | Impact | Workaround |
|-----------|--------|-----------|
| Hardcoded English months | Breaks non-English apps | Build custom month label array, pass to component |
| No timezone awareness | Date offset issues across zones | Document that `DateOnly` is always UTC-agnostic |
| No week numbers | ISO week display missing | Can add manually in consuming component |
| Fixed cell size | Poor responsiveness on mobile | Use container queries in consuming component |
| No preset buttons | Users must manually select dates | Build custom preset component wrapper |

---

## Comparison Matrix

| Aspect | shadcn | Ours | Winner |
|--------|--------|------|--------|
| **Out-of-box features** | 11+ | 8 | shadcn |
| **Customization** | Via classNames object | Via Blazor parameters | Tie (different paradigm) |
| **No external deps** | ❌ (react-day-picker) | ✅ | Ours |
| **Component composability** | Single `Calendar` | 4 separate components | Ours (more granular) |
| **Localization** | ✅ Full support | ❌ None | shadcn |
| **Time picker** | ✅ Example provided | ❌ | shadcn |
| **Accessibility** | ✅ (via library) | ✅ (manual) | Tie |

---

## Next Steps (If Needed)

1. **Monitor user feedback** — Track requests for missing features
2. **Prioritize presets** — Most commonly requested feature
3. **Add locale support** — Start with month names parameter only
4. **Document workarounds** — Explain how to extend for common use cases
5. **Consider `ShowWeekNumbers`** — Low effort, consider adding next iteration

---

## References

- **shadcn Calendar docs:** `context/ui/apps/v4/content/docs/components/base/calendar.mdx`
- **shadcn Calendar source:** `context/ui/apps/v4/registry/new-york-v4/ui/calendar.tsx`
- **react-day-picker:** https://react-day-picker.js.org
- **Our implementation:** `src/ShadcnBlazor/Components/Calendar/`
