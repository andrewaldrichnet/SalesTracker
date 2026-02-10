// Theme initialization script for Bootstrap dark mode support (MAUI)
function initializeTheme() {
    const preferredTheme = getPreferredTheme();
    setTheme(preferredTheme);
}

function getPreferredTheme() {
    // Check if user has a saved preference in localStorage
    const savedTheme = localStorage.getItem('data-bs-theme');
    if (savedTheme) {
        return savedTheme;
    }

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
    localStorage.setItem('data-bs-theme', theme);
}

function toggleTheme() {
    const currentTheme = document.documentElement.getAttribute('data-bs-theme') || 'light';
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    setTheme(newTheme);
}

// Listen for system theme changes and update accordingly
if (window.matchMedia) {
    const darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)');
    darkModeQuery.addEventListener('change', (e) => {
        // Only update if user hasn't set a manual preference
        if (!localStorage.getItem('data-bs-theme')) {
            setTheme(e.matches ? 'dark' : 'light');
        }
    });
}

// Initialize theme on script load
initializeTheme();
