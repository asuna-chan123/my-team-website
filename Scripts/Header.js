// Sub Navigation Active State
document.addEventListener('DOMContentLoaded', function () {
    const subNavLinks = document.querySelectorAll('.sub-nav__link');

    subNavLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            // Chỉ prevent default nếu không phải link thực
            if (this.getAttribute('href') === '#') {
                e.preventDefault();
            }

            // Remove active class from all links
            subNavLinks.forEach(l => {
                l.classList.remove('sub-nav__link--active');
            });

            // Add active class to clicked link
            this.classList.add('sub-nav__link--active');

            // Log thông tin
            const page = this.textContent.trim();
            console.log('Navigation clicked:', page);
        });
    });
});