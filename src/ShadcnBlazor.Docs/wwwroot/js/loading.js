// Update progress bar from Blazor's CSS variables
function updateLoadingProgress() {
    const percentage = getComputedStyle(document.documentElement).getPropertyValue('--blazor-load-percentage').trim() || '0%';
    const progressBar = document.querySelector('.blazor-loader-progress-fill');

    if (progressBar) {
        let widthValue = percentage;
        if (widthValue && !widthValue.includes('%')) {
            widthValue = widthValue + '%';
        }
        progressBar.style.width = widthValue;
    }
}

updateLoadingProgress();
const intervalId = setInterval(updateLoadingProgress, 50);

// Hide loading overlay after Blazor loads + 1 second delay
let blazorLoaded = false;
const checkBlazorLoaded = setInterval(function () {
    const app = document.getElementById('app');
    const overlay = document.getElementById('blazor-loader');

    // Check if Blazor has rendered:
    // - When Blazor loads, it renders content to #app
    // - Check if the app div has child elements (Blazor rendered components)
    const blazorRendered = app && app.children.length > 0;

    // Blazor has loaded when content appears in #app
    if (blazorRendered && !blazorLoaded) {
        blazorLoaded = true;
        clearInterval(intervalId);
        clearInterval(checkBlazorLoaded);

        const progressBar = document.querySelector('.blazor-loader-progress-fill');
        if (progressBar) {
            progressBar.style.width = '100%';
        }

        // Wait 250ms, then fade out the overlay
        setTimeout(function () {
            if (overlay) {
                overlay.style.transition = 'opacity 0.6s ease-out';
                overlay.style.opacity = '0';
                setTimeout(function () {
                    overlay.remove();
                }, 1000);
            }
        }, 250);
    }
}, 100);
