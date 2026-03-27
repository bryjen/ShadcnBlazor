/**
 * Sheet component JavaScript interop.
 * Provides overlay animation, ESC handling, and overlay click.
 * Focus trapping is handled by the FocusTrap component.
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

function applySheetMotionProfile(contentElement, action) {
    const side = (contentElement.getAttribute('data-side') || '').toLowerCase();
    const isHorizontal = side === 'left' || side === 'right';

    // Right-anchored sheets tend to need a bit more duration to feel equally eased.
    const durationMs = side === 'right'
        ? 300
        : isHorizontal
            ? 260
            : 240;

    const timing = action === 'open'
        ? 'cubic-bezier(0.16, 1, 0.3, 1)'
        : 'cubic-bezier(0.4, 0, 1, 1)';

    contentElement.style.transitionDuration = `${durationMs}ms`;
    contentElement.style.transitionTimingFunction = timing;
}

export function initialize(sheetId, dotNetRef) {
    if (sheetHandlers.has(sheetId)) return;

    const handler = {
        sheetId,
        dotNetRef,
        openRaf1: null,
        openRaf2: null,
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
        }
    };

    document.addEventListener('keydown', handler.handleEscape);
    document.addEventListener('click', handler.handleOverlayClick, true);

    sheetHandlers.set(sheetId, handler);
}

export function open(sheetId) {
    const handler = sheetHandlers.get(sheetId);
    if (!handler) return;

    const overlayElement = getOverlayElement(sheetId);
    const contentElement = getContentElement(sheetId);
    if (!overlayElement || !contentElement) return;

    applySheetMotionProfile(contentElement, 'open');

    if (handler.openRaf1) cancelAnimationFrame(handler.openRaf1);
    if (handler.openRaf2) cancelAnimationFrame(handler.openRaf2);

    // Wait for at least one painted "closed" frame so easing is consistently visible.
    handler.openRaf1 = requestAnimationFrame(() => {
        handler.openRaf2 = requestAnimationFrame(() => {
            overlayElement.setAttribute('data-state', 'open');
            contentElement.setAttribute('data-state', 'open');
            handler.openRaf1 = null;
            handler.openRaf2 = null;
        });
    });
}

export function close(sheetId) {
    const handler = sheetHandlers.get(sheetId);
    if (!handler) return;

    const overlayElement = getOverlayElement(sheetId);
    const contentElement = getContentElement(sheetId);
    if (!overlayElement || !contentElement) return;

    applySheetMotionProfile(contentElement, 'close');

    if (handler.openRaf1) cancelAnimationFrame(handler.openRaf1);
    if (handler.openRaf2) cancelAnimationFrame(handler.openRaf2);
    handler.openRaf1 = null;
    handler.openRaf2 = null;

    overlayElement.setAttribute('data-state', 'closed');
    contentElement.setAttribute('data-state', 'closed');
}

export function dispose(sheetId) {
    const handler = sheetHandlers.get(sheetId);
    if (!handler) return;

    if (handler.openRaf1) cancelAnimationFrame(handler.openRaf1);
    if (handler.openRaf2) cancelAnimationFrame(handler.openRaf2);

    document.removeEventListener('keydown', handler.handleEscape);
    document.removeEventListener('click', handler.handleOverlayClick, true);

    sheetHandlers.delete(sheetId);
}
