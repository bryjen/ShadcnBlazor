window.shadcnMarkdownHighlight = {
    highlightAll: () => {
        if (window.hljs && typeof window.hljs.highlightAll === "function") {
            window.hljs.highlightAll();
        }
    }
};
