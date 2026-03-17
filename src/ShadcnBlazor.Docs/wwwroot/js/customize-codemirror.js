window.shadcnCustomizeEditor = window.shadcnCustomizeEditor || (() => {
  let nextId = 1;
  const observers = new Map();

  const cleanupObserver = (id) => {
    const state = observers.get(id);
    if (!state) {
      return;
    }

    state.scroller?.removeEventListener('scroll', state.onScroll);
    state.resizeObserver?.disconnect();
    observers.delete(id);
  };

  const tryAttach = (id, host, dotNetRef) => {
    const state = observers.get(id);
    if (!state) {
      return;
    }

    const scroller = host?.querySelector('.cm-scroller');
    if (!scroller) {
      state.rafAttach = requestAnimationFrame(() => tryAttach(id, host, dotNetRef));
      return;
    }

    state.scroller = scroller;

    let rafPending = false;
    const emit = () => {
      rafPending = false;
      dotNetRef.invokeMethodAsync('OnEditorScroll', scroller.scrollTop, scroller.clientHeight);
    };

    const onScroll = () => {
      if (rafPending) {
        return;
      }

      rafPending = true;
      requestAnimationFrame(emit);
    };

    state.onScroll = onScroll;
    scroller.addEventListener('scroll', onScroll, { passive: true });

    if (window.ResizeObserver) {
      const resizeObserver = new ResizeObserver(() => onScroll());
      resizeObserver.observe(scroller);
      state.resizeObserver = resizeObserver;
    }

    onScroll();
  };

  return {
    observeScroll(host, dotNetRef) {
      if (!host || !dotNetRef) {
        return null;
      }

      const id = nextId++;
      observers.set(id, {
        host,
        dotNetRef,
        scroller: null,
        onScroll: null,
        resizeObserver: null,
        rafAttach: null,
      });

      tryAttach(id, host, dotNetRef);
      return id;
    },

    unobserveScroll(id) {
      cleanupObserver(id);
    }
  };
})();
