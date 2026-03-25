window.shadcnDocsCodeblock = {
    ensureCommandLanguage: () => {
        if (!window.hljs || typeof window.hljs.registerLanguage !== "function") return;
        if (window.hljs.getLanguage && window.hljs.getLanguage("command")) return;
        window.hljs.registerLanguage("command", (hljs) => ({
            name: "command",
            contains: [
                { className: "meta", begin: /^\s*\$?\s*\S+/, relevance: 10 },
                { className: "keyword", begin: /--?[\w-]+/ },
                { className: "string", begin: /"[^"\n]*"|'[^'\n]*'/ },
                { className: "number", begin: /\b\d+\b/ },
                { className: "variable", begin: /\$[\w_]+/ },
                { className: "symbol", begin: /[./~][\w./-]+/ }
            ]
        }));
    },
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
        window.shadcnDocsCodeblock.ensureCommandLanguage();
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
