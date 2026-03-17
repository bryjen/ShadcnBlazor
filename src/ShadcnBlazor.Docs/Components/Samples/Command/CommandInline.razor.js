export function ensureSelectedVisible(containerId, itemId) {
  if (!containerId || !itemId) {
    return;
  }

  const container = document.getElementById(containerId);
  const item = document.getElementById(itemId);
  if (!container || !item) {
    return;
  }

  const containerRect = container.getBoundingClientRect();
  const itemRect = item.getBoundingClientRect();

  const overTop = itemRect.top < containerRect.top;
  const overBottom = itemRect.bottom > containerRect.bottom;

  if (overTop) {
    container.scrollTop -= containerRect.top - itemRect.top;
  } else if (overBottom) {
    container.scrollTop += itemRect.bottom - containerRect.bottom;
  }
}
