// Minimal site script - focused on loaded state and search dropdown

// Add 'loaded' class shortly after load to enable header animations (if any)
window.addEventListener('load', () => {
    setTimeout(() => document.body.classList.add('loaded'), 3500); // Updated to 3500ms
});

// Search dropdown toggle (mobile)
const searchToggle = document.getElementById('searchToggle');
const searchDropdown = document.getElementById('searchDropdown');
if (searchToggle && searchDropdown) {
    searchToggle.addEventListener('click', (e) => {
        e.stopPropagation();
        const isVisible = searchDropdown.classList.contains('search-visible');
        searchDropdown.classList.toggle('search-visible', !isVisible);
        searchDropdown.classList.toggle('search-hidden', isVisible);
    });

    // Close when clicking outside
    document.addEventListener('click', (e) => {
        if (!searchDropdown.contains(e.target) && !searchToggle.contains(e.target)) {
            searchDropdown.classList.remove('search-visible');
            searchDropdown.classList.add('search-hidden');
        }
    });

    // Close on Escape
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            searchDropdown.classList.remove('search-visible');
            searchDropdown.classList.add('search-hidden');
        }
    });
}

// Mobile menu toggle
const mobileMenuToggle = document.getElementById('mobileMenuToggle');
const mobileMenu = document.getElementById('mobileMenu');
if (mobileMenuToggle && mobileMenu) {
    mobileMenuToggle.addEventListener('click', (e) => {
        const isOpen = mobileMenu.classList.contains('open');
        mobileMenu.classList.toggle('open', !isOpen);
        mobileMenu.setAttribute('aria-hidden', String(isOpen));
        mobileMenuToggle.setAttribute('aria-expanded', String(!isOpen));
    });

    // Close mobile menu when clicking outside
    document.addEventListener('click', (e) => {
        if (!mobileMenu.contains(e.target) && !mobileMenuToggle.contains(e.target)) {
            mobileMenu.classList.remove('open');
            mobileMenu.setAttribute('aria-hidden', 'true');
            mobileMenuToggle.setAttribute('aria-expanded', 'false');
        }
    });

    // Close on Escape
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            mobileMenu.classList.remove('open');
            mobileMenu.setAttribute('aria-hidden', 'true');
            mobileMenuToggle.setAttribute('aria-expanded', 'false');
        }
    });
}

// Mobile menu close button
const mobileMenuClose = document.getElementById('mobileMenuClose');
if (mobileMenuClose && mobileMenu) {
    mobileMenuClose.addEventListener('click', (e) => {
        mobileMenu.classList.remove('open');
        mobileMenu.setAttribute('aria-hidden', 'true');
        if (mobileMenuToggle) mobileMenuToggle.setAttribute('aria-expanded', 'false');
    });
}