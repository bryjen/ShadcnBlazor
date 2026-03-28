import React from 'react';
import ReactDOM from 'react-dom/client';
import { Drawer } from 'vaul';
import './vaul.css';

const ROOT_ID = 'vaul-root';
const CLEANUP_DELAY_MS = 500;

let root = null;
let rootContainer = null;
let styleInjected = false;
let drawerIdCounter = 0;
const drawers = new Map();

function mergeClassNames(a, b) {
  if (a && b) return `${a} ${b}`;
  return a || b || '';
}

function pickOption(options, key) {
  if (!options) return undefined;
  if (options[key] !== undefined) return options[key];
  const pascal = `${key[0].toUpperCase()}${key.slice(1)}`;
  if (options[pascal] !== undefined) return options[pascal];
  return undefined;
}

function injectStyles() {
  if (styleInjected) return;
  if (!document?.head) return;

  const style = document.createElement('style');
  style.textContent = `
    .vaul-blazor-overlay {
      background: rgba(0, 0, 0, 0.5);
    }

    .vaul-blazor-content {
      background: var(--card);
      color: var(--foreground);
      border: 1px solid var(--border);
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.25);
    }
  `;
  document.head.appendChild(style);
  styleInjected = true;
}

function ensureRoot() {
  if (root) return;
  if (!document?.body) {
    console.warn('Vaul: document.body is not available, deferring initialization');
    document.addEventListener('DOMContentLoaded', ensureRoot, { once: true });
    return;
  }

  const existing = document.getElementById(ROOT_ID);
  if (existing) existing.remove();

  rootContainer = document.createElement('div');
  rootContainer.id = ROOT_ID;
  document.body.appendChild(rootContainer);
  root = ReactDOM.createRoot(rootContainer);

  injectStyles();
  renderDrawers();
}

function renderDrawers() {
  if (!root) return;

  const entries = Array.from(drawers.values());

  root.render(
    React.createElement(
      React.Fragment,
      null,
      entries.map((entry) =>
        React.createElement(DrawerInstance, {
          key: entry.id,
          entry,
        })
      )
    )
  );
}

function createBlazorHost(containerId) {
  return function BlazorHost() {
    const hostRef = React.useRef(null);

    React.useLayoutEffect(() => {
      const host = hostRef.current;
      const node = document.getElementById(containerId);
      if (!host || !node) return;

      if (node.parentNode !== host) {
        host.appendChild(node);
        node.style.display = '';
      }
    }, [containerId]);

    return React.createElement('div', {
      ref: hostRef,
      style: { display: 'contents' },
    });
  };
}

function createHiddenContainer(containerId, className, fragmentId) {
  const container = document.createElement('div');
  container.id = containerId;
  if (className) container.className = className;
  if (fragmentId !== undefined) {
    container.setAttribute('data-vaul-blazor', fragmentId);
  }
  container.style.display = 'none';
  document.body.appendChild(container);
  return container;
}

function DrawerInstance({ entry }) {
  const { id, open, containerId, options, callbackRef } = entry;
  const drawerOptions = pickOption(options, 'drawer') || {};
  const overlayOptions = pickOption(options, 'overlay') || {};
  const contentOptions = pickOption(options, 'content') || {};
  const title = pickOption(options, 'title') || 'Drawer';
  const description = pickOption(options, 'description') || 'Drawer content';
  const a11yHidden = pickOption(options, 'a11yHidden') ?? true;

  const drawerAdditional = pickOption(drawerOptions, 'additionalProps');
  const overlayAdditionalResolved = pickOption(overlayOptions, 'additionalProps');
  const contentAdditionalResolved = pickOption(contentOptions, 'additionalProps');
  const overlayDisableResolved = pickOption(overlayOptions, 'disableBaseClass');
  const contentDisableResolved = pickOption(contentOptions, 'disableBaseClass');
  const overlayClassResolved = pickOption(overlayOptions, 'className');
  const contentClassResolved = pickOption(contentOptions, 'className');

  const normalizedDrawerProps = {
    dismissible: pickOption(drawerOptions, 'dismissible'),
    modal: pickOption(drawerOptions, 'modal'),
    direction: pickOption(drawerOptions, 'direction'),
    closeThreshold: pickOption(drawerOptions, 'closeThreshold'),
    scrollLockTimeout: pickOption(drawerOptions, 'scrollLockTimeout'),
    handleOnly: pickOption(drawerOptions, 'handleOnly'),
    fixed: pickOption(drawerOptions, 'fixed'),
    nested: pickOption(drawerOptions, 'nested'),
    repositionInputs: pickOption(drawerOptions, 'repositionInputs'),
    preventScrollRestoration: pickOption(drawerOptions, 'preventScrollRestoration'),
    disablePreventScroll: pickOption(drawerOptions, 'disablePreventScroll'),
    autoFocus: pickOption(drawerOptions, 'autoFocus'),
    shouldScaleBackground: pickOption(drawerOptions, 'shouldScaleBackground'),
    setBackgroundColorOnScale: pickOption(drawerOptions, 'setBackgroundColorOnScale'),
    backgroundColor: pickOption(drawerOptions, 'backgroundColor'),
    noBodyStyles: pickOption(drawerOptions, 'noBodyStyles'),
    fadeFromIndex: pickOption(drawerOptions, 'fadeFromIndex'),
    snapToSequentialPoint: pickOption(drawerOptions, 'snapToSequentialPoint'),
    snapPoints: pickOption(drawerOptions, 'snapPoints'),
    activeSnapPoint: pickOption(drawerOptions, 'activeSnapPoint'),
  };

  const overlayBaseClass = overlayDisableResolved ? '' : 'vaul-blazor-overlay';
  const contentBaseClass = contentDisableResolved ? '' : 'vaul-blazor-content';

  const overlayAdditionalClass =
    pickOption(overlayAdditionalResolved, 'className') ?? overlayAdditionalResolved?.className;
  const contentAdditionalClass =
    pickOption(contentAdditionalResolved, 'className') ?? contentAdditionalResolved?.className;

  const overlayClassName = mergeClassNames(
    mergeClassNames(overlayBaseClass, overlayClassResolved),
    overlayAdditionalClass
  );
  const contentClassName = mergeClassNames(
    mergeClassNames(contentBaseClass, contentClassResolved),
    contentAdditionalClass
  );

  const overlayProps = {
    ...(overlayAdditionalResolved || {}),
    className: overlayClassName,
  };

  const contentProps = {
    ...(contentAdditionalResolved || {}),
    className: contentClassName,
  };

  const drawerProps = {
    ...normalizedDrawerProps,
    ...(drawerAdditional || {}),
  };

  const BlazorHost = React.useMemo(() => createBlazorHost(containerId), [containerId]);

  const handleOpenChange = React.useCallback(
    (isOpen) => {
      if (callbackRef?.invokeMethodAsync) {
        callbackRef.invokeMethodAsync('OnOpenChange', isOpen);
      }
      if (!isOpen) {
        window.Vaul?.close(id);
      }
    },
    [id, callbackRef]
  );

  const handleDrag = React.useCallback(
    (_event, percentageDragged) => {
      if (callbackRef?.invokeMethodAsync) {
        callbackRef.invokeMethodAsync('OnDrag', percentageDragged);
      }
    },
    [callbackRef]
  );

  const handleRelease = React.useCallback(
    (_event, isOpen) => {
      if (callbackRef?.invokeMethodAsync) {
        callbackRef.invokeMethodAsync('OnRelease', isOpen);
      }
    },
    [callbackRef]
  );

  const handleClose = React.useCallback(() => {
    if (callbackRef?.invokeMethodAsync) {
      callbackRef.invokeMethodAsync('OnClose');
    }
  }, [callbackRef]);

  const handleAnimationEnd = React.useCallback(
    (isOpen) => {
      if (callbackRef?.invokeMethodAsync) {
        callbackRef.invokeMethodAsync('OnAnimationEnd', isOpen);
      }
    },
    [callbackRef]
  );

  const [activeSnap, setActiveSnap] = React.useState(() => {
    return drawerProps.activeSnapPoint ?? drawerProps.snapPoints?.[0] ?? null;
  });

  React.useEffect(() => {
    if (drawerProps.activeSnapPoint !== undefined) {
      setActiveSnap(drawerProps.activeSnapPoint);
      return;
    }
    if (drawerProps.snapPoints?.length) {
      setActiveSnap(drawerProps.snapPoints[0]);
    }
  }, [drawerProps.activeSnapPoint, drawerProps.snapPoints]);

  const handleSnapPointChange = React.useCallback(
    (value) => {
      setActiveSnap(value);
      if (callbackRef?.invokeMethodAsync) {
        callbackRef.invokeMethodAsync('OnSnapPointChange', value);
      }
    },
    [callbackRef]
  );

  const dismissible =
    drawerProps.dismissible === undefined ? true : drawerProps.dismissible;
  const modal = drawerProps.modal === undefined ? true : drawerProps.modal;
  const snapPoints = drawerProps.snapPoints;
  const activeSnapPoint = activeSnap;
  const fadeFromIndex =
    drawerProps.fadeFromIndex === undefined && snapPoints ? 0 : drawerProps.fadeFromIndex;

  return React.createElement(
    Drawer.Root,
    {
      ...drawerProps,
      open,
      onOpenChange: handleOpenChange,
      onDrag: handleDrag,
      onRelease: handleRelease,
      onClose: handleClose,
      onAnimationEnd: handleAnimationEnd,
      setActiveSnapPoint: handleSnapPointChange,
      snapPoints,
      activeSnapPoint,
      fadeFromIndex,
      modal: modal,
      dismissible,
    },
    React.createElement(
      React.Fragment,
      null,
      React.createElement(Drawer.Overlay, overlayProps),
      React.createElement(
        Drawer.Portal,
        null,
        React.createElement(
          Drawer.Content,
          contentProps,
          React.createElement(
            Drawer.Title,
            { className: a11yHidden ? 'vaul-visually-hidden' : undefined },
            title
          ),
          React.createElement(
            Drawer.Description,
            { className: a11yHidden ? 'vaul-visually-hidden' : undefined },
            description
          ),
          React.createElement(BlazorHost, null)
        )
      )
    )
  );
}

async function cleanupDrawer(drawerId) {
  const entry = drawers.get(drawerId);
  if (!entry) return;

  if (entry.cleanupScheduled) return;
  entry.cleanupScheduled = true;

  try {
    if (entry.blazorRoot) {
      await entry.blazorRoot.dispose();
    }
  } catch (error) {
    console.warn('Vaul: Error disposing Blazor root component:', error);
  }

  try {
    if (entry.callbackRef?.invokeMethodAsync) {
      await entry.callbackRef.invokeMethodAsync('OnDisposed');
    }
  } catch (error) {
    console.warn('Vaul: Error notifying .NET callback disposal:', error);
  }

  const node = document.getElementById(entry.containerId);
  if (node) node.remove();

  drawers.delete(drawerId);
  renderDrawers();

  if (drawers.size === 0 && root) {
    try {
      root.unmount();
    } catch (error) {
      console.warn('Vaul: Error unmounting root:', error);
    }
    root = null;
    if (rootContainer?.parentNode) rootContainer.remove();
    rootContainer = null;
  }
}

async function openComponent(componentIdentifier, fragmentId, options, callbackRef) {
  ensureRoot();

  if (!document?.body) {
    console.error('Vaul: document.body is not available.');
    return null;
  }

  if (!window.Blazor || !window.Blazor.rootComponents) {
    console.error('Vaul: Blazor.rootComponents is not available.');
    return null;
  }

  const drawerId = `vaul_${drawerIdCounter++}`;
  const containerId = `vaul-component-${fragmentId}-${drawerId}`;

  const container = createHiddenContainer(containerId, 'vaul-blazor-host', fragmentId);

  let blazorRoot = null;
  try {
    try {
      blazorRoot = await window.Blazor.rootComponents.add(container, componentIdentifier, { fragmentId });
    } catch (error) {
      if (typeof componentIdentifier === 'string' && componentIdentifier.includes('::')) {
        const fallbackIdentifier = componentIdentifier.split('::').pop();
        blazorRoot = await window.Blazor.rootComponents.add(container, fallbackIdentifier, { fragmentId });
      } else {
        throw error;
      }
    }

    drawers.set(drawerId, {
      id: drawerId,
      open: true,
      containerId,
      fragmentId,
      blazorRoot,
      options: options || {},
      callbackRef: callbackRef || null,
      cleanupScheduled: false,
    });

    renderDrawers();
    return drawerId;
  } catch (error) {
    console.error('Vaul: Error mounting Blazor component:', error);
    container.remove();
    return null;
  }
}

function close(drawerId) {
  const entry = drawers.get(drawerId);
  if (!entry) return;

  if (!entry.open) return;

  entry.open = false;
  renderDrawers();

  window.setTimeout(() => {
    cleanupDrawer(drawerId);
  }, CLEANUP_DELAY_MS);
}

function update(drawerId, options) {
  const entry = drawers.get(drawerId);
  if (!entry) return;

  entry.options = options || {};
  renderDrawers();
}

function destroy(drawerId) {
  cleanupDrawer(drawerId);
}

function cleanupAll() {
  const ids = Array.from(drawers.keys());
  for (const id of ids) {
    cleanupDrawer(id);
  }
}

if (typeof window !== 'undefined') {
  if (window.Vaul && typeof window.Vaul._cleanup === 'function') {
    window.Vaul._cleanup();
  }

  window.Vaul = {
    openComponent,
    close,
    update,
    destroy,
    _cleanup: cleanupAll,
  };
}
