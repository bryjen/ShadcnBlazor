/**
 * Popover positioning via Floating UI.
 * https://floating-ui.com/docs/getting-started
 */
import { computePosition, flip, shift, offset as fuOffset, autoUpdate }
  from 'https://cdn.jsdelivr.net/npm/@floating-ui/dom@1.7.5/+esm';

let _overflowPadding = 10;
let _flipMargin = 0;
let _baseZIndex = 1200;
let _currentZ = 1200;

const _cleanups = new Map();   // popoverId → autoUpdate cleanup fn
const _outsideClick = new Map(); // popoverId → { handler, callbackReference }

// ─── Exported API ───────────────────────────────────────────────────────────

export function initialize(containerClass, flipMargin, overflowPadding, baseZIndex) {
  _flipMargin = flipMargin;
  _overflowPadding = overflowPadding;
  _baseZIndex = baseZIndex;
  _currentZ = baseZIndex;
}

export function setRepositionDebounce(_debounceMilliseconds) {
  // autoUpdate handles debouncing internally; parameter kept for API compatibility
}

export function connect(anchorId, popoverId, options = {}) {
  const anchor = document.getElementById(anchorId);
  const floating = document.getElementById(popoverId);
  if (!anchor || !floating) return;

  // Base position — required for Floating UI
  Object.assign(floating.style, { position: 'absolute', top: '0', left: '0' });
  floating.style.zIndex = String(++_currentZ);

  const cleanup = autoUpdate(anchor, floating, () =>
    _update(anchor, floating, popoverId, options)
  );
  _cleanups.set(popoverId, cleanup);
}

export function disconnect(popoverId) {
  _cleanups.get(popoverId)?.();
  _cleanups.delete(popoverId);
  disableOutsideClickClose(popoverId);
}

export function enableOutsideClickClose(anchorId, popoverId, callbackReference) {
  disableOutsideClickClose(popoverId);

  const anchor   = document.getElementById(anchorId);
  const floating = document.getElementById(popoverId);

  const handler = event => {
    const f = document.getElementById(popoverId);
    const a = document.getElementById(anchorId);
    if (!f || !a) return;
    if (!f.classList.contains('popover-open')) return;
    if (f.contains(event.target) || a.contains(event.target)) return;
    callbackReference.invokeMethodAsync('HandleOutsidePointerDown');
  };

  document.addEventListener('click', handler, false);
  _outsideClick.set(popoverId, { handler, callbackReference });
}

export function disableOutsideClickClose(popoverId) {
  const sub = _outsideClick.get(popoverId);
  if (!sub) return;
  document.removeEventListener('click', sub.handler, false);
  _outsideClick.delete(popoverId);
}

export function repositionAll() {
  for (const [popoverId] of _cleanups) {
    const floating = document.getElementById(popoverId);
    if (floating?.classList.contains('popover-open')) {
      const anchorId = floating.getAttribute('data-anchor-id');
      if (anchorId) {
        const anchor = document.getElementById(anchorId);
        if (anchor) {
          // trigger a re-compute by calling autoUpdate's callback manually
          // (autoUpdate already handles this; this is a manual fallback)
          _cleanups.get(popoverId)?.();
          connect(anchorId, popoverId, floating._floatingOptions ?? {});
        }
      }
    }
  }
}

export function dispose() {
  for (const cleanup of _cleanups.values()) cleanup();
  _cleanups.clear();

  for (const popoverId of _outsideClick.keys()) disableOutsideClickClose(popoverId);
}

// ─── Internal ────────────────────────────────────────────────────────────────

async function _update(anchor, floating, popoverId, options) {
  const {
    placement = 'bottom',
    anchorPlacement,
    widthMode,
    clampList = false,
    offset: offsetPx = 0
  } = options;

  // Store options on the element so repositionAll can reuse them
  floating._floatingOptions = options;

  const anchorTarget = anchor.firstElementChild ?? anchor;

  // Width modes — applied before measuring
  const anchorRect = anchorTarget.getBoundingClientRect();
  floating.style.setProperty('--popover-width', anchorRect.width + 'px');
  floating.style.width = 'auto';
  floating.style.minWidth = 'none';
  floating.style.maxWidth = 'none';

  if (widthMode === 'relative') {
    floating.style.width    = anchorRect.width + 'px';
    floating.style.maxWidth = anchorRect.width + 'px';
    floating.style.minWidth = anchorRect.width + 'px';
  } else if (widthMode === 'adaptive') {
    floating.style.minWidth = anchorRect.width + 'px';
  }

  const { x, y, placement: resolvedPlacement } = await computePosition(anchorTarget, floating, {
    placement,
    middleware: [
      fuOffset(offsetPx),
      flip({ padding: _flipMargin }),
      shift({ padding: _overflowPadding }),
    ],
  });

  Object.assign(floating.style, { left: `${x}px`, top: `${y}px` });

  // data-resolved-side — used by tooltip arrow CSS in other.css lines 139-172
  const resolvedSide = resolvedPlacement.split('-')[0]; // 'top-start' → 'top'
  floating.setAttribute('data-resolved-side', resolvedSide);

  // List clamping — mirrors original popoverHelper list clamp logic
  if (clampList) {
    const firstChild = floating.firstElementChild;
    if (firstChild?.classList.contains('popover-list')) {
      const floatingRect = floating.getBoundingClientRect();
      const availableHeight = window.innerHeight - floatingRect.top - _overflowPadding;
      if (floatingRect.height > availableHeight) {
        const minHeight = _overflowPadding * 3;
        const maxHeight = Math.max(availableHeight, minHeight);
        floating.style.maxHeight = maxHeight + 'px';
        firstChild.style.maxHeight = maxHeight + 'px';
      }
    }
  }
}
