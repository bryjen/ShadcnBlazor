export function chooseSelectDirection(triggerElement) {
    if (!triggerElement) {
        return "down";
    }

    const rect = triggerElement.getBoundingClientRect();
    const viewportHeight = window.innerHeight || document.documentElement.clientHeight || 0;

    const spaceAbove = rect.top;
    const spaceBelow = viewportHeight - rect.bottom;

    // Heuristic: estimated dropdown height in pixels. If there isn't enough
    // room below but there is more room above, prefer opening upwards.
    const ESTIMATED_DROPDOWN_HEIGHT = 260;

    if (spaceBelow < ESTIMATED_DROPDOWN_HEIGHT && spaceAbove > spaceBelow) {
        return "up";
    }

    return "down";
}

export function focusElement(elementRef) {
    if (!elementRef) {
        return;
    }
    
    try {
        // For Blazor ElementReference, we need to access the underlying DOM element
        // This is a workaround - in practice, Blazor handles this automatically
        if (elementRef && typeof elementRef.focus === 'function') {
            elementRef.focus();
        }
    } catch (e) {
        console.warn('Could not focus element:', e);
    }
}

