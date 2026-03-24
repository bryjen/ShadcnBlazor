import React from 'react';
import ReactDOM from 'react-dom/client';
import { Toaster, toast } from 'sonner';
import { CheckCircle, XCircle, AlertTriangle, Info } from 'lucide-react';

let toasterRoot = null;
let toasterContainer = null;
let themeObserver = null;
let sonnerServiceRef = null;

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
    position: 'top-right',
    theme: getTheme(),
    richColors: false,
    closeButton: true,
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
          await sonnerServiceRef.invokeAsyncMethod('InvokeCallback', callbackId);
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
     * Internal cleanup function (called during reload)
     * @private
     */
    _cleanup: cleanupSonner
  };

  /**
   * Initialize Sonner callbacks (called from C# SonnerService.InitializeAsync)
   * @param {DotNetObjectReference} serviceRef - Reference to the SonnerService instance
   */
  window.InitializeSonnerCallbacks = function(serviceRef) {
    if (window.Sonner) {
      window.Sonner.initializeCallbacks(serviceRef);
    }
  };
}
