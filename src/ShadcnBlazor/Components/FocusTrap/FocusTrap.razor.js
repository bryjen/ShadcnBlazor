import { createFocusTrap } from 'https://cdn.jsdelivr.net/npm/focus-trap@7.6.4/+esm';

/**
 * Creates a focus trap for the specified container.
 * @param {HTMLElement} container The element to trap focus within.
 * @param {object} options Configuration options for the trap.
 * @returns {object} The focus trap instance.
 */
export function createTrap(container, options) {
    if (!container) return null;

    const trapOptions = {
        ...options,
        // Ensure selectors are converted if needed, though focus-trap handles strings.
        initialFocus: options.initialFocus || undefined,
        fallbackFocus: options.fallbackFocus || undefined,
    };

    return createFocusTrap(container, trapOptions);
}