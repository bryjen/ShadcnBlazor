window.shadcnDocsOnThisPage = (function () {
    const observers = new Map();

    function disconnect(instanceId) {
        const observer = observers.get(instanceId);
        if (observer) {
            observer.disconnect();
            observers.delete(instanceId);
        }
    }

    return {
        observe: (instanceId, sectionIds, dotNetRef) => {
            disconnect(instanceId);

            const elements = sectionIds
                .map(id => document.getElementById(id))
                .filter(el => el != null);

            if (elements.length === 0) return;

            const visible = new Map();

            const observer = new IntersectionObserver(
                (entries) => {
                    for (const entry of entries) {
                        if (entry.isIntersecting) {
                            visible.set(entry.target.id, entry.boundingClientRect);
                        } else {
                            visible.delete(entry.target.id);
                        }
                    }

                    if (visible.size === 0) return;

                    const sorted = [...visible.entries()]
                        .sort((a, b) => a[1].top - b[1].top);
                    const activeId = sorted[0][0];
                    dotNetRef.invokeMethodAsync("SetActiveSection", activeId);
                },
                {
                    root: null,
                    rootMargin: "-80px 0px -70% 0px",
                    threshold: 0
                }
            );

            elements.forEach(el => observer.observe(el));
            observers.set(instanceId, observer);
        },
        disconnect
    };
})();
