/**
 * Popover positioning via Floating UI.
 * https://floating-ui.com/docs/getting-started
 */
import { computePosition, flip, shift, offset as fuOffset, autoUpdate, hide }
  from 'https://cdn.jsdelivr.net/npm/@floating-ui/dom@1.7.5/+esm';

let _overflowPadding = 10;
let _flipMargin = 0;
let _baseZIndex = 1200;
let _currentZ = 1200;

const _cleanups = new Map();   // popoverId → autoUpdate cleanup fn
const _outsideInteract = new Map(); // popoverId → { handler, callbackReference }

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

  const handler = event => {
    const f = document.getElementById(popoverId);
    const a = document.getElementById(anchorId);
    if (!f || !a) return;
    if (!f.classList.contains('popover-open')) return;
    
    // Check if the interaction is outside both anchor and floating
    if (f.contains(event.target) || a.contains(event.target)) return;
    
    const eventType = (event.type === 'focusin' || event.type === 'focus') ? 1 : 0;
    callbackReference.invokeMethodAsync('HandleInteractOutside', eventType);
  };

  document.addEventListener('pointerdown', handler, true);
  document.addEventListener('focusin', handler, true);
  _outsideInteract.set(popoverId, { handler, callbackReference });
}

export function disableOutsideClickClose(popoverId) {
  const sub = _outsideInteract.get(popoverId);
  if (!sub) return;
  document.removeEventListener('pointerdown', sub.handler, true);
  document.removeEventListener('focusin', sub.handler, true);
  _outsideInteract.delete(popoverId);
}

export function repositionAll() {
  for (const [popoverId] of _cleanups) {
    const floating = document.getElementById(popoverId);
    if (floating?.classList.contains('popover-open')) {
      const anchorId = floating.getAttribute('data-anchor-id');
      if (anchorId) {
        const anchor = document.getElementById(anchorId);
        if (anchor) {
          _update(anchor, floating, popoverId, floating._floatingOptions ?? {});
        }
      }
    }
  }
}

export function dispose() {
  for (const cleanup of _cleanups.values()) cleanup();
  _cleanups.clear();

  for (const popoverId of _outsideInteract.keys()) disableOutsideClickClose(popoverId);
}

// ─── Internal ────────────────────────────────────────────────────────────────

async function _update(anchor, floating, popoverId, options) {
  const {
    placement = 'bottom',
    anchorPlacement,
    widthMode,
    clampList = false,
    offset: offsetPx = 0,
    alignOffset = 0,
    hideWhenDetached = false
  } = options;

  // Store options on the element so repositionAll can reuse them
  floating._floatingOptions = options;

  const anchorTarget = anchor.firstElementChild ?? anchor;

  // Width modes — applied before measuring
  const anchorRect = anchorTarget.getBoundingClientRect();
  floating.style.setProperty('--popover-width', anchorRect.width + 'px');
  
  if (widthMode === 'relative') {
    floating.style.width    = anchorRect.width + 'px';
    floating.style.maxWidth = anchorRect.width + 'px';
    floating.style.minWidth = anchorRect.width + 'px';
  } else if (widthMode === 'adaptive') {
    floating.style.width = 'auto';
    floating.style.minWidth = anchorRect.width + 'px';
    floating.style.maxWidth = 'none';
  } else {
    floating.style.width = 'auto';
    floating.style.minWidth = 'none';
    floating.style.maxWidth = 'none';
  }

  const middleware = [
    fuOffset({ mainAxis: offsetPx, crossAxis: alignOffset }),
    flip({ padding: _flipMargin }),
    shift({ padding: _overflowPadding }),
  ];

  if (hideWhenDetached) {
    middleware.push(hide({ strategy: 'referenceHidden' }));
  }

  const { x, y, placement: resolvedPlacement, middlewareData } = await computePosition(anchorTarget, floating, {
    placement,
    middleware
  });

  Object.assign(floating.style, { left: `${x}px`, top: `${y}px` });

  if (hideWhenDetached && middlewareData.hide) {
    floating.style.visibility = middlewareData.hide.referenceHidden ? 'hidden' : 'visible';
  } else {
    floating.style.visibility = 'visible';
  }

  // data-resolved-side and data-resolved-align
  const [side, align = 'center'] = resolvedPlacement.split('-');
  floating.setAttribute('data-resolved-side', side);
  floating.setAttribute('data-resolved-align', align);

  // --popover-transform-origin
  const originSide = { top: 'bottom', bottom: 'top', left: 'right', right: 'left' }[side];
  const originAlign = {
    start: (side === 'top' || side === 'bottom') ? 'left' : 'top',
    end: (side === 'top' || side === 'bottom') ? 'right' : 'bottom',
    center: 'center'
  }[align];
  floating.style.setProperty('--popover-transform-origin', `${originSide} ${originAlign}`);

  // List clamping
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
