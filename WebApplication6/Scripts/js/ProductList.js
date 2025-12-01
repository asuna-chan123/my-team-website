(function () {
    // Khởi tạo 1 carousel
    function initCarousel(container) {
        const wrapper = container.querySelector(".carousel-wrapper");
        const prev = container.querySelector(".nav-button.prev");
        const next = container.querySelector(".nav-button.next");

        if (!wrapper || !prev || !next) return;

        // mỗi lần bấm sẽ cuộn theo chiều rộng container
        function getStep() {
            return container.clientWidth || 50;
        }

        prev.addEventListener("click", function () {
            wrapper.scrollBy({
                left: -getStep(),
                behavior: "smooth"
            });
        });

        next.addEventListener("click", function () {
            wrapper.scrollBy({
                left: getStep(),
                behavior: "smooth"
            });
        });
    }

    // Hàm dùng cho nút banner trong Razor
    function handleClick(url) {
        if (!url) return;
        window.location.href = url;
    }
    window.handleClick = handleClick;

    // Khi trang load xong
    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll(".js-carousel").forEach(initCarousel);
    });
})();
