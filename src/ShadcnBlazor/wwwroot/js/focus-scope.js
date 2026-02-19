(function () {
    function createFocusScope(container, { loop = false, trapped = false } = {}) {
        let lastFocusedElement = null;
        let paused = false;

        // ── Tabbable candidate discovery ──────────────────────────────────────────

        function getTabbableCandidates() {
            const nodes = [];
            const walker = document.createTreeWalker(container, NodeFilter.SHOW_ELEMENT, {
                acceptNode(node) {
                    if (node.disabled || node.hidden) return NodeFilter.FILTER_SKIP;
                    if (node.tagName === 'INPUT' && node.type === 'hidden') return NodeFilter.FILTER_SKIP;
                    return node.tabIndex >= 0 ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_SKIP;
                }
            });
            while (walker.nextNode()) nodes.push(walker.currentNode);
            return nodes;
        }

        function isHidden(node) {
            if (getComputedStyle(node).visibility === 'hidden') return true;
            let current = node;
            while (current && current !== container) {
                if (getComputedStyle(current).display === 'none') return true;
                current = current.parentElement;
            }
            return false;
        }

        function getTabbableEdges() {
            const candidates = getTabbableCandidates();
            const first = candidates.find(el => !isHidden(el));
            const last = [...candidates].reverse().find(el => !isHidden(el));
            return [first, last];
        }

        function focusElement(element, { select = false } = {}) {
            if (!element?.focus) return;
            const prev = document.activeElement;
            element.focus({ preventScroll: true });
            if (element !== prev && select && element instanceof HTMLInputElement && 'select' in element) {
                element.select();
            }
        }

        // ── Trap enforcement ──────────────────────────────────────────────────────

        function handleFocusIn(e) {
            if (paused) return;
            if (container.contains(e.target)) {
                lastFocusedElement = e.target;
            } else {
                focusElement(lastFocusedElement, { select: true });
            }
        }

        function handleFocusOut(e) {
            if (paused) return;
            if (e.relatedTarget === null) return; // tab away from browser / element removed
            if (!container.contains(e.relatedTarget)) {
                focusElement(lastFocusedElement, { select: true });
            }
        }

        const mutationObserver = new MutationObserver((mutations) => {
            if (document.activeElement !== document.body) return;
            for (const mutation of mutations) {
                if (mutation.removedNodes.length > 0) {
                    focusElement(container);
                    break;
                }
            }
        });

        // ── Tab looping ───────────────────────────────────────────────────────────

        function handleKeyDown(e) {
            if (!loop && !trapped) return;
            if (paused) return;
            if (e.key !== 'Tab' || e.altKey || e.ctrlKey || e.metaKey) return;

            const [first, last] = getTabbableEdges();
            if (!first || !last) {
                if (document.activeElement === container) e.preventDefault();
                return;
            }

            if (!e.shiftKey && document.activeElement === last) {
                e.preventDefault();
                if (loop) focusElement(first, { select: true });
            } else if (e.shiftKey && document.activeElement === first) {
                e.preventDefault();
                if (loop) focusElement(last, { select: true });
            }
        }

        // ── Mount ─────────────────────────────────────────────────────────────────

        const previouslyFocusedElement = document.activeElement;

        function mount() {
            if (trapped) {
                document.addEventListener('focusin', handleFocusIn);
                document.addEventListener('focusout', handleFocusOut);
                mutationObserver.observe(container, { childList: true, subtree: true });
            }

            container.addEventListener('keydown', handleKeyDown);

            // auto-focus first tabbable, excluding links (matches Radix behaviour)
            const mountEvent = new CustomEvent('focusScope.autoFocusOnMount', { bubbles: false, cancelable: true });
            container.dispatchEvent(mountEvent);
            if (!mountEvent.defaultPrevented) {
                const candidates = getTabbableCandidates().filter(el => el.tagName !== 'A');
                const prev = document.activeElement;
                for (const candidate of candidates) {
                    focusElement(candidate, { select: true });
                    if (document.activeElement !== prev) break;
                }
                if (document.activeElement === prev) focusElement(container);
            }
        }

        // ── Unmount ───────────────────────────────────────────────────────────────

        function unmount() {
            document.removeEventListener('focusin', handleFocusIn);
            document.removeEventListener('focusout', handleFocusOut);
            mutationObserver.disconnect();
            container.removeEventListener('keydown', handleKeyDown);

            setTimeout(() => {
                const unmountEvent = new CustomEvent('focusScope.autoFocusOnUnmount', { bubbles: false, cancelable: true });
                container.dispatchEvent(unmountEvent);
                if (!unmountEvent.defaultPrevented) {
                    focusElement(previouslyFocusedElement ?? document.body, { select: true });
                }
            }, 0);
        }

        mount();

        return {
            unmount,
            pause() { paused = true; },
            resume() { paused = false; },
        };
    }

// ── Stack (for nested scopes) ─────────────────────────────────────────────────

    const focusScopeStack = (() => {
        let stack = [];
        return {
            add(scope) {
                if (stack[0] && stack[0] !== scope) stack[0].pause();
                stack = stack.filter(s => s !== scope);
                stack.unshift(scope);
            },
            remove(scope) {
                stack = stack.filter(s => s !== scope);
                stack[0]?.resume();
            }
        };
    })();
})();