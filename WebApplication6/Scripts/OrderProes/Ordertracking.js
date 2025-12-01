
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
            
        });
    });
});

