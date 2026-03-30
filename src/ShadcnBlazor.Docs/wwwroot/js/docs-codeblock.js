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
    },
    applyLineHighlights: (preId, overlayId, linesSpec, lineCount) => {
        const pre = document.getElementById(preId);
        if (!pre) return;
        const overlay = document.getElementById(overlayId);
        if (!overlay) return;

        while (overlay.firstChild) overlay.removeChild(overlay.firstChild);
        if (!linesSpec || !lineCount || lineCount <= 0) return;

        const style = window.getComputedStyle(pre);
        const fontSize = parseFloat(style.fontSize) || 12;
        let lineHeight = parseFloat(style.lineHeight);
        if (!lineHeight || Number.isNaN(lineHeight)) {
            lineHeight = fontSize * 1.65;
        } else if (lineHeight < fontSize * 1.2) {
            lineHeight = lineHeight * fontSize;
        }

        const paddingTop = parseFloat(style.paddingTop) || 0;
        const paddingBottom = parseFloat(style.paddingBottom) || 0;

        const container = overlay.parentElement;
        if (!container) return;
        const fullWidth = Math.max(container.scrollWidth, container.clientWidth);
        const fullHeight = Math.max(pre.scrollHeight + paddingTop + paddingBottom, container.scrollHeight);
        overlay.style.width = `${fullWidth}px`;
        overlay.style.height = `${fullHeight}px`;

        const parseLines = (spec, maxLine) => {
            const set = new Set();
            const parts = spec.split(/[,\s]+/).map(p => p.trim()).filter(Boolean);
            for (const part of parts) {
                if (part.includes("-")) {
                    const [aRaw, bRaw] = part.split("-").map(s => parseInt(s, 10));
                    if (!aRaw || !bRaw) continue;
                    let a = aRaw;
                    let b = bRaw;
                    if (a > b) [a, b] = [b, a];
                    for (let i = a; i <= b; i++) {
                        if (i >= 1 && i <= maxLine) set.add(i);
                    }
                } else {
                    const n = parseInt(part, 10);
                    if (n >= 1 && n <= maxLine) set.add(n);
                }
            }
            return Array.from(set).sort((x, y) => x - y);
        };

        const lines = parseLines(linesSpec, lineCount);
        for (const line of lines) {
            const el = document.createElement("div");
            el.className = "docs-codeblock-line-highlight";
            el.style.top = `${paddingTop + (line - 1) * lineHeight}px`;
            el.style.left = `0px`;
            el.style.right = `0px`;
            el.style.height = `${lineHeight}px`;
            overlay.appendChild(el);
        }
    }
};
