// Simple helper to enable/disable page scrolling by toggling body/html overflow,
// while compensating for the scrollbar so layout doesn't shift.
// Exported as ES module for Blazor JS interop.

function setBodyScrollDisabled(disabled) {
    if (typeof document === "undefined") return;

    var body = document.body;
    var root = document.documentElement;
    if (!body || !root) return;

    if (disabled) {
        // Preserve current overflow and padding so we can restore them later.
        if (body.dataset.prevOverflow === undefined) {
            body.dataset.prevOverflow = body.style.overflow || "";
        }
        if (body.dataset.prevRootOverflow === undefined) {
            body.dataset.prevRootOverflow = root.style.overflow || "";
        }
        if (body.dataset.prevPaddingRight === undefined) {
            body.dataset.prevPaddingRight = body.style.paddingRight || "";
        }

        body.style.overflow = "hidden";
        root.style.overflow = "hidden";
    } else {
        if (body.dataset.prevOverflow !== undefined) {
            body.style.overflow = body.dataset.prevOverflow;
            delete body.dataset.prevOverflow;
        } else {
            body.style.overflow = "";
        }

        if (body.dataset.prevRootOverflow !== undefined) {
            root.style.overflow = body.dataset.prevRootOverflow;
            delete body.dataset.prevRootOverflow;
        } else {
            root.style.overflow = "";
        }

        if (body.dataset.prevPaddingRight !== undefined) {
            body.style.paddingRight = body.dataset.prevPaddingRight;
            delete body.dataset.prevPaddingRight;
        } else {
            body.style.paddingRight = "";
        }

        // Remove the gutter overlay if it exists.
        var gutter = document.getElementById("scroll-lock-gutter");
        if (gutter && gutter.parentNode) {
            gutter.parentNode.removeChild(gutter);
        }
    }
}

/** Locks body scroll to prevent background scrolling (e.g. when a modal is open). */
export function lockScroll() {
    setBodyScrollDisabled(true);
}

/** Unlocks body scroll. */
export function unlockScroll() {
    setBodyScrollDisabled(false);
}