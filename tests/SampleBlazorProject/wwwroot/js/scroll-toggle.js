// Simple helper to enable/disable page scrolling by toggling body/html overflow,
// while compensating for the scrollbar so layout doesn't shift.
// Exposed as a global function for Blazor JS interop.

(function () {
  function setBodyScrollDisabled(disabled) {
    if (typeof document === "undefined") return;

    var body = document.body;
    var root = document.documentElement;
    if (!body || !root) return;

    if (disabled) {
      // Preserve current overflow and padding so we can restore them later.
      if (body.dataset.prevOverflow === undefined) {
        body.dataset.prevOverflow = body.style.overflow || "";
      }
      if (body.dataset.prevRootOverflow === undefined) {
        body.dataset.prevRootOverflow = root.style.overflow || "";
      }
      if (body.dataset.prevPaddingRight === undefined) {
        body.dataset.prevPaddingRight = body.style.paddingRight || "";
      }

      var scrollBarWidth = window.innerWidth - document.documentElement.clientWidth;
      var basePadding = body.dataset.prevPaddingRight || "0px";

      if (scrollBarWidth > 0) {
        body.style.paddingRight = "calc(" + basePadding + " + " + scrollBarWidth + "px)";

        // Add a fixed gutter overlay so the freed scrollbar space has a solid color.
        var existingGutter = document.getElementById("scroll-lock-gutter");
        if (!existingGutter) {
          var gutter = document.createElement("div");
          gutter.id = "scroll-lock-gutter";
          gutter.style.position = "fixed";
          gutter.style.top = "0";
          gutter.style.right = "0";
          gutter.style.bottom = "0";
          gutter.style.width = scrollBarWidth + "px";
          gutter.style.backgroundColor = "#171717";
          gutter.style.pointerEvents = "none";
          gutter.style.zIndex = "50";
          document.body.appendChild(gutter);
        } else {
          existingGutter.style.width = scrollBarWidth + "px";
        }
      }

      body.style.overflow = "hidden";
      root.style.overflow = "hidden";
    } else {
      if (body.dataset.prevOverflow !== undefined) {
        body.style.overflow = body.dataset.prevOverflow;
        delete body.dataset.prevOverflow;
      } else {
        body.style.overflow = "";
      }

      if (body.dataset.prevRootOverflow !== undefined) {
        root.style.overflow = body.dataset.prevRootOverflow;
        delete body.dataset.prevRootOverflow;
      } else {
        root.style.overflow = "";
      }

      if (body.dataset.prevPaddingRight !== undefined) {
        body.style.paddingRight = body.dataset.prevPaddingRight;
        delete body.dataset.prevPaddingRight;
      } else {
        body.style.paddingRight = "";
      }

      // Remove the gutter overlay if it exists.
      var gutter = document.getElementById("scroll-lock-gutter");
      if (gutter && gutter.parentNode) {
        gutter.parentNode.removeChild(gutter);
      }
    }
  }

  // Explicit lock/unlock for Blazor scroll lock service (ref-count in C#).
  window.lockScroll = function () {
    setBodyScrollDisabled(true);
  };
  window.unlockScroll = function () {
    setBodyScrollDisabled(false);
  };
})();
