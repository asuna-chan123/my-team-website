// Cart Management
document.addEventListener('DOMContentLoaded', function () {

    // ============ XÓA SẢN PHẨM KHỎI GIỎ HÀNG ============
    const removeButtons = document.querySelectorAll('.remove-btn');
    const cartItems = document.querySelectorAll('.cart-item');

    removeButtons.forEach((btn, index) => {
        btn.addEventListener('click', function () {
            // Xác nhận trước khi xóa
            if (confirm('Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?')) {
                const cartItem = this.closest('.cart-item');

                // Animation fade out
                cartItem.style.transition = 'opacity 0.3s ease, transform 0.3s ease';
                cartItem.style.opacity = '0';
                cartItem.style.transform = 'translateX(-20px)';

                // Xóa sau khi animation xong
                setTimeout(() => {
                    cartItem.remove();
                    updateCartSummary();
                }, 300);
            }
        });
    });

    // ============ CẬP NHẬT TỔNG GIỎ HÀNG ============
    function updateCartSummary() {
        const remainingItems = document.querySelectorAll('.cart-item');
        const itemCount = remainingItems.length;

        // Cập nhật số lượng sản phẩm
        const countElement = document.querySelector('main > div:nth-child(2) p:first-child');
        if (countElement) {
            countElement.textContent = `Tổng Cộng(${itemCount} sản phẩm)`;
        }

        // Tính tổng tiền
        let total = 0;
        remainingItems.forEach(item => {
            const priceText = item.querySelector('.product-info strong').textContent;
            const price = parseInt(priceText.replace(/[^\d]/g, ''));
            const quantity = parseInt(item.querySelector('.product-quantity').value);
            total += price * quantity;
        });

        // Cập nhật tổng tiền header
        const totalHeaderElement = document.querySelector('main > div:nth-child(2) p:last-child');
        if (totalHeaderElement) {
            totalHeaderElement.textContent = formatPrice(total);
        }

        // Cập nhật order summary
        const itemPriceElement = document.querySelector('.item-price');
        if (itemPriceElement) {
            itemPriceElement.textContent = formatPrice(total);
        }

        const totalPriceElement = document.querySelector('.total-price');
        if (totalPriceElement) {
            totalPriceElement.textContent = formatPrice(total);
        }

        // Cập nhật thuế (7.4%)
        const tax = Math.round(total * 0.074);
        const taxNoteElement = document.querySelector('.tax-note');
        if (taxNoteElement) {
            taxNoteElement.textContent = `(Đã bao gồm thuế ${formatPrice(tax)})`;
        }

        // Cập nhật số lượng trong order summary
        const itemLabelElement = document.querySelector('.item-label');
        if (itemLabelElement) {
            itemLabelElement.textContent = `${itemCount} các sản phẩm`;
        }

        // Nếu giỏ hàng trống
        if (itemCount === 0) {
            showEmptyCart();
        }
    }

    // Format giá tiền
    function formatPrice(price) {
        return price.toLocaleString('vi-VN') + '₫';
    }

    // Hiển thị giỏ hàng trống
    function showEmptyCart() {
        const main = document.querySelector('main');
        const emptyMessage = document.createElement('div');
        emptyMessage.style.gridColumn = '1 / 2';
        emptyMessage.style.textAlign = 'center';
        emptyMessage.style.padding = '60px 20px';
        emptyMessage.innerHTML = `
            <i class="fa-solid fa-cart-shopping" style="font-size: 80px; color: #ddd; margin-bottom: 20px;"></i>
            <h3 style="font-size: 24px; margin-bottom: 10px;">Giỏ hàng của bạn đang trống</h3>
            <p style="color: #767677; margin-bottom: 30px;">Hãy thêm sản phẩm vào giỏ hàng để tiếp tục mua sắm</p>
            <button onclick="window.location.href='/'" style="background: #000; color: #fff; border: none; padding: 15px 40px; font-weight: 700; cursor: pointer;">TIẾP TỤC MUA SẮM</button>
        `;

        // Xóa tất cả cart items
        document.querySelectorAll('.cart-item').forEach(item => item.remove());
        document.querySelector('main > div:nth-child(3)').after(emptyMessage);
    }

    // ============ CẬP NHẬT SỐ LƯỢNG ============
    const quantitySelects = document.querySelectorAll('.product-quantity');
    quantitySelects.forEach(select => {
        select.addEventListener('change', function () {
            updateCartSummary();
        });
    });

    // ============ WISHLIST TOGGLE ============
    const wishlistButtons = document.querySelectorAll('.wishlist-btn');
    wishlistButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const icon = this.querySelector('i');
            if (icon.classList.contains('fa-regular')) {
                icon.classList.remove('fa-regular');
                icon.classList.add('fa-solid');
                icon.style.color = '#e32b2b';

                // Show notification
                showNotification('Đã thêm vào danh sách yêu thích');
            } else {
                icon.classList.remove('fa-solid');
                icon.classList.add('fa-regular');
                icon.style.color = '';

                showNotification('Đã xóa khỏi danh sách yêu thích');
            }
        });
    });

    // Hiển thị thông báo
    function showNotification(message) {
        const notification = document.createElement('div');
        notification.textContent = message;
        notification.style.cssText = `
            position: fixed;
            bottom: 30px;
            left: 50%;
            transform: translateX(-50%);
            background: #000;
            color: #fff;
            padding: 15px 30px;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 700;
            z-index: 1000;
            animation: slideUp 0.3s ease;
        `;

        document.body.appendChild(notification);

        setTimeout(() => {
            notification.style.opacity = '0';
            notification.style.transition = 'opacity 0.3s ease';
            setTimeout(() => notification.remove(), 300);
        }, 2000);
    }

    // ============ CAROUSEL SUGGESTION ============
    const suggestion = document.querySelector('.suggestion');
    const carouselNav = document.querySelector('.carousel-nav');
    const progressBar = document.querySelector('.progress-bar');

    if (suggestion && carouselNav && progressBar) {

        // Scroll khi click nút
        carouselNav.addEventListener('click', function () {
            const scrollAmount = 500;
            suggestion.scrollBy({
                left: scrollAmount,
                behavior: 'smooth'
            });
        });

        // Cập nhật progress bar khi scroll
        suggestion.addEventListener('scroll', function () {
            const scrollLeft = suggestion.scrollLeft;
            const scrollWidth = suggestion.scrollWidth - suggestion.clientWidth;
            const scrollPercent = (scrollLeft / scrollWidth) * 100;

            progressBar.style.width = scrollPercent + '%';

            // Ẩn/hiện nút next khi đến cuối
            if (scrollLeft >= scrollWidth - 10) {
                carouselNav.style.opacity = '0.3';
                carouselNav.style.cursor = 'not-allowed';
            } else {
                carouselNav.style.opacity = '1';
                carouselNav.style.cursor = 'pointer';
            }
        });

        // Auto scroll với nút trái phải bàn phím
        document.addEventListener('keydown', function (e) {
            if (e.key === 'ArrowLeft') {
                suggestion.scrollBy({
                    left: -300,
                    behavior: 'smooth'
                });
            } else if (e.key === 'ArrowRight') {
                suggestion.scrollBy({
                    left: 300,
                    behavior: 'smooth'
                });
            }
        });

        // Drag to scroll
        let isDown = false;
        let startX;
        let scrollLeftStart;

        suggestion.addEventListener('mousedown', (e) => {
            isDown = true;
            suggestion.style.cursor = 'grabbing';
            startX = e.pageX - suggestion.offsetLeft;
            scrollLeftStart = suggestion.scrollLeft;
        });

        suggestion.addEventListener('mouseleave', () => {
            isDown = false;
            suggestion.style.cursor = 'grab';
        });

        suggestion.addEventListener('mouseup', () => {
            isDown = false;
            suggestion.style.cursor = 'grab';
        });

        suggestion.addEventListener('mousemove', (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - suggestion.offsetLeft;
            const walk = (x - startX) * 2;
            suggestion.scrollLeft = scrollLeftStart - walk;
        });

        // Set cursor
        suggestion.style.cursor = 'grab';
    }

    // ============ SMOOTH SCROLL TO CHECKOUT ============
    const checkoutBtn = document.querySelector('.checkout-btn');
    if (checkoutBtn) {
        checkoutBtn.addEventListener('click', function () {
            const itemCount = document.querySelectorAll('.cart-item').length;
            if (itemCount === 0) {
                alert('Giỏ hàng của bạn đang trống!');
                return;
            }

            // Show loading
            this.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> ĐANG XỬ LÝ...';
            this.style.opacity = '0.7';

            // Simulate checkout process
            setTimeout(() => {
                alert('Chuyển đến trang thanh toán...');
                // window.location.href = '/checkout';
            }, 1000);
        });
    }

    // ============ PROMO CODE ============
    const promoBtn = document.querySelector('.promo button');
    if (promoBtn) {
        promoBtn.addEventListener('click', function () {
            const code = prompt('Nhập mã khuyến mãi:');
            if (code) {
                // Simulate promo code validation
                if (code.toUpperCase() === 'ADIDAS2024' || code.toUpperCase() === 'SALE20') {
                    showNotification('✓ Mã khuyến mãi đã được áp dụng!');
                    // Apply discount logic here
                } else {
                    showNotification('✗ Mã khuyến mãi không hợp lệ');
                }
            }
        });
    }

    // Add CSS animation
    const style = document.createElement('style');
    style.textContent = `
        @keyframes slideUp {
            from {
                transform: translate(-50%, 20px);
                opacity: 0;
            }
            to {
                transform: translate(-50%, 0);
                opacity: 1;
            }
        }
    `;
    document.head.appendChild(style);

    console.log('Adidas Cart JS loaded successfully!');
});