// Dialog component JavaScript interop
const dialogHandlers = new Map();

function getOverlayElement(dialogId) {
    return document.querySelector(`[data-dialog-overlay="${dialogId}"]`);
}

function getContentElement(dialogId) {
    return document.querySelector(`[data-dialog-content="${dialogId}"]`);
}

export function initializeDialog(dialogId, dotNetRef) {
    // Store handler with document-level listeners so it survives Blazor re-renders
    const handler = {
        dialogId,
        dotNetRef,
        handleEscape: (e) => {
            if (e.key !== 'Escape') return;
            const contentElement = getContentElement(dialogId);
            if (contentElement && contentElement.getAttribute('data-state') === 'open') {
                dotNetRef.invokeMethodAsync('HandleEscape');
            }
        },
        handleOverlayClick: (e) => {
            const overlayElement = getOverlayElement(dialogId);
            const contentElement = getContentElement(dialogId);
            if (!overlayElement || !contentElement) return;

            if (e.target === overlayElement && contentElement.getAttribute('data-state') === 'open') {
                dotNetRef.invokeMethodAsync('HandleOverlayClick');
            }
        },
        trapFocus: (e) => {
            if (e.key !== 'Tab') return;

            const contentElement = getContentElement(dialogId);
            if (!contentElement || contentElement.getAttribute('data-state') !== 'open') {
                return;
            }

            const focusableElements = contentElement.querySelectorAll(
                'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
            );
            if (focusableElements.length === 0) return;

            const firstElement = focusableElements[0];
            const lastElement = focusableElements[focusableElements.length - 1];

            if (e.shiftKey) {
                if (document.activeElement === firstElement) {
                    e.preventDefault();
                    lastElement?.focus();
                }
            } else {
                if (document.activeElement === lastElement) {
                    e.preventDefault();
                    firstElement?.focus();
                }
            }
        }
    };

    // Add document-level listeners once per dialogId
    document.addEventListener('keydown', handler.handleEscape);
    document.addEventListener('click', handler.handleOverlayClick, true);
    document.addEventListener('keydown', handler.trapFocus);

    dialogHandlers.set(dialogId, handler);
}

export function openDialog(dialogId) {
    const handler = dialogHandlers.get(dialogId);
    if (!handler) return;

    const overlayElement = getOverlayElement(dialogId);
    const contentElement = getContentElement(dialogId);
    if (!overlayElement || !contentElement) return;

    // Apply base styles for JS-driven animation each time in case DOM was re-rendered
    overlayElement.style.opacity = '0';
    overlayElement.style.backdropFilter = 'blur(0px) saturate(120%)';
    overlayElement.style.transition = 'opacity 250ms ease-in-out, backdrop-filter 250ms ease-in-out';
    overlayElement.style.pointerEvents = 'none';

    contentElement.style.opacity = '0';
    contentElement.style.transform = 'translate(-50%, -50%) scale(0.95)';
    contentElement.style.transition = 'opacity 250ms ease-in-out, transform 250ms ease-in-out';
    
    // Set state
    overlayElement.setAttribute('data-state', 'open');
    contentElement.setAttribute('data-state', 'open');

    // Enable interactions
    overlayElement.style.pointerEvents = 'auto';

    // Animate overlay frost (fade + blur in)
    // Use double RAF to ensure the browser applies initial styles before transition
    requestAnimationFrame(() => {
        requestAnimationFrame(() => {
            overlayElement.style.opacity = '1';
            overlayElement.style.backdropFilter = 'blur(16px) saturate(150%)';
        });
    });

    // Animate content (fade + subtle pop)
    requestAnimationFrame(() => {
        requestAnimationFrame(() => {
            contentElement.style.opacity = '1';
            contentElement.style.transform = 'translate(-50%, -50%) scale(1)';
        });
    });
    // Focus first element
    setTimeout(() => {
        const firstFocusable = contentElement.querySelector(
            'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
        );
        firstFocusable?.focus();
    }, 100);
}

export function closeDialog(dialogId) {
    const handler = dialogHandlers.get(dialogId);
    if (!handler) return;

    const overlayElement = getOverlayElement(dialogId);
    const contentElement = getContentElement(dialogId);
    if (!overlayElement || !contentElement) return;
    
    // Set state
    overlayElement.setAttribute('data-state', 'closed');
    contentElement.setAttribute('data-state', 'closed');

    // Ensure transitions are set before animating
    overlayElement.style.transition = 'opacity 250ms ease-in-out, backdrop-filter 250ms ease-in-out';
    contentElement.style.transition = 'opacity 250ms ease-in-out, transform 250ms ease-in-out';
    
    // Disable pointer events immediately
    overlayElement.style.pointerEvents = 'none';
    
    // Use double RAF to ensure transitions are applied before changing values
    requestAnimationFrame(() => {
        requestAnimationFrame(() => {
            // Animate overlay fade out and blur removal
            overlayElement.style.opacity = '0';
            overlayElement.style.backdropFilter = 'blur(0px) saturate(120%)';
            
            // Animate content fade out and scale down
            contentElement.style.opacity = '0';
            contentElement.style.transform = 'translate(-50%, -50%) scale(0.95)';
        });
    });
}

export function disposeDialog(dialogId) {
    const handler = dialogHandlers.get(dialogId);
    if (!handler) return;

    // Remove event listeners
    document.removeEventListener('keydown', handler.handleEscape);
    document.removeEventListener('click', handler.handleOverlayClick, true);
    document.removeEventListener('keydown', handler.trapFocus);

    // No scroll handling here – scroll locking is managed via toggleBodyScroll

    dialogHandlers.delete(dialogId);
}

export function renderDialogPortal(dialogId, html) {
    // Create or get portal container
    let portalContainer = document.getElementById('blazor-dialog-portal');
    if (!portalContainer) {
        portalContainer = document.createElement('div');
        portalContainer.id = 'blazor-dialog-portal';
        document.body.appendChild(portalContainer);
    }

    // Render dialog content
    portalContainer.innerHTML = html;
    
    // Re-initialize after rendering
    setTimeout(() => {
        const dotNetRef = window[`dialog_${dialogId}_ref`];
        if (dotNetRef) {
            initializeDialog(dialogId, dotNetRef);
        }
    }, 10);
}
