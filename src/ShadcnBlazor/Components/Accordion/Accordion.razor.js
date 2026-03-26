const KEY_SET = new Set([
  "Home",
  "End",
  "ArrowDown",
  "ArrowUp",
  "ArrowLeft",
  "ArrowRight",
]);

const handlerMap = new WeakMap();

function getOrientation(root) {
  return root.getAttribute("data-orientation") || "vertical";
}

function getDirection(root) {
  const dir = root.getAttribute("dir");
  if (dir === "ltr" || dir === "rtl") return dir;
  const docDir = document?.documentElement?.getAttribute("dir");
  return docDir === "rtl" ? "rtl" : "ltr";
}

function getEnabledTriggers(root) {
  return Array.from(
    root.querySelectorAll('[data-slot="accordion-trigger"]:not([disabled])')
  );
}

function onKeyDownFactory(root) {
  return function onKeyDown(event) {
    if (!KEY_SET.has(event.key)) return;

    const trigger = event.target?.closest?.('[data-slot="accordion-trigger"]');
    if (!trigger || !root.contains(trigger)) return;

    const triggers = getEnabledTriggers(root);
    if (triggers.length === 0) return;

    const currentIndex = triggers.indexOf(trigger);
    if (currentIndex === -1) return;

    event.preventDefault();

    const orientation = getOrientation(root);
    const dir = getDirection(root);
    const isLtr = dir === "ltr";

    let nextIndex = currentIndex;
    const lastIndex = triggers.length - 1;

    const moveNext = () => {
      nextIndex = currentIndex + 1;
      if (nextIndex > lastIndex) nextIndex = 0;
    };

    const movePrev = () => {
      nextIndex = currentIndex - 1;
      if (nextIndex < 0) nextIndex = lastIndex;
    };

    switch (event.key) {
      case "Home":
        nextIndex = 0;
        break;
      case "End":
        nextIndex = lastIndex;
        break;
      case "ArrowRight":
        if (orientation === "horizontal") {
          if (isLtr) moveNext();
          else movePrev();
        }
        break;
      case "ArrowDown":
        if (orientation === "vertical") moveNext();
        break;
      case "ArrowLeft":
        if (orientation === "horizontal") {
          if (isLtr) movePrev();
          else moveNext();
        }
        break;
      case "ArrowUp":
        if (orientation === "vertical") movePrev();
        break;
    }

    triggers[nextIndex]?.focus?.();
  };
}

export function initialize(root) {
  if (!root || handlerMap.has(root)) return;
  const handler = onKeyDownFactory(root);
  handlerMap.set(root, handler);
  root.addEventListener("keydown", handler);
}

export function destroy(root) {
  if (!root) return;
  const handler = handlerMap.get(root);
  if (!handler) return;
  root.removeEventListener("keydown", handler);
  handlerMap.delete(root);
}
