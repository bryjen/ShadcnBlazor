let registeredHandler = null;

const COMMAND_TRIGGER_SELECTOR = "#global-command-search-trigger";

function isEditableTarget(target) {
    if (!target) {
        return false;
    }

    if (target.isContentEditable) {
        return true;
    }

    const tag = target.tagName?.toLowerCase();
    return tag === "input" || tag === "textarea" || tag === "select";
}

function clickCommandTrigger() {
    const trigger = document.querySelector(COMMAND_TRIGGER_SELECTOR);
    if (!trigger) {
        return;
    }

    trigger.click();
}

function isCtrlShiftP(event) {
    return event.ctrlKey && event.shiftKey && !event.altKey && !event.metaKey && (event.key || "").toLowerCase() === "p";
}

function isCtrlK(event) {
    return event.ctrlKey && !event.shiftKey && !event.altKey && !event.metaKey && (event.key || "").toLowerCase() === "k";
}

export function registerGlobalShortcuts() {
    if (registeredHandler) {
        return;
    }

    registeredHandler = (event) => {
        if (isEditableTarget(event.target)) {
            return;
        }

        if (isCtrlShiftP(event) || isCtrlK(event)) {
            event.preventDefault();
            event.stopPropagation();
            clickCommandTrigger();
        }
    };

    document.addEventListener("keydown", registeredHandler, true);
}

export function unregisterGlobalShortcuts() {
    if (!registeredHandler) {
        return;
    }

    document.removeEventListener("keydown", registeredHandler, true);
    registeredHandler = null;
}
