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
