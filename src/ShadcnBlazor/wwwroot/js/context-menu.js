const listeners = new WeakMap();

export function initialize(triggerElement, dotNetRef) {
    const handler = (e) => {
        e.preventDefault();
        dotNetRef.invokeMethodAsync('OnContextMenu', e.clientX, e.clientY);
    };
    listeners.set(triggerElement, handler);
    triggerElement.addEventListener('contextmenu', handler);
}

export function dispose(triggerElement) {
    const handler = listeners.get(triggerElement);
    if (handler) {
        triggerElement.removeEventListener('contextmenu', handler);
        listeners.delete(triggerElement);
    }
}

function getMenuItems(menu) {
    return Array.from(
        menu.querySelectorAll('[role="menuitem"]:not([aria-disabled="true"]):not([disabled]), [role="menuitemcheckbox"]:not([aria-disabled="true"]):not([disabled]), [role="menuitemradio"]:not([aria-disabled="true"]):not([disabled])')
    );
}

export function focusMenu(menuId) {
    const menu = document.getElementById(menuId);
    if (menu) menu.focus();
}

export function focusFirstItem(menuId) {
    const menu = document.getElementById(menuId);
    if (!menu) return;
    const items = getMenuItems(menu);
    if (items.length > 0) items[0].focus();
}

export function focusLastItem(menuId) {
    const menu = document.getElementById(menuId);
    if (!menu) return;
    const items = getMenuItems(menu);
    if (items.length > 0) items[items.length - 1].focus();
}

export function moveFocus(menuId, direction) {
    const menu = document.getElementById(menuId);
    if (!menu) return;
    const items = getMenuItems(menu);
    if (items.length === 0) return;
    const active = document.activeElement;
    const currentIndex = items.indexOf(active);
    if (currentIndex === -1) {
        items[0].focus();
        return;
    }
    const nextIndex = (currentIndex + direction + items.length) % items.length;
    items[nextIndex].focus();
}

export function focusElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) element.focus();
}
