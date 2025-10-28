// Tab Switching Functionality
document.addEventListener('DOMContentLoaded', function () {
    const tabs = document.querySelectorAll('.tab');

    tabs.forEach(tab => {
        tab.addEventListener('click', function () {
            // Remove active class from all tabs
            tabs.forEach(t => {
                t.classList.remove('active');
                t.setAttribute('aria-selected', 'false');
            });

            // Add active class to clicked tab
            this.classList.add('active');
            this.setAttribute('aria-selected', 'true');

            // Optional: Add your logic here for different tab content
            const tabText = this.textContent.trim();
            console.log('Tab clicked:', tabText);

            // Example: You can show/hide different content based on tab
            if (tabText === 'Online') {
                // Handle online orders
                console.log('Showing online orders');
            } else if (tabText === 'In-store') {
                // Handle in-store orders
                console.log('Showing in-store orders');
            }
        });
    });
});

// Sub Navigation Active State
document.addEventListener('DOMContentLoaded', function () {
    const subNavLinks = document.querySelectorAll('.sub-nav__link');

    subNavLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            // Uncomment below if you want to prevent default link behavior
            // e.preventDefault();

            // Remove active class from all links
            subNavLinks.forEach(l => {
                l.classList.remove('sub-nav__link--active');
            });

            // Add active class to clicked link
            this.classList.add('sub-nav__link--active');
        });
    });
});