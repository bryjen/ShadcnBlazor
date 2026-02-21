/**
 * Simplified popover positioning system
 * Features: basic positioning, auto-flip, width modes, list clamping, debouncing
 * Future: animations, nested popovers
 * Exported as ES module for Blazor JS interop.
 */

const popoverHelper = {
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

    calculatePosition: function (classList, anchorRect, popoverRect) {
        let top = anchorRect.top;
        let left = anchorRect.left;
        let offsetX = 0;
        let offsetY = 0;

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

    shouldFlip: function (classList, anchorX, anchorY, popoverWidth, popoverHeight) {
        const margin = this.flipMargin;
        let flipVertical = false;
        let flipHorizontal = false;

        if (classList.contains('popover-top-left') ||
            classList.contains('popover-top-center') ||
            classList.contains('popover-top-right')) {
            const spaceBelow = window.innerHeight - anchorY - margin;
            const spaceAbove = anchorY - margin;
            if (popoverHeight > spaceBelow && spaceAbove > spaceBelow) {
                flipVertical = true;
            }
        }

        if (classList.contains('popover-bottom-left') ||
            classList.contains('popover-bottom-center') ||
            classList.contains('popover-bottom-right')) {
            const spaceAbove = anchorY - margin;
            const spaceBelow = window.innerHeight - anchorY - margin;
            if (popoverHeight > spaceAbove && spaceBelow > spaceAbove) {
                flipVertical = true;
            }
        }

        if (classList.contains('popover-top-left') ||
            classList.contains('popover-center-left') ||
            classList.contains('popover-bottom-left')) {
            const spaceRight = window.innerWidth - anchorX - margin;
            const spaceLeft = anchorX - margin;
            if (popoverWidth > spaceRight && spaceLeft > spaceRight) {
                flipHorizontal = true;
            }
        }

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

        const isRelativeWidth = classList.contains('popover-relative-width');
        const isAdaptiveWidth = classList.contains('popover-adaptive-width');

        popover.style.maxWidth = 'none';
        popover.style.minWidth = 'none';
        popover.style.width = 'auto';

        popover.style.setProperty('--popover-width', anchorRect.width + 'px');

        if (isRelativeWidth) {
            popover.style.width = anchorRect.width + 'px';
            popover.style.maxWidth = anchorRect.width + 'px';
            popover.style.minWidth = anchorRect.width + 'px';
        } else if (isAdaptiveWidth) {
            popover.style.minWidth = anchorRect.width + 'px';
        }

        let position = this.calculatePosition(classList, anchorRect, popoverRect);

        const shouldFlip = this.shouldFlip(classList, position.anchorX, position.anchorY,
            popoverRect.width, popoverRect.height);

        let finalPlacementClasses = classList;
        if (shouldFlip.flipVertical || shouldFlip.flipHorizontal) {
            const flippedClasses = this.applyFlip(classList, shouldFlip.flipVertical, shouldFlip.flipHorizontal);
            const tempDiv = document.createElement('div');
            flippedClasses.forEach(c => tempDiv.classList.add(c));
            position = this.calculatePosition(tempDiv.classList, anchorRect, popoverRect);
            finalPlacementClasses = tempDiv.classList;
        }

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

        let left = position.left + position.offsetX;
        let top = position.top + position.offsetY;

        const offset = parseInt(popover.getAttribute('data-offset') || '0', 10);
        if (offset > 0) {
            if (finalPlacementClasses.contains('popover-bottom-left') ||
                finalPlacementClasses.contains('popover-bottom-center') ||
                finalPlacementClasses.contains('popover-bottom-right')) {
                top -= offset;
            } else if (finalPlacementClasses.contains('popover-top-left') ||
                finalPlacementClasses.contains('popover-top-center') ||
                finalPlacementClasses.contains('popover-top-right')) {
                top += offset;
            } else if (finalPlacementClasses.contains('popover-center-left') ||
                finalPlacementClasses.contains('popover-top-left') ||
                finalPlacementClasses.contains('popover-bottom-left')) {
                left -= offset;
            } else if (finalPlacementClasses.contains('popover-center-right') ||
                finalPlacementClasses.contains('popover-top-right') ||
                finalPlacementClasses.contains('popover-bottom-right')) {
                left += offset;
            }
        }

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

        popover.style.left = left + 'px';
        popover.style.top = top + 'px';
        popover.removeAttribute('data-popover-retry');

        const resolvedSide = this.getResolvedSide(finalPlacementClasses);
        popover.setAttribute('data-resolved-side', resolvedSide);

        this.updateZIndex(popover);
    },

    getResolvedSide: function (classList) {
        if (classList.contains('popover-bottom-left') || classList.contains('popover-bottom-center') || classList.contains('popover-bottom-right')) {
            return 'top';
        }
        if (classList.contains('popover-top-left') || classList.contains('popover-top-center') || classList.contains('popover-top-right')) {
            return 'bottom';
        }
        if (classList.contains('popover-center-left') || classList.contains('popover-top-left') || classList.contains('popover-bottom-left')) {
            return 'left';
        }
        return 'right';
    },

    updateZIndex: function (popover) {
        if (!popover.style.zIndex || parseInt(popover.style.zIndex) < this.baseZIndex) {
            popover.style.zIndex = this.baseZIndex + 1;
        }
    },

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

class PopoverManager {
    constructor(helper) {
        this.helper = helper;
        this.popovers = new Map();
        this.observer = null;
        this.repositionDebounceMilliseconds = 25;
        this.onResize = null;
        this.onScroll = null;
        this.outsideClickSubscriptions = new Map();
    }

    bindWindowListeners() {
        this.onResize = this.helper.debounce(() => {
            this.helper.repositionAll();
        }, this.repositionDebounceMilliseconds);

        this.onScroll = this.helper.debounce(() => {
            this.helper.repositionAll();
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
        this.helper.containerClass = containerClass;
        this.helper.flipMargin = flipMargin;
        this.helper.overflowPadding = overflowPadding;
        this.helper.baseZIndex = baseZIndex;

        this.unbindWindowListeners();
        this.bindWindowListeners();

        this.observeProvider();
    }

    observeProvider() {
        const provider = document.querySelector('.' + this.helper.containerClass);
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
                            this.helper.placePopover(anchorId, target.id);
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
                this.helper.placePopover(anchorId, popoverId);
            }
        }
    }

    enableOutsideClickClose(anchorId, popoverId, callbackReference) {
        this.disableOutsideClickClose(popoverId);

        const handler = event => {
            const popover = document.getElementById(popoverId);
            const anchor = document.getElementById(anchorId);
            if (!popover || !anchor) {
                return;
            }

            if (!popover.classList.contains('popover-open')) {
                return;
            }

            const target = event.target;
            if (popover.contains(target) || anchor.contains(target)) {
                return;
            }

            callbackReference.invokeMethodAsync('HandleOutsidePointerDown');
        };

        document.addEventListener('click', handler, false);
        this.outsideClickSubscriptions.set(popoverId, {
            handler,
            callbackReference
        });
    }

    disableOutsideClickClose(popoverId) {
        const subscription = this.outsideClickSubscriptions.get(popoverId);
        if (!subscription) {
            return;
        }

        document.removeEventListener('click', subscription.handler, false);
        this.outsideClickSubscriptions.delete(popoverId);
    }

    disconnect(popoverId) {
        this.popovers.delete(popoverId);
        this.disableOutsideClickClose(popoverId);
    }

    dispose() {
        if (this.observer) {
            this.observer.disconnect();
        }

        for (const key of this.outsideClickSubscriptions.keys()) {
            this.disableOutsideClickClose(key);
        }

        this.unbindWindowListeners();
        this.popovers.clear();
    }
}

const popoverManager = new PopoverManager(popoverHelper);

export function initialize(containerClass, flipMargin, overflowPadding, baseZIndex) {
    popoverManager.initialize(containerClass, flipMargin, overflowPadding, baseZIndex);
}

export function setRepositionDebounce(debounceMilliseconds) {
    popoverManager.setRepositionDebounce(debounceMilliseconds);
}

export function connect(anchorId, popoverId) {
    popoverManager.connect(anchorId, popoverId);
}

export function disconnect(popoverId) {
    popoverManager.disconnect(popoverId);
}

export function enableOutsideClickClose(anchorId, popoverId, callbackReference) {
    popoverManager.enableOutsideClickClose(anchorId, popoverId, callbackReference);
}

export function disableOutsideClickClose(popoverId) {
    popoverManager.disableOutsideClickClose(popoverId);
}

export function dispose() {
    popoverManager.dispose();
}

