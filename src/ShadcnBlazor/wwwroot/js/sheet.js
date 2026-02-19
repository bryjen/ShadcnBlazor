/**
 * Sheet component JavaScript interop.
 * Provides overlay animation, ESC handling, overlay click, and focus trap.
 * Uses data-sheet-* attributes to avoid conflicts with Dialog.
 * Exported as ES module for Blazor JS interop.
 */

const sheetHandlers = new Map();

function getOverlayElement(sheetId) {
    return document.querySelector(`[data-sheet-overlay="${sheetId}"]`);
}

function getContentElement(sheetId) {
    return document.querySelector(`[data-sheet-content="${sheetId}"]`);
}

export function initialize(sheetId, dotNetRef) {
    if (sheetHandlers.has(sheetId)) return;

    const handler = {
        sheetId,
        dotNetRef,
        handleEscape: (e) => {
            if (e.key !== 'Escape') return;
            const contentElement = getContentElement(sheetId);
            if (contentElement && contentElement.getAttribute('data-state') === 'open') {
                dotNetRef.invokeMethodAsync('HandleEscape');
            }
        },
        handleOverlayClick: (e) => {
            const overlayElement = getOverlayElement(sheetId);
            const contentElement = getContentElement(sheetId);
            if (!overlayElement || !contentElement) return;
            if (e.target === overlayElement && contentElement.getAttribute('data-state') === 'open') {
                dotNetRef.invokeMethodAsync('HandleOverlayClick');
            }
        },
        trapFocus: (e) => {
            if (e.key !== 'Tab') return;
            const contentElement = getContentElement(sheetId);
            if (!contentElement || contentElement.getAttribute('data-state') !== 'open') return;

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

    document.addEventListener('keydown', handler.handleEscape);
    document.addEventListener('click', handler.handleOverlayClick, true);
    document.addEventListener('keydown', handler.trapFocus);

    sheetHandlers.set(sheetId, handler);
}

export function open(sheetId) {
    const handler = sheetHandlers.get(sheetId);
    if (!handler) return;

    const overlayElement = getOverlayElement(sheetId);
    const contentElement = getContentElement(sheetId);
    if (!overlayElement || !contentElement) return;

    overlayElement.setAttribute('data-state', 'open');
    contentElement.setAttribute('data-state', 'open');

    focusFirstInSheet(sheetId);
}

export function focusFirstInSheet(sheetId) {
    const contentElement = getContentElement(sheetId);
    if (!contentElement) return;
    setTimeout(() => {
        const firstFocusable = contentElement.querySelector(
            'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
        );
        firstFocusable?.focus();
    }, 50);
}

export function close(sheetId) {
    const handler = sheetHandlers.get(sheetId);
    if (!handler) return;

    const overlayElement = getOverlayElement(sheetId);
    const contentElement = getContentElement(sheetId);
    if (!overlayElement || !contentElement) return;

    overlayElement.setAttribute('data-state', 'closed');
    contentElement.setAttribute('data-state', 'closed');
}

export function dispose(sheetId) {
    const handler = sheetHandlers.get(sheetId);
    if (!handler) return;

    document.removeEventListener('keydown', handler.handleEscape);
    document.removeEventListener('click', handler.handleOverlayClick, true);
    document.removeEventListener('keydown', handler.trapFocus);

    sheetHandlers.delete(sheetId);
}
