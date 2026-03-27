/**
 * Dialog component JavaScript interop.
 * Provides frosted backdrop animation, ESC handling, and overlay click.
 * Focus trapping is handled by the FocusTrap component.
 * Exported as ES module for Blazor JS interop.
 */

const dialogHandlers = new Map();

function getOverlayElement(dialogId) {
    return document.querySelector(`[data-dialog-overlay="${dialogId}"]`);
}

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
        },
        handleOverlayClick: (e) => {
            const overlayElement = getOverlayElement(dialogId);
            const contentElement = getContentElement(dialogId);
            if (!overlayElement || !contentElement) return;
            if (e.target === overlayElement && contentElement.getAttribute('data-state') === 'open') {
                dotNetRef.invokeMethodAsync('HandleOverlayClick');
            }
        }
    };

    document.addEventListener('keydown', handler.handleEscape);
    document.addEventListener('click', handler.handleOverlayClick, true);

    dialogHandlers.set(dialogId, handler);
}

export function open(dialogId) {
    const handler = dialogHandlers.get(dialogId);
    if (!handler) return;

    const overlayElement = getOverlayElement(dialogId);
    const contentElement = getContentElement(dialogId);
    if (!overlayElement || !contentElement) return;

    overlayElement.setAttribute('data-state', 'open');
    contentElement.setAttribute('data-state', 'open');
}

export function close(dialogId) {
    const handler = dialogHandlers.get(dialogId);
    if (!handler) return;

    const overlayElement = getOverlayElement(dialogId);
    const contentElement = getContentElement(dialogId);
    if (!overlayElement || !contentElement) return;

    overlayElement.setAttribute('data-state', 'closed');
    contentElement.setAttribute('data-state', 'closed');
}

export function dispose(dialogId) {
    const handler = dialogHandlers.get(dialogId);
    if (!handler) return;

    document.removeEventListener('keydown', handler.handleEscape);
    document.removeEventListener('click', handler.handleOverlayClick, true);

    dialogHandlers.delete(dialogId);
}
