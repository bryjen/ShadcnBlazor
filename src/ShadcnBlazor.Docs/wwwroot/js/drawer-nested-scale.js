// Drawer nested scaling utilities
(function() {
  // Find drawer content element - look for Vaul drawer containers
  function findDrawerContent(drawerId) {
    // Vaul renders drawers with specific Vaul CSS classes
    // Find the drawer content div that has the vaul-content class or similar

    // First try: look for divs with the drawer container ID structure
    let element = document.querySelector(`[id*="${drawerId}"]`);

    if (!element || !element.querySelector('[role="dialog"]')) {
      // Second try: find all Vaul drawer divs and get the second-to-last one
      // (last one is the nested, second-to-last is the parent)
      const allDrawers = Array.from(document.querySelectorAll('[role="dialog"], [data-vaul-blazor-host]'));
      if (allDrawers.length >= 2) {
        // Return the element before the last (should be parent)
        element = allDrawers[allDrawers.length - 2];
      } else if (allDrawers.length === 1) {
        element = allDrawers[0];
      }
    }

    if (!element && element.querySelector('[role="dialog"]')) {
      element = element.querySelector('[role="dialog"]');
    }

    return element;
  }

  // Apply scaling and animation to a drawer
  window.Vaul = window.Vaul || {};

  window.Vaul.applyNestedScaling = function(parentDrawerId) {
    // Give the DOM a moment to render the drawer
    setTimeout(() => {
      const element = findDrawerContent(parentDrawerId);
      if (!element) {
        console.warn('Vaul: Could not find drawer element for scaling', parentDrawerId);
        return;
      }

      // Add animation class and apply transform
      element.style.transition = 'transform 0.3s cubic-bezier(0.32, 0.72, 0, 1)';
      element.style.transformOrigin = 'center bottom';
      element.style.transform = 'scale(0.95)';
      element.style.borderRadius = '12px';
    }, 50);
  };

  window.Vaul.removeNestedScaling = function(parentDrawerId) {
    const element = findDrawerContent(parentDrawerId);
    if (!element) {
      console.warn('Vaul: Could not find drawer element for unscaling', parentDrawerId);
      return;
    }

    // Remove transform with animation
    element.style.transition = 'transform 0.3s cubic-bezier(0.32, 0.72, 0, 1)';
    element.style.transform = 'scale(1)';

    // Clean up after animation completes
    setTimeout(() => {
      element.style.transition = '';
      element.style.transformOrigin = '';
      element.style.borderRadius = '';
    }, 300);
  };
})();
