// Details.js - pure JS (no Razor). Đảm bảo view đã đưa window.imagesByColor & window.defaultColor
document.addEventListener('DOMContentLoaded', function () {
    const imagesByColor = window.imagesByColor || {};
    let currentColor = window.defaultColor || Object.keys(imagesByColor)[0] || 'default';

    function formatVND(n) { return (n || 0).toLocaleString('vi-VN') + '₫'; }
    function parseVND(t) { return parseInt(String(t || '').replace(/\D/g, '')) || 0; }

    function normalizeSmallSrc(src) {
        if (!src) return '/Content/images/placeholder-60.png';
        return src;
    }
    function normalizeLargeSrc(src) {
        if (!src) return '/Content/images/placeholder-600.png';
        if (src.includes('height=600')) return src;
        if (src.includes('height=200')) return src.replace(/height=\d+&width=\d+/, 'height=600&width=600');
        return src;
    }

    function updateNavButtons() {
        const track = document.getElementById('thumbContainer');
        const prev = document.getElementById('thumbPrev');
        const next = document.getElementById('thumbNext');
        if (!track || !prev || !next) return;
        if (track.scrollWidth <= track.clientWidth + 4) {
            prev.disabled = true; next.disabled = true; return;
        }
        prev.disabled = track.scrollLeft <= 0;
        next.disabled = track.scrollLeft + track.clientWidth >= track.scrollWidth - 1;
    }

    function enableDragScroll(track) {
        if (!track) return;
        let isDown = false, startX = 0, scrollLeft = 0;
        track.addEventListener('pointerdown', (e) => {
            isDown = true; track.setPointerCapture(e.pointerId);
            startX = e.pageX - track.offsetLeft; scrollLeft = track.scrollLeft; track.style.cursor = 'grabbing';
        });
        track.addEventListener('pointermove', (e) => {
            if (!isDown) return;
            const x = e.pageX - track.offsetLeft; const walk = (x - startX);
            track.scrollLeft = scrollLeft - walk; updateNavButtons();
        });
        track.addEventListener('pointerup', (e) => { isDown = false; try { track.releasePointerCapture(e.pointerId); } catch { } track.style.cursor = 'grab'; updateNavButtons(); });
        track.addEventListener('pointerleave', () => { isDown = false; updateNavButtons(); });
        track.style.cursor = 'grab';
    }

    function renderThumbnailsFor(color) {
        const list = imagesByColor[color] || [];
        const thumbContainer = document.getElementById('thumbContainer');
        const mainImageEl = document.getElementById('mainImage');
        if (!thumbContainer || !mainImageEl) return;
        thumbContainer.innerHTML = '';
        if (list.length === 0) { mainImageEl.src = '/Content/images/placeholder-600.png'; updateNavButtons(); return; }
        mainImageEl.src = normalizeLargeSrc(list[0]) + '?v=' + new Date().getTime();
        list.forEach(src => {
            const img = document.createElement('img');
            img.className = 'thumb'; img.src = normalizeSmallSrc(src); img.setAttribute('data-src', normalizeLargeSrc(src));
            img.style.width = '128px'; img.style.height = '128px'; img.style.objectFit = 'cover'; img.style.cursor = 'pointer'; img.style.padding = '3px';
            img.addEventListener('click', function () {
                document.querySelectorAll('.thumb').forEach(t => t.classList.remove('active'));
                img.classList.add('active');
                mainImageEl.src = this.getAttribute('data-src') + '?v=' + new Date().getTime();
            });
            thumbContainer.appendChild(img);
        });
        enableDragScroll(thumbContainer);
        setTimeout(updateNavButtons, 120);
    }

    document.getElementById('thumbPrev')?.addEventListener('click', function () {
        const track = document.getElementById('thumbContainer'); if (!track) return;
        track.scrollBy({ left: -Math.round(track.clientWidth * 0.7), behavior: 'smooth' }); setTimeout(updateNavButtons, 250);
    });
    document.getElementById('thumbNext')?.addEventListener('click', function () {
        const track = document.getElementById('thumbContainer'); if (!track) return;
        track.scrollBy({ left: Math.round(track.clientWidth * 0.7), behavior: 'smooth' }); setTimeout(updateNavButtons, 250);
    });

    const colorOptionsRoot = document.getElementById('colorOptions');
    if (colorOptionsRoot) {
        colorOptionsRoot.addEventListener('click', function (e) {
            const option = e.target.closest('.color-option'); if (!option) return;
            const color = option.getAttribute('data-color'); currentColor = color;
            document.querySelectorAll('#colorOptions .color-option').forEach(o => { o.classList.remove('active'); o.style.outline = ''; });
            option.classList.add('active'); option.style.outline = '2px solid #000';
            renderThumbnailsFor(color);
            const colorNameEl = document.querySelector('.color-name'); if (colorNameEl) colorNameEl.textContent = color;
        });
        const defaultEl = colorOptionsRoot.querySelector(`.color-option[data-color="${currentColor}"]`);
        if (defaultEl) { defaultEl.classList.add('active'); defaultEl.style.outline = '2px solid #000'; }
    }

    document.querySelectorAll('.size-option').forEach(btn => {
        btn.addEventListener('click', function () { if (btn.disabled) return; document.querySelectorAll('.size-option').forEach(b => b.classList.remove('active')); btn.classList.add('active'); });
    });

    // popup cart logic
    let currentQty = 1, unitPrice = 0, originalUnitPrice = 0;
    function updateCartSummary() {
        const subtotal = unitPrice * currentQty;
        document.getElementById('popupSubtotal') && (document.getElementById('popupSubtotal').textContent = formatVND(subtotal));
        document.getElementById('popupGrandTotal') && (document.getElementById('popupGrandTotal').textContent = formatVND(subtotal));
        const rowOriginal = document.getElementById('rowOriginal');
        if (originalUnitPrice > unitPrice) {
            document.getElementById('popupOriginal') && (document.getElementById('popupOriginal').textContent = formatVND(originalUnitPrice * currentQty));
            rowOriginal?.classList.remove('hidden');
        } else rowOriginal?.classList.add('hidden');
        document.getElementById('popupItemCount') && (document.getElementById('popupItemCount').textContent = `${currentQty} mặt hàng`);
    }
    function changeQty(change) { currentQty = Math.max(1, currentQty + change); document.getElementById('popupQty') && (document.getElementById('popupQty').textContent = currentQty); updateCartSummary(); }
    window.changeQty = changeQty;

    function openCartPopup(selectedColor, selectedSize, imageSrc) {
        const name = document.querySelector('.product-title')?.textContent?.trim() || 'Sản phẩm';
        const priceText = document.querySelector('.product-price')?.textContent || '0₫';
        unitPrice = parseVND(priceText);
        const priceEl = document.querySelector('.product-price');
        const dataOriginal = priceEl?.getAttribute('data-original-price'); const originalFromAttr = dataOriginal ? parseInt(dataOriginal, 10) : 0;
        originalUnitPrice = Math.max(originalFromAttr || 0, parseVND(document.querySelector('.product-price-original')?.textContent || '0') || 0, unitPrice);
        currentQty = 1;
        document.getElementById('popupImage') && (document.getElementById('popupImage').src = imageSrc);
        document.getElementById('popupName') && (document.getElementById('popupName').textContent = name);
        document.getElementById('popupUnitPrice') && (document.getElementById('popupUnitPrice').textContent = priceText);
        document.getElementById('popupColor') && (document.getElementById('popupColor').textContent = selectedColor || 'Không xác định');
        document.getElementById('popupSize') && (document.getElementById('popupSize').textContent = selectedSize || 'Chưa chọn');
        document.getElementById('popupQty') && (document.getElementById('popupQty').textContent = currentQty);
        updateCartSummary();
        const popup = document.getElementById('cartPopup'); if (popup) { popup.classList.remove('hidden'); popup.classList.add('flex'); }
    }
    window.openCartPopup = openCartPopup;
    window.closeCartPopup = function () { const popup = document.getElementById('cartPopup'); if (popup) { popup.classList.add('hidden'); popup.classList.remove('flex'); } };
    window.confirmAddToCart = function () { window.location.href = '/OrderProes/Order'; };

    const addToCartBtn = document.querySelector('.btn-primary');
    if (addToCartBtn) addToCartBtn.addEventListener('click', function () {
        const selectedColor = document.querySelector('.color-option.active img')?.alt || 'Mặc định';
        const selectedSize = document.querySelector('.size-option.active')?.innerText || 'Chưa chọn';
        const imageSrc = document.getElementById('mainImage')?.src || '';
        openCartPopup(selectedColor, selectedSize, imageSrc);
    });

    renderThumbnailsFor(currentColor);
});
// thêm trong DOMContentLoaded hoặc ở cuối file
(function renderGallery() {
    try {
        var imagesByColor = window.imagesByColor || {};
        var defaultColor = window.defaultColor || Object.keys(imagesByColor)[0];
        var mainImage = document.getElementById("mainImage");
        var thumbContainer = document.getElementById("thumbContainer");
        var colorOptions = document.getElementById("colorOptions");

        if (!thumbContainer) return;

        function clear(el) { while (el.firstChild) el.removeChild(el.firstChild); }

        function renderThumbsForColor(color) {
            clear(thumbContainer);
            var imgs = imagesByColor[color] || [];
            imgs.forEach(function (src) {
                var im = document.createElement("img");
                im.className = "thumb";
                im.src = src;
                im.style = "width:125px;height:125px;object-fit:cover;padding:3px;cursor:pointer;";
                im.addEventListener("click", function () { if (mainImage) mainImage.src = src; });
                thumbContainer.appendChild(im);
            });
            if (imgs.length && mainImage) mainImage.src = imgs[0];
        }

        // attach handlers to server-rendered swatches (if any) or render them
        if (colorOptions) {
            var swatches = colorOptions.querySelectorAll('.color-option');
            if (swatches && swatches.length) {
                swatches.forEach(function (el) {
                    el.addEventListener('click', function () {
                        var color = el.dataset.color;
                        renderThumbsForColor(color);
                    });
                });
            } else {
                // render swatches dynamically
                Object.keys(imagesByColor).forEach(function (color) {
                    var kv = imagesByColor[color];
                    var thumb = kv[0] || '';
                    var div = document.createElement('div');
                    div.className = 'color-option';
                    div.dataset.color = color;
                    div.style = "cursor:pointer;text-align:center;margin-right:8px;";
                    var img = document.createElement('img');
                    img.src = thumb;
                    img.style = "width:75px;height:75px;object-fit:cover;padding:4px;";
                    var lbl = document.createElement('div');
                    lbl.style = "font-size:12px";
                    lbl.innerText = color;
                    div.appendChild(img);
                    div.appendChild(lbl);
                    div.addEventListener('click', function () { renderThumbsForColor(color); });
                    colorOptions.appendChild(div);
                });
            }
        }

        // initial render
        if (defaultColor) renderThumbsForColor(defaultColor);

    } catch (e) {
        console.error("Gallery init error:", e);
    }
})();
