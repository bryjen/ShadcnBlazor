export function focusFirstItem(menuId) {
  const menu = document.getElementById(menuId);
  if (!menu) return;
  const items = getMenuItems(menu);
  items[0]?.focus();
}

export function focusLastItem(menuId) {
  const menu = document.getElementById(menuId);
  if (!menu) return;
  const items = getMenuItems(menu);
  items[items.length - 1]?.focus();
}

export function moveFocus(menuId, direction) {
  const menu = document.getElementById(menuId);
  if (!menu) return;
  const items = getMenuItems(menu);
  if (items.length === 0) return;
  const active = document.activeElement;
  const currentIndex = items.indexOf(active);
  if (currentIndex === -1) {
    items[0]?.focus();
    return;
  }
  const nextIndex = (currentIndex + direction + items.length) % items.length;
  items[nextIndex]?.focus();
}

function getMenuItems(menu) {
  return Array.from(
    menu.querySelectorAll('[role="menuitem"]')
  ).filter((el) => {
    if (el.getAttribute('aria-disabled') === 'true') return false;
    if (el.hasAttribute('disabled')) return false;
    return true;
  });
}
