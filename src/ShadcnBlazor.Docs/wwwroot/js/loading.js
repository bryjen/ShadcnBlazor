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

    // Check if Blazor has replaced the static content:
    // - When Blazor loads, it replaces #app's content
    // - The static home page HTML is wrapped in a marker div, so check if Blazor changed it
    // - Blazor's App.razor will render and eventually the app will have interactive elements
    const staticMarker = document.getElementById('static-home-marker');
    const blazorRendered = app && staticMarker === null;

    // Blazor has loaded when the static marker is gone (replaced by Blazor)
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
