(function () {
    if (!window.shadcnTheme) {
        window.shadcnTheme = {};
    }

    window.shadcnTheme.setVar = function (name, value) {
        document.documentElement.style.setProperty(name, value);
    };

    window.shadcnTheme.setVars = function (vars) {
        if (!vars) {
            return;
        }

        Object.keys(vars).forEach(function (key) {
            const value = vars[key];
            if (value !== null && value !== undefined && value !== '') {
                document.documentElement.style.setProperty(key, value);
            }
        });
    };

    window.shadcnTheme.injectStylesheet = function (url) {
        if (!url) {
            return;
        }

        if (document.querySelector('link[href="' + url + '"]')) {
            return;
        }

        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = url;
        document.head.appendChild(link);
    };

    window.shadcnTheme.setPrimary = function (hex) {
        window.shadcnTheme.setVar('--primary', hex);
    };
})();
