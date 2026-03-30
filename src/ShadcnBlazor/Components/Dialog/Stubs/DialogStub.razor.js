/**
 * Dialog component JavaScript interop.
 * Handles focus management and event listeners.
 * Animation state is managed via CSS data attributes.
 */

const dialogHandlers = new Map();

function getContentElement(dialogId) {
    return document.querySelector(`[data-dialog-content="${dialogId}"]`);
}

export function initialize(dialogId, dotNetRef) {
    if (dialogHandlers.has(dialogId)) return;

    const handler = {
        dialogId,
        dotNetRef,
        handleEscape: (e) => {
            if (e.key !== 'Escape') return;
            const contentElement = getContentElement(dialogId);
            if (contentElement && contentElement.getAttribute('data-state') === 'open') {
                dotNetRef.invokeMethodAsync('HandleEscape');
            }
        }
    };

    document.addEventListener('keydown', handler.handleEscape);
    dialogHandlers.set(dialogId, handler);
}

export function focusFirstInDialog(dialogId) {
    const contentElement = getContentElement(dialogId);
    if (!contentElement) return;

    const focusableElements = contentElement.querySelectorAll(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );
    if (focusableElements.length > 0) {
        focusableElements[0].focus();
    }
}

export function dispose(dialogId) {
    const handler = dialogHandlers.get(dialogId);
    if (!handler) return;

    document.removeEventListener('keydown', handler.handleEscape);
    dialogHandlers.delete(dialogId);
}
