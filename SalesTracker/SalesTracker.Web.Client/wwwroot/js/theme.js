// Theme initialization script for Bootstrap dark mode support
function initializeTheme() {
    alert('init theme');
    const preferredTheme = getPreferredTheme();
    setTheme(preferredTheme);
}

function getPreferredTheme() {
    console.log(window.matchMedia('(prefers-color-scheme: dark)'));

    // Check the system preference
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
        return 'dark';
    }

    // Default to light
    return 'light';
}

function setTheme(theme) {
    const html = document.documentElement;
    html.setAttribute('data-bs-theme', theme);
}


// Listen for system theme changes and update accordingly
if (window.matchMedia) {
    const darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)');
    console.log('matches', darkModeQuery);
    darkModeQuery.addEventListener('change', (e) => {
        // Only update if user hasn't set a manual preference
        if (!localStorage.getItem('data-bs-theme')) {
            setTheme(e.matches ? 'dark' : 'light');
        }
    });
}

// Initialize theme on script load
initializeTheme();
