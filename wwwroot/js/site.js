function toggleTheme(isDarkMode) {
    console.log('toggleTheme called with isDarkMode:', isDarkMode);
    console.log('Body classList before removal:', document.body.classList);
    document.body.classList.remove('light-theme');
    document.body.classList.remove('dark-theme');
    document.body.classList.remove('sidemenu-open');
    console.log('Body classList after removal:', document.body.classList);
    if (isDarkMode) {
        document.body.classList.add('dark-theme');
        localStorage.setItem('theme', 'dark');
    } else {
        document.body.classList.add('light-theme');
        localStorage.setItem('theme', 'light');
    }
    console.log('Body classList after addition:', document.body.classList);
}

// Controleer opgeslagen thema bij het laden van de pagina
document.addEventListener('DOMContentLoaded', function() {
    const savedTheme = localStorage.getItem('theme');
    const themeToggle = document.getElementById('themeToggle');
    if (savedTheme === 'dark') {
        document.body.classList.add('dark-theme');
        if (themeToggle) themeToggle.checked = true;
    }
});

// Controleer opgeslagen thema bij het laden van de pagina
document.addEventListener('DOMContentLoaded', function() {
    const savedTheme = localStorage.getItem('theme');
    const themeToggle = document.getElementById('themeToggle');
    if (savedTheme === 'dark') {
        document.body.classList.add('dark-theme');
        if (themeToggle) themeToggle.checked = true;
    }
});