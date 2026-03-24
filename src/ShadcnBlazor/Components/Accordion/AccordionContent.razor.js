export function measureHeight(element) {
  if (!element) return "0px";

  const height = element.scrollHeight;
  return `${height}px`;
}
