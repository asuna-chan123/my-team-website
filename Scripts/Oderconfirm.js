// Continue Shopping Button
function continueShopping() {
    console.log('Chuyển tới trang chủ');
    // Uncomment để redirect thật
    // window.location.href = '/';
    alert('Chuyển tới trang chủ');
}

// Track Order Button
function trackOrder() {
    console.log('Chuyển tới trang theo dõi đơn hàng');
    // Uncomment để redirect thật
    // window.location.href = '/Oder/Oder';
    alert('Chuyển tới trang theo dõi đơn hàng: #ADI-2025-001234');
}

// Print Order Confirmation
function printOrder() {
    window.print();
    console.log('In đơn hàng');
}

// Share Order Confirmation
function shareOrder() {
    const orderNumber = '#ADI-2025-001234';
    const message = `Đơn hàng của tôi ${orderNumber} đã được xác nhận tại Adidas!`;

    if (navigator.share) {
        navigator.share({
            title: 'Xác nhận đơn hàng Adidas',
            text: message
        }).catch(err => console.log('Share failed:', err));
    } else {
        alert('Sao chép: ' + message);
    }
}

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    console.log('Order confirmation page loaded');

    // Optional: Add animation to success icon
    const successIcon = document.querySelector('.success-icon');
    if (successIcon) {
        successIcon.style.animation = 'fadeIn 0.5s ease-in';
    }
});

// Optional: Add CSS animation
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