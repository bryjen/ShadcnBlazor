import React from 'react';
import ReactDOM from 'react-dom/client';
import { Toaster, toast } from 'sonner';
import { CheckCircle, XCircle, AlertTriangle, Info } from 'lucide-react';

let toasterRoot = null;
let toasterContainer = null;
let themeObserver = null;
let sonnerServiceRef = null;
const componentRoots = new Map();
const promiseRegistry = new Map();
let promiseIdCounter = 0;

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
      style: { display: 'contents' }
    });
  };
}

function createHiddenContainer(containerId, className, fragmentId) {
  const container = document.createElement('div');
  container.id = containerId;
  if (className) {
    container.className = className;
  }
  if (fragmentId !== undefined) {
    container.setAttribute('data-sonner-blazor', fragmentId);
  }
  container.style.display = 'none';
  document.body.appendChild(container);
  return container;
}

/**
 * Detect current theme based on 'dark' class on html element
 */
function getTheme() {
  return document.documentElement.classList.contains('dark') ? 'dark' : 'light';
}

/**
 * Render the Toaster with current theme
 */
function renderToaster() {
  if (!toasterRoot || !toasterContainer) return;

  toasterRoot.render(React.createElement(Toaster, {
    position: 'top-center',
    theme: getTheme(),
    toastOptions: {
      style: {
        background: 'var(--card)',
        borderColor: 'var(--border)',
        color: 'var(--foreground)',
        border: '1px solid var(--border)'
      }
    }
  }));
}

/**
 * Initialize the Sonner Toaster component
 * Handles DOM readiness and reload scenarios
 */
function initializeSonner() {
  // Check if already initialized
  if (toasterRoot !== null) {
    return;
  }

  // Ensure document.body is available
  if (!document.body) {
    console.warn('Sonner: document.body is not available, deferring initialization');
    document.addEventListener('DOMContentLoaded', initializeSonner, { once: true });
    return;
  }


  // Check for existing container (reload scenario)
  const existing = document.getElementById('sonner-container');
  if (existing) {
    existing.remove();
  }

  // Create a container for the Sonner Toaster component
  toasterContainer = document.createElement('div');
  toasterContainer.id = 'sonner-container';
  document.body.appendChild(toasterContainer);

  // Inject CSS to override Sonner variables with design tokens
  const styleEl = document.createElement('style');
  styleEl.textContent = `
    #sonner-container [data-sonner-toaster] {
      --normal-bg: var(--card);
      --normal-border: var(--border);
      --normal-text: var(--foreground);
      --normal-bg-hover: var(--card);
      --normal-border-hover: var(--border);
      --success-bg: var(--card);
      --success-border: var(--border);
      --success-text: var(--foreground);
      --error-bg: var(--card);
      --error-border: var(--border);
      --error-text: var(--foreground);
      --warning-bg: var(--card);
      --warning-border: var(--border);
      --warning-text: var(--foreground);
    }
  `;
  document.head.appendChild(styleEl);

  // Mount the Toaster component
  toasterRoot = ReactDOM.createRoot(toasterContainer);
  renderToaster();

  // Watch for theme changes on html element
  if (themeObserver) {
    themeObserver.disconnect();
  }

  themeObserver = new MutationObserver(() => {
    renderToaster();
  });

  themeObserver.observe(document.documentElement, {
    attributes: true,
    attributeFilter: ['class']
  });
}

/**
 * Clean up Sonner instance (for reload scenarios)
 */
function cleanupSonner() {
  if (themeObserver) {
    themeObserver.disconnect();
    themeObserver = null;
  }

  if (componentRoots.size > 0) {
    for (const root of componentRoots.values()) {
      try {
        root.dispose();
      } catch (e) {
        console.warn('Sonner: Error disposing root component during cleanup:', e);
      }
    }
    componentRoots.clear();
  }

  if (toasterRoot !== null) {
    try {
      toasterRoot.unmount();
    } catch (e) {
      console.warn('Sonner: Error unmounting Toaster:', e);
    }
    toasterRoot = null;
  }

  if (toasterContainer !== null && toasterContainer.parentNode) {
    toasterContainer.remove();
    toasterContainer = null;
  }
}

// Initialize Sonner when the script loads
initializeSonner();

// Handle hot module reloading (Vite dev mode)
if (import.meta.hot) {
  import.meta.hot.dispose(() => {
    cleanupSonner();
  });
}

// Expose Sonner API globally (with namespace collision detection)
if (typeof window !== 'undefined') {
  // Check if Sonner already exists (reload scenario)
  if (window.Sonner && typeof window.Sonner._cleanup === 'function') {
    window.Sonner._cleanup();
  }

  window.Sonner = {
    /**
     * Initialize Sonner callbacks with a DotNetObjectReference
     * Called from C# during service initialization
     * @param {DotNetObjectReference} serviceRef - Reference to the SonnerService instance
     */
    initializeCallbacks: function(serviceRef) {
      sonnerServiceRef = serviceRef;
    },

    /**
     * Invoke a registered callback via C# callback mechanism
     * @param {string} callbackId - ID of the callback to invoke
     */
    invokeCallback: async function(callbackId) {
      if (sonnerServiceRef) {
        try {
          await sonnerServiceRef.invokeMethodAsync('InvokeCallback', callbackId);
        } catch (error) {
          console.error('Sonner: Error invoking callback:', error);
        }
      } else {
        console.warn('Sonner: Service reference not initialized. Call InitializeSonnerCallbacks first.');
      }
    },

    /**
     * Show a toast notification
     * @param {string} message - The message to display
     * @param {object} options - Optional configuration (includes action: { label, callbackId })
     * @returns {string|number} Toast ID
     */
    show: function(message, options) {
      options = options || {};
      const toastOptions = { icon: React.createElement(Info, { size: 20 }), ...options };
      if (options.action) {
        toastOptions.action = {
          label: options.action.label,
          onClick: () => this.invokeCallback(options.action.callbackId)
        };
        delete toastOptions.action.callbackId;
      }
      return toast(message, toastOptions);
    },

    /**
     * Show a success toast
     * @param {string} message - The message to display
     * @param {object} options - Optional configuration (includes action: { label, callbackId })
     * @returns {string|number} Toast ID
     */
    success: function(message, options) {
      options = options || {};
      const toastOptions = { icon: React.createElement(CheckCircle, { size: 20 }), ...options };
      if (options.action) {
        toastOptions.action = {
          label: options.action.label,
          onClick: () => this.invokeCallback(options.action.callbackId)
        };
        delete toastOptions.action.callbackId;
      }
      return toast.success(message, toastOptions);
    },

    /**
     * Show an error toast
     * @param {string} message - The message to display
     * @param {object} options - Optional configuration (includes action: { label, callbackId })
     * @returns {string|number} Toast ID
     */
    error: function(message, options) {
      options = options || {};
      const toastOptions = { icon: React.createElement(XCircle, { size: 20 }), ...options };
      if (options.action) {
        toastOptions.action = {
          label: options.action.label,
          onClick: () => this.invokeCallback(options.action.callbackId)
        };
        delete toastOptions.action.callbackId;
      }
      return toast.error(message, toastOptions);
    },

    /**
     * Show a warning toast
     * @param {string} message - The message to display
     * @param {object} options - Optional configuration (includes action: { label, callbackId })
     * @returns {string|number} Toast ID
     */
    warning: function(message, options) {
      options = options || {};
      const toastOptions = { icon: React.createElement(AlertTriangle, { size: 20 }), ...options };
      if (options.action) {
        toastOptions.action = {
          label: options.action.label,
          onClick: () => this.invokeCallback(options.action.callbackId)
        };
        delete toastOptions.action.callbackId;
      }
      return toast(message, toastOptions);
    },

    /**
     * Show a promise-based toast
     * @param {Promise} promise - The promise to track
     * @param {object} messages - Object with 'loading', 'success', and 'error' messages
     * @param {object} options - Optional configuration
     * @returns {Promise} The original promise
     */
    promise: function(promise, messages, options = {}) {
      return toast.promise(promise, messages, options);
    },

    /**
     * Dismiss a specific toast or all toasts
     * @param {string|number} toastId - Optional toast ID to dismiss
     */
    dismiss: function(toastId) {
      if (toastId !== undefined) {
        toast.dismiss(toastId);
      } else {
        toast.dismiss();
      }
    },

    /**
     * Show a toast that hosts a Blazor component.
     * @param {string} componentIdentifier - Component identifier for Blazor.rootComponents.add
     * @param {string} fragmentId - ID for the registered fragment in .NET
     * @param {object} options - Optional toast configuration
     * @returns {Promise<string>} Toast ID as string
     */
    showComponent: async function(componentIdentifier, fragmentId, options) {
      const containerId = `sonner-component-${fragmentId}`;
      const toastOptions = { ...(options || {}) };
      const originalOnDismiss = toastOptions.onDismiss;

      // For headless/custom toasts, remove default toast styling so component styling shows
      if (!toastOptions.style) {
        toastOptions.style = {};
      }
      toastOptions.style.border = 'none';
      toastOptions.style.background = 'transparent';

      const BlazorHost = createBlazorHost(containerId);
      let toastId;

      const cleanup = async (toastId) => {
        const root = componentRoots.get(toastId);
        if (root) {
          try {
            await root.dispose();
          } catch (e) {
            console.warn('Sonner: Error disposing Blazor root component:', e);
          }
          componentRoots.delete(toastId);
        }
        const el = document.getElementById(containerId);
        if (el) {
          el.remove();
        }
        if (typeof originalOnDismiss === 'function') {
          try {
            originalOnDismiss();
          } catch (e) {
            console.warn('Sonner: Error in onDismiss handler:', e);
          }
        }
      };

      toastOptions.onDismiss = () => cleanup(toastId);

      try {
        const container = createHiddenContainer(containerId, 'sonner-blazor-toast', fragmentId);
        if (!window.Blazor || !window.Blazor.rootComponents) {
          console.error('Sonner: Blazor.rootComponents is not available.');
          container.remove();
          return null;
        }
        let root = null;
        try {
          root = await window.Blazor.rootComponents.add(container, componentIdentifier, { fragmentId });
        } catch (e) {
          if (typeof componentIdentifier === 'string' && componentIdentifier.includes('::')) {
            const fallbackIdentifier = componentIdentifier.split('::').pop();
            root = await window.Blazor.rootComponents.add(container, fallbackIdentifier, { fragmentId });
          } else {
            throw e;
          }
        }
        toastId = toast.custom(() => React.createElement(BlazorHost), toastOptions);
        componentRoots.set(toastId, root);
        return String(toastId);
      } catch (error) {
        console.error('Sonner: Error mounting Blazor component toast:', error);
        const existing = document.getElementById(containerId);
        if (existing) {
          existing.remove();
        }
        return null;
      }
    },

    /**
     * Show a toast using Sonner's default structure, mounting a Blazor component
     * into the title slot so default styling is preserved.
     * @param {string} componentIdentifier - Component identifier for Blazor.rootComponents.add
     * @param {string} fragmentId - ID for the registered fragment in .NET
     * @param {object} options - Optional toast configuration
     * @returns {Promise<string|null>} Toast ID as string, or null on failure
     */
    showComponentDefault: async function(componentIdentifier, fragmentId, options) {
      const slotId = `sonner-slot-${fragmentId}`;
      const toastOptions = { ...(options || {}) };
      const originalOnDismiss = toastOptions.onDismiss;
      const BlazorHost = createBlazorHost(slotId);
      let toastId;

      const cleanup = async (toastId) => {
        const root = componentRoots.get(toastId);
        if (root) {
          try {
            await root.dispose();
          } catch (e) {
            console.warn('Sonner: Error disposing Blazor root component:', e);
          }
          componentRoots.delete(toastId);
        }
        const el = document.getElementById(slotId);
        if (el) {
          el.remove();
        }
        if (typeof originalOnDismiss === 'function') {
          try {
            originalOnDismiss();
          } catch (e) {
            console.warn('Sonner: Error in onDismiss handler:', e);
          }
        }
      };

      toastOptions.onDismiss = () => cleanup(toastId);

      try {
        const container = createHiddenContainer(slotId);
        if (!window.Blazor || !window.Blazor.rootComponents) {
          console.error('Sonner: Blazor.rootComponents is not available.');
          container.remove();
          return null;
        }
        let root = null;
        try {
          root = await window.Blazor.rootComponents.add(container, componentIdentifier, { fragmentId });
        } catch (e) {
          if (typeof componentIdentifier === 'string' && componentIdentifier.includes('::')) {
            const fallbackIdentifier = componentIdentifier.split('::').pop();
            root = await window.Blazor.rootComponents.add(container, fallbackIdentifier, { fragmentId });
          } else {
            throw e;
          }
        }
        toastId = toast(React.createElement(BlazorHost), toastOptions);
        componentRoots.set(toastId, root);
        return String(toastId);
      } catch (error) {
        console.error('Sonner: Error mounting Blazor component toast:', error);
        const existing = document.getElementById(slotId);
        if (existing) {
          existing.remove();
        }
        return null;
      }
    },

    /**
     * Create a promise toast and return a promise ID for later resolution
     * @param {string} loadingMsg - Message to show while loading
     * @param {string} successMsg - Message to show on success
     * @param {string} errorMsg - Message to show on error
     * @returns {string} Promise ID
     */
    createPromiseToast: function(loadingMsg, successMsg, errorMsg) {
      const id = String(promiseIdCounter++);
      let resolve, reject;

      const promise = new Promise((res, rej) => {
        resolve = res;
        reject = rej;
      });

      promiseRegistry.set(id, { resolve, reject });

      toast.promise(promise, {
        loading: loadingMsg,
        success: successMsg,
        error: errorMsg
      });

      return id;
    },

    /**
     * Resolve a promise toast by ID
     * @param {string} promiseId - The promise ID returned from createPromiseToast
     * @param {any} result - The result value
     */
    resolvePromiseToast: function(promiseId, result) {
      const entry = promiseRegistry.get(promiseId);
      if (entry) {
        entry.resolve(result);
        promiseRegistry.delete(promiseId);
      }
    },

    /**
     * Reject a promise toast by ID
     * @param {string} promiseId - The promise ID returned from createPromiseToast
     * @param {string} errorMsg - The error message
     */
    rejectPromiseToast: function(promiseId, errorMsg) {
      const entry = promiseRegistry.get(promiseId);
      if (entry) {
        entry.reject(new Error(errorMsg));
        promiseRegistry.delete(promiseId);
      }
    },

    /**
     * Internal cleanup function (called during reload)
     * @private
     */
    _cleanup: cleanupSonner
  };

  /**
   * Initialize Sonner callbacks (called from C# SonnerService.EnsureInitializedAsync)
   * @param {DotNetObjectReference} serviceRef - Reference to the SonnerService instance
   */
  window.InitializeSonnerCallbacks = function(serviceRef) {
    if (window.Sonner) {
      window.Sonner.initializeCallbacks(serviceRef);
    }
  };
}
