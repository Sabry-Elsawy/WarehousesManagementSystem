// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        // Check if the link matches the current path
        // We use startsWith or includes depending on strictness. 
        // For now, exact match or simple inclusion if not root.
        if (href && href !== '#' && (currentPath === href.toLowerCase() || (href !== '/' && currentPath.startsWith(href.toLowerCase())))) {
            link.classList.add('active');
            
            // If it's a nested link, expand the parent
            const parentCollapse = link.closest('.collapse');
            if (parentCollapse) {
                // Bootstrap 5 collapse show class
                parentCollapse.classList.add('show');
                
                // Update the toggle button state
                const parentToggle = document.querySelector(`[data-bs-target="#${parentCollapse.id}"]`);
                if (parentToggle) {
                    parentToggle.classList.remove('collapsed');
                    parentToggle.setAttribute('aria-expanded', 'true');
                    parentToggle.classList.add('active'); // Optional: highlight parent too
                }
            }
        }
    });
});
