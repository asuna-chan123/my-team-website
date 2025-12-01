
// nút tiếp tục mua sắm
function continueShopping() {
    console.log('chuyển tới trang chủ');
    window.location.href = '/';
}

// nút theo dõi đơn hàng
function trackOrder() {
    console.log('chuyển tới trang theo dõi đơn hàng');
    window.location.href = '/Oder/Odertracking';
}

// khởi tạo khi trang tải xong
document.addEventListener('DOMContentLoaded', function () {
    console.log('trang xác nhận đơn hàng đã tải');
    const successIcon = document.querySelector('.success-icon');
    if (successIcon) {
        successIcon.style.animation = 'fadeIn 0.5s ease-in';
    }
});

// tùy chọn: thêm animation css
const style = document.createElement('style');
style.textContent = `
    @keyframes fadeIn {
        from {
            opacity: 0;
            transform: scale(0.8);
        }
        to {
            opacity: 1;
            transform: scale(1);
        }
    }
`;
document.head.appendChild(style);
