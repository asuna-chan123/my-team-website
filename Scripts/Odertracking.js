// chức năng chuyển đổi tab
document.addEventListener('DOMContentLoaded', function () {
    const tabs = document.querySelectorAll('.tab');

    tabs.forEach(tab => {
        tab.addEventListener('click', function () {
            // xóa class active khỏi tất cả tab
            tabs.forEach(t => {
                t.classList.remove('active');
                t.setAttribute('aria-selected', 'false');
            });
            // thêm class active vào tab được click
            this.classList.add('active');
            this.setAttribute('aria-selected', 'true');
            // tùy chọn: thêm logic cho nội dung tab khác nhau
            const tabText = this.textContent.trim();
            console.log('tab được click:', tabText);
            // ví dụ: có thể hiển thị/ẩn nội dung khác nhau dựa trên tab
            //if (tabText === 'Online') {
            //    // xử lý đơn hàng online
            //    console.log('hiển thị đơn hàng online');
            //} else if (tabText === 'In-store') {
            //    // xử lý đơn hàng tại cửa hàng
            //    console.log('hiển thị đơn hàng tại cửa hàng');
            //}
        });
    });
});

// trạng thái active của menu điều hướng phụ
//document.addEventListener('DOMContentLoaded', function () {
//    const subNavLinks = document.querySelectorAll('.sub-nav__link');
//    subNavLinks.forEach(link => {
//        link.addEventListener('click', function (e) {
//            // bỏ comment nếu muốn ngăn hành động mặc định của link
//            // e.preventDefault();
//            // xóa class active khỏi tất cả link
//            subNavLinks.forEach(l => {
//                l.classList.remove('sub-nav__link--active');
//            });
//            // thêm class active vào link được click
//            this.classList.add('sub-nav__link--active');
//        });
//    });
//});