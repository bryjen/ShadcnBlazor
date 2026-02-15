/**
 * Dialog component JavaScript interop.
 * Provides frosted backdrop animation, ESC handling, overlay click, and focus trap.
 */
(function () {
    const dialogHandlers = new Map();

    function getOverlayElement(dialogId) {
        return document.querySelector(`[data-dialog-overlay="${dialogId}"]`);
    }

    function getContentElement(dialogId) {
        return document.querySelector(`[data-dialog-content="${dialogId}"]`);
    }

    function initialize(dialogId, dotNetRef) {
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
            },
            trapFocus: (e) => {
                if (e.key !== 'Tab') return;
                const contentElement = getContentElement(dialogId);
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

        dialogHandlers.set(dialogId, handler);
    }

    function open(dialogId) {
        const handler = dialogHandlers.get(dialogId);
        if (!handler) return;

        const overlayElement = getOverlayElement(dialogId);
        const contentElement = getContentElement(dialogId);
        if (!overlayElement || !contentElement) return;

        overlayElement.setAttribute('data-state', 'open');
        contentElement.setAttribute('data-state', 'open');

        focusFirstInDialog(dialogId);
    }

    function focusFirstInDialog(dialogId) {
        const contentElement = getContentElement(dialogId);
        if (!contentElement) return;
        setTimeout(() => {
            const firstFocusable = contentElement.querySelector(
                'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
            );
            firstFocusable?.focus();
        }, 50);
    }

    function close(dialogId) {
        const handler = dialogHandlers.get(dialogId);
        if (!handler) return;

        const overlayElement = getOverlayElement(dialogId);
        const contentElement = getContentElement(dialogId);
        if (!overlayElement || !contentElement) return;

        overlayElement.setAttribute('data-state', 'closed');
        contentElement.setAttribute('data-state', 'closed');
    }

    function dispose(dialogId) {
        const handler = dialogHandlers.get(dialogId);
        if (!handler) return;

        document.removeEventListener('keydown', handler.handleEscape);
        document.removeEventListener('click', handler.handleOverlayClick, true);
        document.removeEventListener('keydown', handler.trapFocus);

        dialogHandlers.delete(dialogId);
    }

    window.ShadcnBlazor = window.ShadcnBlazor || {};
    window.ShadcnBlazor.Dialog = window.ShadcnBlazor.Dialog || {};
    window.ShadcnBlazor.Dialog.initialize = initialize;
    window.ShadcnBlazor.Dialog.open = open;
    window.ShadcnBlazor.Dialog.close = close;
    window.ShadcnBlazor.Dialog.dispose = dispose;
    window.ShadcnBlazor.Dialog.focusFirstInDialog = focusFirstInDialog;
})();
