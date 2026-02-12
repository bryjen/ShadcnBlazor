/**
 * Simplified popover positioning system
 * Features: basic positioning, auto-flip, width modes, list clamping, debouncing
 * Future: animations, nested popovers
 */
window.popoverHelper = {
    containerClass: 'popover-provider',
    overflowPadding: 10,
    flipMargin: 0,
    baseZIndex: 1200,
    debounce: function (func, wait) {
        let timeout;
        return function executedFunction(...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func(...args), wait);
        };
    },

    isFiniteNumber: function (value) {
        return Number.isFinite(value);
    },

    isValidRect: function (rect, requirePositiveSize) {
        if (!this.isFiniteNumber(rect.top) || !this.isFiniteNumber(rect.left) || !this.isFiniteNumber(rect.width) || !this.isFiniteNumber(rect.height)) {
            return false;
        }

        if (requirePositiveSize && (rect.width <= 0 || rect.height <= 0)) {
            return false;
        }

        return true;
    },

    retryPlacement: function (popover, anchorId, popoverId) {
        const retries = parseInt(popover.getAttribute('data-popover-retry') ?? '0', 10);
        if (retries >= 3) {
            return;
        }

        popover.setAttribute('data-popover-retry', (retries + 1).toString());
        requestAnimationFrame(() => this.placePopover(anchorId, popoverId));
    },

    // Position calculations based on anchor/transform origins
    calculatePosition: function (classList, anchorRect, popoverRect) {
        let top = anchorRect.top;
        let left = anchorRect.left;
        let offsetX = 0;
        let offsetY = 0;

        // Transform origin (where on popover aligns with anchor)
        if (classList.contains('popover-top-left')) {
            offsetX = 0; offsetY = 0;
        } else if (classList.contains('popover-top-center')) {
            offsetX = -popoverRect.width / 2; offsetY = 0;
        } else if (classList.contains('popover-top-right')) {
            offsetX = -popoverRect.width; offsetY = 0;
        } else if (classList.contains('popover-center-left')) {
            offsetX = 0; offsetY = -popoverRect.height / 2;
        } else if (classList.contains('popover-center-center')) {
            offsetX = -popoverRect.width / 2; offsetY = -popoverRect.height / 2;
        } else if (classList.contains('popover-center-right')) {
            offsetX = -popoverRect.width; offsetY = -popoverRect.height / 2;
        } else if (classList.contains('popover-bottom-left')) {
            offsetX = 0; offsetY = -popoverRect.height;
        } else if (classList.contains('popover-bottom-center')) {
            offsetX = -popoverRect.width / 2; offsetY = -popoverRect.height;
        } else if (classList.contains('popover-bottom-right')) {
            offsetX = -popoverRect.width; offsetY = -popoverRect.height;
        }

        // Anchor origin (where on parent to attach)
        if (classList.contains('popover-anchor-top-left')) {
            left = anchorRect.left; top = anchorRect.top;
        } else if (classList.contains('popover-anchor-top-center')) {
            left = anchorRect.left + anchorRect.width / 2; top = anchorRect.top;
        } else if (classList.contains('popover-anchor-top-right')) {
            left = anchorRect.right; top = anchorRect.top;
        } else if (classList.contains('popover-anchor-center-left')) {
            left = anchorRect.left; top = anchorRect.top + anchorRect.height / 2;
        } else if (classList.contains('popover-anchor-center-center')) {
            left = anchorRect.left + anchorRect.width / 2; top = anchorRect.top + anchorRect.height / 2;
        } else if (classList.contains('popover-anchor-center-right')) {
            left = anchorRect.right; top = anchorRect.top + anchorRect.height / 2;
        } else if (classList.contains('popover-anchor-bottom-left')) {
            left = anchorRect.left; top = anchorRect.bottom;
        } else if (classList.contains('popover-anchor-bottom-center')) {
            left = anchorRect.left + anchorRect.width / 2; top = anchorRect.bottom;
        } else if (classList.contains('popover-anchor-bottom-right')) {
            left = anchorRect.right; top = anchorRect.bottom;
        }

        return { top, left, offsetX, offsetY, anchorX: left, anchorY: top };
    },

    // Auto-flip logic when popover would go off-screen
    shouldFlip: function (classList, anchorX, anchorY, popoverWidth, popoverHeight) {
        const margin = this.flipMargin;
        let flipVertical = false;
        let flipHorizontal = false;

        // Top-aligned popovers
        if (classList.contains('popover-top-left') ||
            classList.contains('popover-top-center') ||
            classList.contains('popover-top-right')) {
            const spaceBelow = window.innerHeight - anchorY - margin;
            const spaceAbove = anchorY - margin;
            if (popoverHeight > spaceBelow && spaceAbove > spaceBelow) {
                flipVertical = true;
            }
        }

        // Bottom-aligned popovers
        if (classList.contains('popover-bottom-left') ||
            classList.contains('popover-bottom-center') ||
            classList.contains('popover-bottom-right')) {
            const spaceAbove = anchorY - margin;
            const spaceBelow = window.innerHeight - anchorY - margin;
            if (popoverHeight > spaceAbove && spaceBelow > spaceAbove) {
                flipVertical = true;
            }
        }

        // Left-aligned popovers
        if (classList.contains('popover-top-left') ||
            classList.contains('popover-center-left') ||
            classList.contains('popover-bottom-left')) {
            const spaceRight = window.innerWidth - anchorX - margin;
            const spaceLeft = anchorX - margin;
            if (popoverWidth > spaceRight && spaceLeft > spaceRight) {
                flipHorizontal = true;
            }
        }

        // Right-aligned popovers
        if (classList.contains('popover-top-right') ||
            classList.contains('popover-center-right') ||
            classList.contains('popover-bottom-right')) {
            const spaceLeft = anchorX - margin;
            const spaceRight = window.innerWidth - anchorX - margin;
            if (popoverWidth > spaceLeft && spaceRight > spaceLeft) {
                flipHorizontal = true;
            }
        }

        return { flipVertical, flipHorizontal };
    },

    // Apply flip by swapping classes
    applyFlip: function (classList, flipVertical, flipHorizontal) {
        const classArray = Array.from(classList);

        if (flipVertical) {
            classArray.forEach((cls, i) => {
                if (cls.includes('top-')) classArray[i] = cls.replace('top-', 'bottom-');
                else if (cls.includes('bottom-')) classArray[i] = cls.replace('bottom-', 'top-');
                else if (cls.includes('anchor-top-')) classArray[i] = cls.replace('anchor-top-', 'anchor-bottom-');
                else if (cls.includes('anchor-bottom-')) classArray[i] = cls.replace('anchor-bottom-', 'anchor-top-');
            });
        }

        if (flipHorizontal) {
            classArray.forEach((cls, i) => {
                if (cls.includes('-left')) classArray[i] = cls.replace('-left', '-right');
                else if (cls.includes('-right')) classArray[i] = cls.replace('-right', '-left');
            });
        }

        return classArray;
    },

    // Main positioning function
    placePopover: function (anchorId, popoverId) {
        const anchor = document.getElementById(anchorId);
        const popover = document.getElementById(popoverId);

        if (!anchor || !popover) return;

        const classList = popover.classList;
        if (!classList.contains('popover-open')) return;

        const anchorTarget = anchor.firstElementChild ?? anchor;
        const anchorRect = anchorTarget.getBoundingClientRect();
        const popoverRect = popover.getBoundingClientRect();

        if (!this.isValidRect(anchorRect, true) || !this.isValidRect(popoverRect, true)) {
            this.retryPlacement(popover, anchorId, popoverId);
            return;
        }

        // Width modes
        const isRelativeWidth = classList.contains('popover-relative-width');
        const isAdaptiveWidth = classList.contains('popover-adaptive-width');

        popover.style.maxWidth = 'none';
        popover.style.minWidth = 'none';

        if (isRelativeWidth) {
            popover.style.maxWidth = anchorRect.width + 'px';
        } else if (isAdaptiveWidth) {
            popover.style.minWidth = anchorRect.width + 'px';
        }

        // Calculate position
        let position = this.calculatePosition(classList, anchorRect, popoverRect);

        // Auto-flip if needed
        const shouldFlip = this.shouldFlip(classList, position.anchorX, position.anchorY,
            popoverRect.width, popoverRect.height);

        if (shouldFlip.flipVertical || shouldFlip.flipHorizontal) {
            const flippedClasses = this.applyFlip(classList, shouldFlip.flipVertical, shouldFlip.flipHorizontal);
            const tempDiv = document.createElement('div');
            flippedClasses.forEach(c => tempDiv.classList.add(c));
            position = this.calculatePosition(tempDiv.classList, anchorRect, popoverRect);
        }

        // List height clamping
        const firstChild = popover.firstElementChild;
        if (firstChild && firstChild.classList.contains('popover-list')) {
            const popoverTop = position.top + position.offsetY;
            const availableHeight = window.innerHeight - popoverTop - this.overflowPadding;

            if (popoverRect.height > availableHeight) {
                const minHeight = this.overflowPadding * 3;
                const maxHeight = Math.max(availableHeight, minHeight);
                popover.style.maxHeight = maxHeight + 'px';
                firstChild.style.maxHeight = maxHeight + 'px';
            }
        }

        // Bounds checking
        let left = position.left + position.offsetX;
        let top = position.top + position.offsetY;

        if (!this.isFiniteNumber(left) || !this.isFiniteNumber(top)) {
            this.retryPlacement(popover, anchorId, popoverId);
            return;
        }

        if (left < this.overflowPadding && Math.abs(left) < popoverRect.width) {
            left = this.overflowPadding;
        }
        if (top < this.overflowPadding && Math.abs(top) < popoverRect.height) {
            top = this.overflowPadding;
        }

        // Apply position
        popover.style.left = left + 'px';
        popover.style.top = top + 'px';
        popover.removeAttribute('data-popover-retry');

        // Z-index (simple stacking for now - room for expansion)
        this.updateZIndex(popover);
    },

    // Simple z-index management (placeholder for future nested support)
    updateZIndex: function (popover) {
        if (!popover.style.zIndex || parseInt(popover.style.zIndex) < this.baseZIndex) {
            popover.style.zIndex = this.baseZIndex + 1;
        }
        // TODO: Handle nested popovers, dialogs, appbars
    },

    // Reposition all open popovers
    repositionAll: function () {
        const provider = document.querySelector('.' + this.containerClass);
        if (!provider) return;

        provider.querySelectorAll('.popover-open').forEach(popover => {
            const anchorId = popover.getAttribute('data-anchor-id');
            if (anchorId) {
                this.placePopover(anchorId, popover.id);
            }
        });
    }
};

/**
 * Popover lifecycle manager
 */
class PopoverManager {
    constructor() {
        this.popovers = new Map();
        this.observer = null;
        this.repositionDebounceMilliseconds = 25;
        this.onResize = null;
        this.onScroll = null;
    }

    bindWindowListeners() {
        this.onResize = window.popoverHelper.debounce(() => {
            window.popoverHelper.repositionAll();
        }, this.repositionDebounceMilliseconds);

        this.onScroll = window.popoverHelper.debounce(() => {
            window.popoverHelper.repositionAll();
        }, this.repositionDebounceMilliseconds);

        window.addEventListener('resize', this.onResize, { passive: true });
        window.addEventListener('scroll', this.onScroll, { passive: true });
    }

    unbindWindowListeners() {
        if (this.onResize) {
            window.removeEventListener('resize', this.onResize);
        }

        if (this.onScroll) {
            window.removeEventListener('scroll', this.onScroll);
        }

        this.onResize = null;
        this.onScroll = null;
    }

    setRepositionDebounce(debounceMilliseconds) {
        const parsed = Number.isFinite(debounceMilliseconds)
            ? Math.max(0, Math.floor(debounceMilliseconds))
            : 25;

        if (parsed === this.repositionDebounceMilliseconds) {
            return;
        }

        this.repositionDebounceMilliseconds = parsed;

        if (this.onResize || this.onScroll) {
            this.unbindWindowListeners();
            this.bindWindowListeners();
        }
    }

    initialize(containerClass, flipMargin, overflowPadding, baseZIndex) {
        window.popoverHelper.containerClass = containerClass;
        window.popoverHelper.flipMargin = flipMargin;
        window.popoverHelper.overflowPadding = overflowPadding;
        window.popoverHelper.baseZIndex = baseZIndex;

        this.unbindWindowListeners();
        this.bindWindowListeners();

        this.observeProvider();
    }

    observeProvider() {
        const provider = document.querySelector('.' + window.popoverHelper.containerClass);
        if (!provider) {
            console.error('Popover provider not found');
            return;
        }

        this.observer = new MutationObserver(mutations => {
            mutations.forEach(mutation => {
                if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                    const target = mutation.target;
                    if (target.classList.contains('popover-open')) {
                        const anchorId = target.getAttribute('data-anchor-id');
                        if (anchorId) {
                            window.popoverHelper.placePopover(anchorId, target.id);
                        }
                    }
                }
            });
        });

        this.observer.observe(provider, {
            attributes: true,
            subtree: true,
            attributeFilter: ['class']
        });
    }

    connect(anchorId, popoverId) {
        this.popovers.set(popoverId, { anchorId });

        const popover = document.getElementById(popoverId);
        if (popover) {
            popover.setAttribute('data-anchor-id', anchorId);
            if (popover.classList.contains('popover-open')) {
                window.popoverHelper.placePopover(anchorId, popoverId);
            }
        }
    }

    disconnect(popoverId) {
        this.popovers.delete(popoverId);
    }

    dispose() {
        if (this.observer) {
            this.observer.disconnect();
        }
        this.unbindWindowListeners();
        this.popovers.clear();
    }
}

window.popoverManager = new PopoverManager();









