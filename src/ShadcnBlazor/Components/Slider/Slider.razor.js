export function getPointerPercentage(elementId, clientX) {
    const element = document.getElementById(elementId);
    if (!element) return 0;

    const rect = element.getBoundingClientRect();
    const percent = ((clientX - rect.left) / rect.width) * 100;
    return Math.max(0, Math.min(100, percent));
}

export function setPointerCapture(elementId, pointerId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.setPointerCapture(pointerId);
    }
}

export function releasePointerCapture(elementId, pointerId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.releasePointerCapture(pointerId);
    }
}
