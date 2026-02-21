const THEME_STORAGE_KEY = "shadcn-docs-theme";

function setDarkMode(isDark) {
    const root = document.documentElement;
    root.classList.toggle("dark", isDark);
    root.style.colorScheme = isDark ? "dark" : "light";
}

export function initializeTheme() {
    let isDark = true;

    try {
        const saved = localStorage.getItem(THEME_STORAGE_KEY);
        if (saved === "light") {
            isDark = false;
        } else if (saved === "dark") {
            isDark = true;
        } else {
            isDark = document.documentElement.classList.contains("dark")
                || window.matchMedia("(prefers-color-scheme: dark)").matches;
        }
    } catch {
        isDark = document.documentElement.classList.contains("dark");
    }

    setDarkMode(isDark);
    return isDark;
}

export function toggleTheme() {
    const isDark = !document.documentElement.classList.contains("dark");
    setDarkMode(isDark);

    try {
        localStorage.setItem(THEME_STORAGE_KEY, isDark ? "dark" : "light");
    } catch {
        // Ignore storage failures (privacy mode / blocked storage).
    }

    return isDark;
}
