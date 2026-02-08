// Tooltip component JavaScript interop
// Simplified and more reliable implementation following Floating UI patterns

const tooltipHandlers = new Map();
let rafId = null;

// Auto-update loop for all visible tooltips
function autoUpdate() {
    if (rafId !== null) return; // Already running
    
    function update() {
        let hasVisible = false;
        for (const [tooltipId, handler] of tooltipHandlers.entries()) {
            if (handler.isVisible && handler.triggerElement && handler.contentElement) {
                const side = handler.contentElement.getAttribute('data-side') || 'top';
                const sideOffset = parseInt(handler.contentElement.getAttribute('data-side-offset') || '0', 10);
                positionTooltip(tooltipId, side, sideOffset);
                hasVisible = true;
            }
        }
        
        if (hasVisible) {
            rafId = requestAnimationFrame(update);
        } else {
            rafId = null;
        }
    }
    
    rafId = requestAnimationFrame(update);
}

export function markTooltipTrigger(tooltipId) {
    const tooltipContainer = document.querySelector(`[data-tooltip-id="${tooltipId}"]`);
    if (!tooltipContainer) return;
    
    const contentElement = document.querySelector(`[data-tooltip-content="${tooltipId}"]`);
    if (!contentElement) return;
    
    // If the trigger was already annotated (e.g. TooltipTrigger added it), skip scanning
    const existingTrigger = tooltipContainer.querySelector(`[data-tooltip-trigger="${tooltipId}"]`);
    if (existingTrigger) {
        return;
    }

    // Find the first interactive element that's not the content
    const allElements = tooltipContainer.querySelectorAll('button, a, input, select, textarea, [role="button"], [tabindex]');
    
    for (const element of allElements) {
        if (element === contentElement || element.closest('[data-tooltip-content]') === contentElement) {
            continue;
        }
        if (element.hasAttribute('data-tooltip-trigger')) {
            continue;
        }
        
        element.setAttribute('data-tooltip-trigger', tooltipId);
        break;
    }
}

export function initializeTooltip(tooltipId, dotNetRef, delayDuration = 0) {
    const triggerElement = document.querySelector(`[data-tooltip-trigger="${tooltipId}"]`);
    const contentElement = document.querySelector(`[data-tooltip-content="${tooltipId}"]`);
    
    if (!triggerElement || !contentElement) {
        return;
    }

    let showTimeout = null;
    let hideTimeout = null;
    let isVisible = false;
    let isHoveringTrigger = false;
    let isHoveringContent = false;

    const clearTimeouts = () => {
        if (showTimeout) {
            clearTimeout(showTimeout);
            showTimeout = null;
        }
        if (hideTimeout) {
            clearTimeout(hideTimeout);
            hideTimeout = null;
        }
    };

    const show = () => {
        clearTimeouts();
        
        if (isVisible) return;
        
        showTimeout = setTimeout(() => {
            if (!isVisible && (isHoveringTrigger || isHoveringContent)) {
                isVisible = true;
                const side = contentElement.getAttribute('data-side') || 'top';
                const sideOffset = parseInt(contentElement.getAttribute('data-side-offset') || '0', 10);
                
                // Position first (while hidden)
                positionTooltip(tooltipId, side, sideOffset);
                
                // Make visible with fade-in
                contentElement.setAttribute('data-state', 'open');
                contentElement.style.display = 'block';
                contentElement.style.pointerEvents = 'auto';
                
                // Force reflow to ensure display change is applied
                contentElement.offsetHeight;
                
                // Fade in using JS
                contentElement.style.opacity = '0';
                requestAnimationFrame(() => {
                    contentElement.style.opacity = '1';
                });
                
                // Update handler state
                const handler = tooltipHandlers.get(tooltipId);
                if (handler) {
                    handler.isVisible = true;
                }
                
                // Start auto-update loop
                autoUpdate();
            }
        }, delayDuration);
    };

    const hide = () => {
        clearTimeouts();
        
        if (!isVisible) return;
        
        // Small delay to allow moving from trigger to content
        hideTimeout = setTimeout(() => {
            if (isVisible && !isHoveringTrigger && !isHoveringContent) {
                isVisible = false;
                contentElement.setAttribute('data-state', 'closed');
                contentElement.style.pointerEvents = 'none';
                
                // Fade out using JS
                contentElement.style.opacity = '0';
                
                // Update handler state
                const handler = tooltipHandlers.get(tooltipId);
                if (handler) {
                    handler.isVisible = false;
                }
                
                // Hide completely after fade animation (200ms matches transition)
                setTimeout(() => {
                    if (contentElement.getAttribute('data-state') === 'closed') {
                        contentElement.style.display = 'none';
                    }
                }, 200);
            }
        }, 50);
    };

    const handleTriggerMouseEnter = () => {
        isHoveringTrigger = true;
        show();
    };

    const handleTriggerMouseLeave = (e) => {
        isHoveringTrigger = false;
        // Check if moving to content
        const relatedTarget = e.relatedTarget;
        if (relatedTarget && (relatedTarget === contentElement || contentElement.contains(relatedTarget))) {
            isHoveringContent = true;
            return;
        }
        hide();
    };

    const handleContentMouseEnter = () => {
        isHoveringContent = true;
        clearTimeouts();
    };

    const handleContentMouseLeave = () => {
        isHoveringContent = false;
        hide();
    };

    const handleFocus = () => {
        isHoveringTrigger = true;
        show();
    };

    const handleBlur = () => {
        isHoveringTrigger = false;
        hide();
    };

    const handleEscape = (e) => {
        if (e.key === 'Escape' && isVisible) {
            isHoveringTrigger = false;
            isHoveringContent = false;
            isVisible = false;
            clearTimeouts();
            
            const handler = tooltipHandlers.get(tooltipId);
            if (handler) {
                handler.isVisible = false;
            }
            
            contentElement.setAttribute('data-state', 'closed');
            contentElement.style.pointerEvents = 'none';
            contentElement.style.opacity = '0';
            
            setTimeout(() => {
                if (contentElement.getAttribute('data-state') === 'closed') {
                    contentElement.style.display = 'none';
                }
            }, 200);
        }
    };

    // Add event listeners
    triggerElement.addEventListener('mouseenter', handleTriggerMouseEnter);
    triggerElement.addEventListener('mouseleave', handleTriggerMouseLeave);
    triggerElement.addEventListener('focus', handleFocus);
    triggerElement.addEventListener('blur', handleBlur);
    
    contentElement.addEventListener('mouseenter', handleContentMouseEnter);
    contentElement.addEventListener('mouseleave', handleContentMouseLeave);
    
    document.addEventListener('keydown', handleEscape);

    // Store handler with all event handlers for cleanup
    tooltipHandlers.set(tooltipId, {
        triggerElement,
        contentElement,
        dotNetRef,
        isVisible: false,
        show,
        hide,
        clearTimeouts,
        handleTriggerMouseEnter,
        handleTriggerMouseLeave,
        handleContentMouseEnter,
        handleContentMouseLeave,
        handleFocus,
        handleBlur,
        handleEscape
    });
}

export function positionTooltip(tooltipId, side = 'top', sideOffset = 0) {
    const handler = tooltipHandlers.get(tooltipId);
    if (!handler) return;

    const { triggerElement, contentElement } = handler;
    
    if (!triggerElement || !contentElement) return;
    
    const triggerRect = triggerElement.getBoundingClientRect();
    const contentRect = contentElement.getBoundingClientRect();
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;
    const padding = 8;

    let top = 0;
    let left = 0;
    let actualSide = side;

    // Calculate initial position
    switch (side) {
        case 'top':
            top = triggerRect.top - contentRect.height - sideOffset;
            left = triggerRect.left + (triggerRect.width - contentRect.width) / 2;
            break;
        case 'bottom':
            top = triggerRect.bottom + sideOffset;
            left = triggerRect.left + (triggerRect.width - contentRect.width) / 2;
            break;
        case 'left':
            left = triggerRect.left - contentRect.width - sideOffset;
            top = triggerRect.top + (triggerRect.height - contentRect.height) / 2;
            break;
        case 'right':
            left = triggerRect.right + sideOffset;
            top = triggerRect.top + (triggerRect.height - contentRect.height) / 2;
            break;
    }

    // Auto-flip if would go off-screen
    if (actualSide === 'top' && top < padding) {
        actualSide = 'bottom';
        top = triggerRect.bottom + sideOffset;
        left = triggerRect.left; // Maintain left alignment
    } else if (actualSide === 'bottom' && top + contentRect.height > viewportHeight - padding) {
        actualSide = 'top';
        top = triggerRect.top - contentRect.height - sideOffset;
        left = triggerRect.left; // Maintain left alignment
    } else if (actualSide === 'left' && left < padding) {
        actualSide = 'right';
        left = triggerRect.right + sideOffset;
        top = triggerRect.top; // Maintain top alignment (start at top)
    } else if (actualSide === 'right' && left + contentRect.width > viewportWidth - padding) {
        actualSide = 'left';
        left = triggerRect.left - contentRect.width - sideOffset;
        top = triggerRect.top; // Maintain top alignment (start at top)
    }

    // Shift to keep within viewport bounds
    if (left < padding) {
        left = padding;
    } else if (left + contentRect.width > viewportWidth - padding) {
        left = viewportWidth - contentRect.width - padding;
    }

    if (top < padding) {
        top = padding;
    } else if (top + contentRect.height > viewportHeight - padding) {
        top = viewportHeight - contentRect.height - padding;
    }

    // Apply position (position is already fixed from initial style, just update coords)
    const tooltipContainer = contentElement.closest('[data-tooltip-id]');
    const containerRect = tooltipContainer ? tooltipContainer.getBoundingClientRect() : null;
    const relativeTop = containerRect ? top - containerRect.top : top;
    const relativeLeft = containerRect ? left - containerRect.left : left;

    contentElement.style.top = `${relativeTop}px`;
    contentElement.style.left = `${relativeLeft}px`;
    contentElement.style.zIndex = '50';
    
    // Update data-side if flipped
    if (actualSide !== side) {
        contentElement.setAttribute('data-side', actualSide);
        const arrow = contentElement.querySelector('[data-side]');
        if (arrow) {
            arrow.setAttribute('data-side', actualSide);
        }
    }
}

export function disposeTooltip(tooltipId) {
    const handler = tooltipHandlers.get(tooltipId);
    if (!handler) return;

    handler.clearTimeouts();
    
    // Remove event listeners
    const { 
        triggerElement, 
        contentElement, 
        handleTriggerMouseEnter,
        handleTriggerMouseLeave,
        handleContentMouseEnter,
        handleContentMouseLeave,
        handleFocus,
        handleBlur,
        handleEscape 
    } = handler;
    
    if (triggerElement) {
        triggerElement.removeEventListener('mouseenter', handleTriggerMouseEnter);
        triggerElement.removeEventListener('mouseleave', handleTriggerMouseLeave);
        triggerElement.removeEventListener('focus', handleFocus);
        triggerElement.removeEventListener('blur', handleBlur);
    }
    
    if (contentElement) {
        contentElement.removeEventListener('mouseenter', handleContentMouseEnter);
        contentElement.removeEventListener('mouseleave', handleContentMouseLeave);
    }
    
    document.removeEventListener('keydown', handleEscape);

    tooltipHandlers.delete(tooltipId);
}
