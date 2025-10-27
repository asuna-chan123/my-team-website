// LOADING - khi tải trang
document.addEventListener("DOMContentLoaded", () => {
    const startTime = Date.now();
    window.addEventListener("load", () => {
        const elapsed = Date.now() - startTime;
        const remaining = Math.max(0, 3500 - elapsed);
        setTimeout(() => {
            window.scrollTo({ top: 0, left: 0 });
            document.body.classList.add("loaded");
        }, remaining);
    });
});


//
const orderBtn = document.getElementById('orderBtn');       // Nút "Đặt hàng"
const orderPanel = document.getElementById('orderPanel');   // Bảng đặt hàng
const closeOrder = document.getElementById('closeOrder');   // Nút đóng bảng đặt hàng
const orderForm = document.getElementById('orderForm');     // Form đặt hàng

// mở đóng bảng đặt hàng 
function openOrder() {
    // Mở bảng đặt hàng bằng cách thêm class 'open'
    orderPanel.classList.add('open');
    // cập nhật ARIA để bảng đặt hàng mở
    orderPanel.setAttribute('aria-hidden', 'false');
}

function closeOrderPanel() {
    // Đóng bảng đặt hàng bằng cách xóa class 'open'
    orderPanel.classList.remove('open');
    // cập nhật ARIA để bảng đặt hàng đóng
    orderPanel.setAttribute('aria-hidden', 'true');
}

// CHỨC NĂNG DI CHUYỂN NÚT ĐẶT HÀNG - Animation chuyển đổi nút
function moveButtonIntoPanel() {
    // Kiểm tra nút chưa được di chuyển vào panel
    if (!orderBtn.classList.contains('in-panel')) {
        // Thêm class để đánh dấu nút đang ở trong panel
        orderBtn.classList.add('in-panel');
        // Đổi text nút thành "Trở lại"
        orderBtn.textContent = 'Trở lại';
        // Di chuyển nút vào trong panel trong DOM
        orderPanel.appendChild(orderBtn);
        // Xóa mở bảng đặt hàng và thêm đóng bảng đặt hàng
        orderBtn.removeEventListener('click', doOpenPanel);
        orderBtn.addEventListener('click', doClosePanel);
    }
}

function moveButtonOutOfPanel() {
    // Kiểm tra nút đang ở trong bảng đặt hàng
    if (orderBtn.classList.contains('in-panel')) {
        // Xóa class đánh dấu
        orderBtn.classList.remove('in-panel');
        // Đổi text nút về "Đặt hàng"
        orderBtn.textContent = 'Đặt hàng';
        // Di chuyển nút trở lại vị trí ban đầu
        document.body.appendChild(orderBtn);
        // xóa đóng panel và thêm lại mở panel
        orderBtn.removeEventListener('click', doClosePanel);
        orderBtn.addEventListener('click', doOpenPanel);
    }
}

// Mở đóng bảng đặt hàng chính 
function doOpenPanel() {
    // Mở bảng đặt hàng bằng cách thêm class 'open'
    orderPanel.classList.add('open');
    // Cập nhật ARIA 
    orderPanel.setAttribute('aria-hidden', 'false');
    // Kích hoạt hiệu ứng các layer trượt vào
    animateLayersIn();
    // Di chuyển nút vào trong bảng đặt hàng
    moveButtonIntoPanel();
    // Thêm class vào body để đánh dấu bảng đặt hàng đang mở
    document.body.classList.add('order-open');
}

function doClosePanel() {
    // Đóng bảng đặt hàng bằng cách xóa class 'open'
    orderPanel.classList.remove('open');
    // Cập nhật ARIA 
    orderPanel.setAttribute('aria-hidden', 'true');
    // Kích hoạt hiệu ứng các bảng đặt hàng trượt ra
    animateLayersOut();
    // Di chuyển nút ra khỏi bảng đặt hàng
    moveButtonOutOfPanel();
    // Xóa class khỏi body
    document.body.classList.remove('order-open');
}


// Gắn tính năng click cho nút đặt hàng
orderBtn.addEventListener('click', doOpenPanel);
// Gắn tính năng click cho nút đóng
closeOrder.addEventListener('click', doClosePanel);

// CHỨC NĂNG ĐÓNG KHI CLICK BÊN NGOÀI - Click outside to close
document.addEventListener('click', (e) => {
    // Kiểm tra click có nằm ngoài bảng đặt hàng và nút đặt hàng không
    // Và bảng đặt hàng đang mở
    if (!orderPanel.contains(e.target) &&
        !orderBtn.contains(e.target) &&
        orderPanel.classList.contains('open')) {
        // Đóng bảng đặt hàng nếu click bên ngoài
        doClosePanel();
    }
});

// CHỨC NĂNG XỬ LÝ FORM ĐẶT HÀNG - Form submission
orderForm.addEventListener('submit', (e) => {
    // Ngăn chặn hành vi đặt hàng mặc định của form
    e.preventDefault();
    // Lấy dữ liệu từ form
    const data = new FormData(orderForm);
    const name = data.get('name');    // Tên sản phẩm
    const size = data.get('size');    // Kích thước
    const color = data.get('color');  // Màu sắc
    // Hiển thị thông báo đặt hàng thành công
    alert(`Đã thêm: ${name} - Size: ${size} - Màu: ${color}`);
    // Đóng bảng đặt hàng sau khi đặt hàng
    doClosePanel();
});

// CHỨC NĂNG CHỌN SIZE VÀ MÀU - Tile selection logic
// Lấy tất cả các ô chọn size và màu
const sizeTiles = document.querySelectorAll('.size-grid .option-tile');
const colorTiles = document.querySelectorAll('.color-grid .option-tile');
// Lấy các input ẩn để lưu giá trị được chọn
const selectedSize = document.getElementById('selectedSize');
const selectedColor = document.getElementById('selectedColor');

// Hàm chung để xử lý việc chọn ô (tile)
function selectTile(tiles, hiddenInput, tile) {
    // Xóa class 'selected' / 'đã chọn' khỏi tất cả các ô
    tiles.forEach(t => t.classList.remove('selected'));
    // Thêm class 'selected' / 'đã chọn' vào ô được chọn
    tile.classList.add('selected');
    // Lưu giá trị vào input ẩn
    hiddenInput.value = tile.getAttribute('data-value');
}

// Gắn sự kiện click cho các ô chọn size
sizeTiles.forEach(tile => {
    tile.addEventListener('click', () => selectTile(sizeTiles, selectedSize, tile));
});

// Gắn sự kiện click cho các ô chọn màu
colorTiles.forEach(tile => {
    tile.addEventListener('click', () => selectTile(colorTiles, selectedColor, tile));
});


// CHỨC NĂNG HIỆU ỨNG LAYER - Stagger animation cho các panel
const layers = document.querySelectorAll('.panel-layer');

// Hiệu ứng các layer trượt vào (từ phải sang trái)
function animateLayersIn() {
    layers.forEach((l, i) => {
        // Mỗi layer xuất hiện sau layer trước 80ms
        setTimeout(() => { l.style.right = '0'; }, i * 80);
    });
}

// Hiệu ứng các layer trượt ra (từ trái sang phải)
function animateLayersOut() {
    layers.forEach((l, i) => {
        // Mỗi layer biến mất sau layer trước 60ms
        setTimeout(() => { l.style.right = '100%'; }, i * 60);
    });
}


// Tránh lặp animation
// Các control cần đánh dấu đã chạy animation
const controlsToMark = [
    document.getElementById('Nameboost'),
    document.getElementById('orderPriceBox'),
    document.getElementById('orderBtn')
];

// Đánh dấu animation đã chạy khi kết thúc
controlsToMark.forEach(el => {
    if (!el) return; // Bỏ qua nếu element không tồn tại
    el.addEventListener('animationend', () => {
        // Thêm class để đánh dấu đã chạy animation
        el.classList.add('has-animated');
    }, { once: true }); // Chỉ chạy một lần
});

// Đảm bảo không chạy lại animation khi resize
window.addEventListener('resize', () => {
    controlsToMark.forEach(el => {
        if (!el) return;
        // Thêm class ngay lập tức để tránh animation lặp lại
        el.classList.add('has-animated');
    });
});

//thu nhỏ nút đặt hàng khi scroll
(function () {
    let ticking = false; // Flag để tối ưu performance
    const threshold = 48; // Số pixel scroll trước khi thu nhỏ controls

    function update() {
        ticking = false;
        // Không thu nhỏ controls khi panel đặt hàng đang mở
        if (document.body.classList.contains('order-open')) {
            document.body.classList.remove('scrolled-controls');
            return;
        }

        // Thu nhỏ controls khi scroll xuống quá threshold
        if (window.scrollY > threshold) {
            document.body.classList.add('scrolled-controls');
        } else {
            document.body.classList.remove('scrolled-controls');
        }
    }

    // sự kiện scroll với requestAnimationFrame để tối ưu performance
    window.addEventListener('scroll', () => {
        if (!ticking) {
            window.requestAnimationFrame(() => update());
            ticking = true;
        }
    }, { passive: true }); // passive: true để tối ưu performance

    // Khởi tạo trạng thái khi trang load xong
    document.addEventListener('DOMContentLoaded', () => {
        if (window.scrollY > threshold) {
            document.body.classList.add('scrolled-controls');
        }
    });
})();

//HIỆU ỨNG BENTO CARDS
(function () {
    const bentoSection = document.getElementById('bento-section');
    if (!bentoSection) return; // Thoát nếu không có bento section

    // Kiểm tra user có muốn giảm animation không
    const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (prefersReduced) {
        // Hiển thị ngay lập tức không có animation nếu user muốn giảm motion
        document.querySelectorAll('.bento-card').forEach(c => c.classList.add('bento-visible'));
        return;
    }

    const grid = bentoSection.querySelector('.grid-features');
    const cards = Array.from(bentoSection.querySelectorAll('.bento-card'));

    // Tạo Intersection Observer để theo dõi khi bento section xuất hiện
    const io = new IntersectionObserver((entries, obs) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                // Thêm class animate vào grid để kích hoạt stagger delays
                if (grid) grid.classList.add('bento-animate');

                // Hiển thị từng card với delay khác nhau để tạo hiệu ứng cascade
                cards.forEach((c, i) => {
                    // Timeout nhỏ để đảm bảo transition-delay được áp dụng
                    setTimeout(() => c.classList.add('bento-visible'), i * 60);
                });

                // Ngắt observer sau khi đã kích hoạt animation
                obs.disconnect();
            }
        });
    }, {
        threshold: 0.12 // Kích hoạt khi 12% của section xuất hiện
    });

    // Bắt đầu observe bento section
    io.observe(bentoSection);
})();