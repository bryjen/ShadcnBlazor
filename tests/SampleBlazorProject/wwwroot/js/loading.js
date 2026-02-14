// Health quotes array
const healthQuotes = [
    "Early detection increases cancer survival rates by up to 90%",
    "Tracking sleep patterns can predict illness 2-3 days before symptoms appear",
    "Small daily habits compound into 37x health improvements over a year",
    "Regular symptom tracking helps doctors diagnose 40% faster",
    "Catching high blood pressure early prevents 80% of heart complications",
    "Your body shows warning signs 3-7 days before most illnesses",
    "Preventative care costs 10x less than treating advanced conditions",
    "Patterns in resting heart rate can indicate infection before fever starts",
    "Early intervention reduces diabetes complications by 60%",
    "Tracking hydration improves cognitive performance by 14%",
    "Small changes today prevent 75% of chronic diseases tomorrow",
    "Your body temperature fluctuates predictably when illness is brewing",
    "Consistent health data reveals patterns invisible to occasional checkups",
    "Monitoring stress levels can prevent 70% of tension headaches",
    "Early treatment of infections reduces recovery time by 50%",
    "Regular tracking helps identify food sensitivities before they worsen",
    "Preventative screenings detect 95% of issues while still treatable",
    "Your body's patterns are unique—tracking reveals your personal baselines",
    "Catching vitamin deficiencies early prevents months of fatigue",
    "AI can spot health trends humans miss in daily symptom data",
    "Proactive care adds an average of 10 healthy years to lifespan",
    "Monitoring sleep quality predicts immune system strength",
    "Small symptoms cluster into patterns that reveal root causes",
    "Early detection of mental health changes improves treatment outcomes by 80%",
    "Your health data tells a story—tracking helps you read it"
];

// Randomly select and display a quote on page load
(function() {
    const quoteEl = document.querySelector('.loading-quote');
    if (quoteEl) {
        const randomQuote = healthQuotes[Math.floor(Math.random() * healthQuotes.length)];
        quoteEl.textContent = `Did you know? ${randomQuote}`;
    }
})();

// Update loading text and percentage from Blazor's CSS variables
function updateLoadingProgress() {
    const percentage = getComputedStyle(document.documentElement).getPropertyValue('--blazor-load-percentage').trim() || '0%';
    const statusText = getComputedStyle(document.documentElement).getPropertyValue('--blazor-load-percentage-text').trim() || 'Initializing Diagnostics';
    
    const statusEl = document.querySelector('.loading-status-text');
    const progressBar = document.querySelector('.loading-progress-bar');
    
    // Update status text
    if (statusEl) {
        statusEl.textContent = statusText;
    }
    
    // Update progress bar width
    if (progressBar) {
        let widthValue = percentage;
        if (widthValue && !widthValue.includes('%')) {
            widthValue = widthValue + '%';
        }
        progressBar.style.width = widthValue;
    }
}

// Update on load and periodically
updateLoadingProgress();
const intervalId = setInterval(updateLoadingProgress, 50);

// Hide loading overlay after Blazor loads + 1 second delay
let blazorLoaded = false;
const checkBlazorLoaded = setInterval(() => {
    const app = document.getElementById('app');
    const overlay = document.getElementById('blazor-loading-overlay');
    
    // Check if Blazor has loaded (when #app content changes from loading screen)
    if (app && app.querySelector('main') === null && !blazorLoaded) {
        blazorLoaded = true;
        clearInterval(intervalId);
        clearInterval(checkBlazorLoaded);
        
        // Manually set progress to 100% when Blazor loads
        const statusEl = document.querySelector('.loading-status-text');
        const progressBar = document.querySelector('.loading-progress-bar');
        
        if (progressBar) {
            progressBar.style.width = '100%';
        }
        if (statusEl) {
            statusEl.textContent = 'Ready';
        }
        
        // Wait 1 second, then fade out the overlay
        setTimeout(() => {
            if (overlay) {
                overlay.style.transition = 'opacity 0.5s ease-out';
                overlay.style.opacity = '0';
                setTimeout(() => {
                    overlay.style.display = 'none';
                }, 500);
            }
        }, 1000);
    }
}, 100);
