// Dropdown click-outside detection
const clickOutsideHandlers = new Map();

export function registerClickOutside(dropdownId, dotNetRef) {
    // Remove existing handler if any
    unregisterClickOutside(dropdownId);

    const handler = (event) => {
        const dropdownElement = document.querySelector(`[data-dropdown-menu-content="${dropdownId}"]`);
        const triggerElement = document.querySelector(`[data-dropdown-menu-trigger="${dropdownId}"]`);

        if (!dropdownElement || !triggerElement) {
            unregisterClickOutside(dropdownId);
            return;
        }

        // Check if click is outside both dropdown and trigger
        if (!dropdownElement.contains(event.target) && !triggerElement.contains(event.target)) {
            dotNetRef.invokeMethodAsync('HandleClickOutside', dropdownId);
        }
    };

    // Use capture phase to catch clicks before they bubble
    document.addEventListener('click', handler, true);
    clickOutsideHandlers.set(dropdownId, handler);
}

export function unregisterClickOutside(dropdownId) {
    const handler = clickOutsideHandlers.get(dropdownId);
    if (handler) {
        document.removeEventListener('click', handler, true);
        clickOutsideHandlers.delete(dropdownId);
    }
}

// Dropdown menu handlers for positioning and animations
const dropdownHandlers = new Map();
let dropdownRafId = null;

// Auto-update loop for all visible dropdowns
function autoUpdateDropdowns() {
    if (dropdownRafId !== null) return; // Already running
    
    function update() {
        let hasVisible = false;
        for (const [dropdownId, handler] of dropdownHandlers.entries()) {
            if (handler.isVisible && handler.triggerElement && handler.contentElement) {
                const side = handler.side || 'bottom';
                const align = handler.align || 'start';
                const sideOffset = handler.sideOffset || 4;
                positionDropdownMenu(dropdownId, side, align, sideOffset);
                hasVisible = true;
            }
        }
        
        if (hasVisible) {
            dropdownRafId = requestAnimationFrame(update);
        } else {
            dropdownRafId = null;
        }
    }
    
    dropdownRafId = requestAnimationFrame(update);
}

// Dropdown menu positioning
export function positionDropdownMenu(dropdownId, side = 'bottom', align = 'start', sideOffset = 4) {
    const triggerElement = document.querySelector(`[data-dropdown-menu-trigger="${dropdownId}"]`);
    const contentElement = document.querySelector(`[data-dropdown-menu-content="${dropdownId}"]`);
    
    if (!triggerElement || !contentElement) {
        return;
    }

    const triggerRect = triggerElement.getBoundingClientRect();
    const contentRect = contentElement.getBoundingClientRect();
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;
    const padding = 8;

    let top = 0;
    let left = 0;

    // Calculate position based on side
    switch (side) {
        case 'top':
            top = triggerRect.top - contentRect.height - sideOffset;
            break;
        case 'bottom':
            top = triggerRect.bottom + sideOffset;
            break;
        case 'left':
            left = triggerRect.left - contentRect.width - sideOffset;
            top = triggerRect.top;
            break;
        case 'right':
            left = triggerRect.right + sideOffset;
            top = triggerRect.top;
            break;
        default:
            top = triggerRect.bottom + sideOffset;
    }

    // Calculate alignment
    if (side === 'top' || side === 'bottom') {
        switch (align) {
            case 'start':
                left = triggerRect.left;
                break;
            case 'center':
                left = triggerRect.left + (triggerRect.width / 2) - (contentRect.width / 2);
                break;
            case 'end':
                left = triggerRect.right - contentRect.width;
                break;
        }
    } else {
        switch (align) {
            case 'start':
                top = triggerRect.top;
                break;
            case 'center':
                top = triggerRect.top + (triggerRect.height / 2) - (contentRect.height / 2);
                break;
            case 'end':
                top = triggerRect.bottom - contentRect.height;
                break;
        }
    }

    // Keep within viewport bounds
    if (left < padding) left = padding;
    if (left + contentRect.width > viewportWidth - padding) left = viewportWidth - contentRect.width - padding;
    if (top < padding) top = padding;
    if (top + contentRect.height > viewportHeight - padding) top = viewportHeight - contentRect.height - padding;

    // Apply position (position is already fixed from initial style, just update coords)
    contentElement.style.top = `${top}px`;
    contentElement.style.left = `${left}px`;
    contentElement.style.zIndex = '50';
}

export function showDropdownMenu(dropdownId, side = 'bottom', align = 'start', sideOffset = 4) {
    const contentElement = document.querySelector(`[data-dropdown-menu-content="${dropdownId}"]`);
    if (!contentElement) return;

    // Temporarily make visible (but transparent) to measure dimensions
    contentElement.setAttribute('data-state', 'open');
    contentElement.style.display = 'block';
    contentElement.style.visibility = 'hidden';
    contentElement.style.opacity = '0';
    contentElement.style.pointerEvents = 'none';

    // Force reflow to ensure dimensions are calculated
    contentElement.offsetHeight;

    // Position with correct dimensions
    positionDropdownMenu(dropdownId, side, align, sideOffset);

    // Make visible with fade-in
    contentElement.style.visibility = 'visible';
    contentElement.style.pointerEvents = 'auto';

    // Fade in using JS
    requestAnimationFrame(() => {
        contentElement.style.opacity = '1';
    });

    // Store handler for auto-update
    const triggerElement = document.querySelector(`[data-dropdown-menu-trigger="${dropdownId}"]`);
    if (triggerElement) {
        dropdownHandlers.set(dropdownId, {
            triggerElement,
            contentElement,
            isVisible: true,
            side,
            align,
            sideOffset
        });
        autoUpdateDropdowns();
    }
}

export function hideDropdownMenu(dropdownId) {
    const contentElement = document.querySelector(`[data-dropdown-menu-content="${dropdownId}"]`);
    if (!contentElement) return;

    const handler = dropdownHandlers.get(dropdownId);
    if (handler) {
        handler.isVisible = false;
    }

    contentElement.setAttribute('data-state', 'closed');
    contentElement.style.pointerEvents = 'none';

    // Fade out using JS
    contentElement.style.opacity = '0';

    // Hide completely after fade animation (200ms matches transition)
    setTimeout(() => {
        if (contentElement.getAttribute('data-state') === 'closed') {
            contentElement.style.display = 'none';
        }
    }, 200);

    dropdownHandlers.delete(dropdownId);
}
