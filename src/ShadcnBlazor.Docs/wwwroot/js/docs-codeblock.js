window.shadcnDocsCodeblock = {
    copyToClipboard: (text) => {
        return navigator.clipboard.writeText(text);
    },
    highlightAll: () => {
        if (window.hljs && typeof window.hljs.highlightAll === "function") {
            window.hljs.highlightAll();
        }
    }
};
