// Hàm định dạng tiền tệ
function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN') + '₫';
}

// Hàm cập nhật tổng giá
function updateCartTotal() {
    const cartItems = document.querySelectorAll('.cart-item');
    let totalPrice = 0;
    let totalProducts = 0;

    cartItems.forEach(item => {
        const price = parseInt(item.dataset.productPrice);
        const quantitySelect = item.querySelector('.product-quantity');
        const quantity = parseInt(quantitySelect.value);

        totalPrice += price * quantity;
        totalProducts += quantity;
    });

    // Cập nhật tổng cộng ở trên cùng
    const summaryDiv = document.querySelector('main > div:nth-child(2)');
    summaryDiv.innerHTML = `
        <p>Tổng Cộng(${totalProducts} sản phẩm) </p>
        <p>${formatCurrency(totalPrice)}</p>
    `;

    // Cập nhật mục "TÓM TẮT ĐƠN HÀNG" bên phải
    const itemLabel = document.querySelector('.item-label');
    const itemPrice = document.querySelector('.item-price');
    const totalLabel = document.querySelector('.total-price');

    itemLabel.textContent = `${totalProducts} các sản phẩm`;
    itemPrice.textContent = formatCurrency(totalPrice);
    totalLabel.textContent = formatCurrency(totalPrice);

    // Cập nhật thuế (10% nếu có)
    const taxAmount = Math.round(totalPrice * 0.1);
    const taxNote = document.querySelector('.tax-note');
    taxNote.textContent = `(Đã bao gồm thuế ${formatCurrency(taxAmount)})`;
}

// Thêm sự kiện change cho tất cả dropdown số lượng
document.addEventListener('DOMContentLoaded', function () {
    const quantitySelects = document.querySelectorAll('.product-quantity');

    quantitySelects.forEach(select => {
        select.addEventListener('change', function () {
            updateCartTotal();
        });
    });

    // Xóa sản phẩm
    const removeButtons = document.querySelectorAll('.remove-btn');
    removeButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const cartItem = this.closest('.cart-item');
            cartItem.remove();
            updateCartTotal();
        });
    });

    // Cập nhật lần đầu
    updateCartTotal();
});