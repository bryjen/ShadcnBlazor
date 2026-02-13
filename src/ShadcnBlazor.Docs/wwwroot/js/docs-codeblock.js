window.shadcnDocsCodeblock = {
    copyToClipboard: (text) => {
        return navigator.clipboard.writeText(text);
    },
    highlightAll: () => {
        if (window.hljs && typeof window.hljs.highlightAll === "function") {
            window.hljs.highlightAll();
        }
    },
    highlightElement: (elementOrId) => {
        const element = typeof elementOrId === "string"
            ? document.getElementById(elementOrId)
            : elementOrId;
        if (!element || !window.hljs || typeof window.hljs.highlightElement !== "function") return;
        const lang = (element.getAttribute("class") || "").match(/language-(\S+)/)?.[1];
        if (lang && (lang === "razor" || lang === "cshtml-razor")) {
            const languages = window.hljs.listLanguages ? window.hljs.listLanguages() : [];
            const hasRazor = languages.includes("razor") || languages.includes("cshtml-razor");
            if (!hasRazor) {
                element.classList.remove("language-razor", "language-cshtml-razor");
                element.classList.add("language-html");
            }
        }
        window.hljs.highlightElement(element);
    }
};
