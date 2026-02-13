window.shadcnMarkdownEditor = {
  getTextAreaInfo: (id) => {
    const el = document.getElementById(id);
    if (!el) {
      return null;
    }

    return {
      value: el.value ?? "",
      start: el.selectionStart ?? 0,
      end: el.selectionEnd ?? 0
    };
  },
  setTextAreaValueAndSelection: (id, value, start, end) => {
    const el = document.getElementById(id);
    if (!el) {
      return;
    }

    el.value = value ?? "";
    const safeStart = Math.max(0, Math.min(start ?? 0, el.value.length));
    const safeEnd = Math.max(0, Math.min(end ?? safeStart, el.value.length));
    el.setSelectionRange(safeStart, safeEnd);
    el.focus();
  },
  getTextAreaHeight: (id) => {
    const el = document.getElementById(id);
    if (!el) {
      return 140;
    }

    return el.clientHeight || 140;
  }
};
