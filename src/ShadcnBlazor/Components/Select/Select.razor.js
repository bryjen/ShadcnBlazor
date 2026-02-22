export function ensureOptionVisible(listboxId, optionId) {
  const listbox = document.getElementById(listboxId);
  const option = document.getElementById(optionId);

  if (!listbox || !option) {
    return;
  }

  const listboxRect = listbox.getBoundingClientRect();
  const optionRect = option.getBoundingClientRect();

  if (optionRect.top < listboxRect.top) {
    listbox.scrollTop -= listboxRect.top - optionRect.top;
  } else if (optionRect.bottom > listboxRect.bottom) {
    listbox.scrollTop += optionRect.bottom - listboxRect.bottom;
  }
}

const listboxResizeHandlers = new Map();
let popoverModulePromise;

function getPopoverModule() {
  if (!popoverModulePromise) {
    popoverModulePromise = import('/ShadcnBlazor/_content/ShadcnBlazor/js/popovers.js');
  }

  return popoverModulePromise;
}

function queuePopoverReposition() {
  requestAnimationFrame(() => {
    getPopoverModule()
      .then((module) => {
        if (typeof module?.repositionAll === 'function') {
          module.repositionAll();
        }
      })
      .catch(() => {
        // Ignore reposition errors to avoid breaking select interactions.
      });
  });
}

function measureAndApplyListboxMaxHeight(listboxId, maxVisibleItems, viewportMarginPx) {
  const listbox = document.getElementById(listboxId);
  if (!listbox) {
    return;
  }

  const options = listbox.querySelectorAll('[role="option"]');
  if (!options.length || !Number.isFinite(maxVisibleItems) || maxVisibleItems < 1) {
    listbox.style.removeProperty('max-height');
    return;
  }

  const count = Math.min(maxVisibleItems, options.length);
  let measuredHeight = 0;
  for (let i = 0; i < count; i++) {
    measuredHeight += options[i].offsetHeight;
  }

  const viewportCap = Math.max(window.innerHeight - viewportMarginPx, 0);
  const maxHeight = Math.min(measuredHeight, viewportCap);

  listbox.style.maxHeight = `${maxHeight}px`;
  listbox.style.overflowY = 'auto';
}

export function applyListboxMaxVisibleItems(listboxId, maxVisibleItems, viewportMarginPx = 96) {
  const maxItems = Number(maxVisibleItems);
  const margin = Number.isFinite(viewportMarginPx) ? viewportMarginPx : 96;

  const existingHandler = listboxResizeHandlers.get(listboxId);
  if (existingHandler) {
    window.removeEventListener('resize', existingHandler);
    listboxResizeHandlers.delete(listboxId);
  }

  const handler = () => {
    measureAndApplyListboxMaxHeight(listboxId, maxItems, margin);
    queuePopoverReposition();
  };

  handler();

  window.addEventListener('resize', handler);
  listboxResizeHandlers.set(listboxId, handler);
}

export function disposeListboxMaxVisibleItems(listboxId) {
  const handler = listboxResizeHandlers.get(listboxId);
  if (handler) {
    window.removeEventListener('resize', handler);
    listboxResizeHandlers.delete(listboxId);
  }
}

function isFocusableElement(element) {
  if (!element) {
    return false;
  }

  if (element.hasAttribute('disabled') || element.getAttribute('aria-disabled') === 'true') {
    return false;
  }

  if (element.getAttribute('tabindex') === '-1') {
    return false;
  }

  const style = window.getComputedStyle(element);
  if (style.display === 'none' || style.visibility === 'hidden') {
    return false;
  }

  return true;
}

export function focusAdjacentFocusable(triggerId, moveForward) {
  const trigger = document.getElementById(triggerId);
  if (!trigger) {
    return;
  }

  const selector = [
    'a[href]',
    'button',
    'input',
    'select',
    'textarea',
    '[tabindex]'
  ].join(',');

  const focusables = Array.from(document.querySelectorAll(selector)).filter(isFocusableElement);
  const currentIndex = focusables.findIndex((element) => element === trigger);
  if (currentIndex < 0) {
    return;
  }

  const nextIndex = moveForward ? currentIndex + 1 : currentIndex - 1;
  if (nextIndex < 0 || nextIndex >= focusables.length) {
    return;
  }

  focusables[nextIndex].focus();
}
