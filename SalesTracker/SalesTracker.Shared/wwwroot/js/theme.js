console.log('theme-js loaded');

// Theme initialization script for Bootstrap dark mode support
window.initializeTheme = function () {
    console.log('init theme');
    const preferredTheme = getPreferredTheme();
    setTheme(preferredTheme);
}

function getPreferredTheme() {
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

function toggleTheme() {
    const currentTheme = document.documentElement.getAttribute('data-bs-theme') || 'light';
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    setTheme(newTheme);
}

// Initialize theme on script load
window.initializeTheme();

document.onreadystatechange = function () {
    if (document.readyState == "interactive") {
        window.initializeTheme();
        setTimeout(function () {
            window.initializeTheme();
        });
    }


    // Listen for system theme changes and update accordingly
    if (window.matchMedia) {
        const darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)');
        darkModeQuery.addEventListener('change', (e) => {
            setTheme(e.matches ? 'dark' : 'light');
        });
    }

}
