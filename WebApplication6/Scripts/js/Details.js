// Details.js - pure JS (no Razor). Assumes window.imagesByColor & window.defaultColor exist
(function () {
    if (window.__DETAILS_INITIALIZED) return;
    window.__DETAILS_INITIALIZED = true;

    document.addEventListener('DOMContentLoaded', function () {
        const imagesByColor = window.imagesByColor || {};
        let currentColor = window.defaultColor || Object.keys(imagesByColor)[0] || 'default';
        const variantStocks = window.variantStocks || [];

        function getCurrentSizeName() {
            const btn = document.querySelector('.size-option.active');
            return btn ? btn.innerText.trim() : null;
        }
        function formatVND(n) { return (n || 0).toLocaleString('vi-VN') + '₫'; }
        function parseVND(t) { return parseInt(String(t || '').replace(/\D/g, '')) || 0; }

        function normalizeSmallSrc(src) { if (!src) return '/Content/images/placeholder-60.png'; return src; }
        function normalizeLargeSrc(src) {
            if (!src) return '/Content/images/placeholder-600.png';
            if (src.includes('height=600')) return src;
            if (src.includes('height=200')) return src.replace(/height=\d+&width=\d+/, 'height=600&width=600');
            return src;
        }

        const mainImageEl = document.getElementById('mainImage');
        const thumbContainer = document.getElementById('thumbContainer');

        function clear(el) { while (el && el.firstChild) el.removeChild(el.firstChild); }

        function updateStockLabel() {
            const stockEl = document.getElementById('stockText');
            const addBtn = document.querySelector('.btn-primary');
            const plusBtn = document.getElementById('btnQtyPlus');
            const minusBtn = document.getElementById('btnQtyMinus');
            const warningEl = document.getElementById('qtyWarning');

            if (!stockEl || !addBtn) return;

            const sizeName = getCurrentSizeName();
            if (warningEl) warningEl.classList.add('hidden');

            if (!sizeName || !currentColor) {
                stockEl.textContent = 'Hết hàng.';
                addBtn.disabled = true;
                addBtn.classList.add("disabled-btn");
                currentMaxQty = 0;

                if (plusBtn) {
                    plusBtn.disabled = true;
                    plusBtn.classList.add('qty-btn-disabled');
                }
                if (minusBtn) minusBtn.disabled = true;

                currentQty = 0;
                const qtyEl = document.getElementById('popupQty');
                if (qtyEl) qtyEl.textContent = currentQty;
                return;
            }

            const variant = variantStocks.find(v =>
                String(v.ColorID) === String(currentColor) &&
                v.SizeName === sizeName
            );

            if (!variant || variant.StockQty <= 0) {
                stockEl.textContent = 'Hết hàng.';
                addBtn.disabled = true;
                addBtn.classList.add("disabled-btn");
                currentMaxQty = 0;

                if (plusBtn) {
                    plusBtn.disabled = true;
                    plusBtn.classList.add('qty-btn-disabled');
                }
                if (minusBtn) minusBtn.disabled = true;

                currentQty = 0;
                const qtyEl = document.getElementById('popupQty');
                if (qtyEl) qtyEl.textContent = currentQty;
            } else {
                stockEl.textContent = `Còn ${variant.StockQty} sản phẩm trong kho.`;
                addBtn.disabled = false;
                addBtn.classList.remove("disabled-btn");

                currentMaxQty = variant.StockQty;

                // reset trạng thái nút +/-
                if (plusBtn) {
                    plusBtn.disabled = currentQty >= currentMaxQty;
                    plusBtn.classList.toggle('qty-btn-disabled', currentQty >= currentMaxQty);
                }
                if (minusBtn) {
                    minusBtn.disabled = currentQty <= 1;
                    minusBtn.classList.toggle('qty-btn-disabled', currentQty <= 1);
                }
            }
        }

        function updateNavButtons() {
            const track = thumbContainer;
            const prev = document.getElementById('thumbPrev');
            const next = document.getElementById('thumbNext');
            if (!track || !prev || !next) return;
            if (track.scrollWidth <= track.clientWidth + 4) { prev.disabled = true; next.disabled = true; return; }
            prev.disabled = track.scrollLeft <= 0;
            next.disabled = track.scrollLeft + track.clientWidth >= track.scrollWidth - 1;
        }

        // Render thumb elements for a color
        function renderThumbsForColor(color) {
            const list = imagesByColor[color] || [];
            clear(thumbContainer);
            if (!thumbContainer) return;
            if (!list.length) {
                const placeholder = document.createElement('img');
                placeholder.className = 'thumb';
                placeholder.src = '/Content/images/placeholder-60.png';
                placeholder.setAttribute('data-src', '/Content/images/placeholder-600.png');
                thumbContainer.appendChild(placeholder);
                if (mainImageEl) mainImageEl.src = '/Content/images/placeholder-600.png';
                setTimeout(updateNavButtons, 60);
                return;
            }

            list.forEach((src, idx) => {
                const im = document.createElement('img');
                im.className = 'thumb' + (idx === 0 ? ' active' : '');
                im.src = normalizeSmallSrc(src);
                im.setAttribute('data-src', normalizeLargeSrc(src));
                im.style.width = '125px';
                im.style.height = '125px';
                im.style.objectFit = 'cover';
                im.style.padding = '3px';
                im.style.cursor = 'pointer';
                thumbContainer.appendChild(im);
            });

            // set main image to first
            if (mainImageEl) {
                const first = list[0];
                mainImageEl.src = normalizeLargeSrc(first) + '?v=' + new Date().getTime();
            }
            setTimeout(updateNavButtons, 120);
        }

        // Event delegation for thumbnail clicks (single listener)
        if (thumbContainer && mainImageEl) {
            // remove existing marker-listener if present
            if (!thumbContainer.__thumbClickAttached) {
                thumbContainer.addEventListener('click', function (e) {
                    const img = e.target.closest('.thumb');
                    if (!img) return;
                    // if the user was dragging, we may want to ignore the click (handled by moved flag below)
                    if (thumbContainer.__justDragged) {
                        // small timeout to avoid immediate clicks after drag
                        thumbContainer.__justDragged = false;
                        return;
                    }
                    document.querySelectorAll('.thumb').forEach(t => t.classList.remove('active'));
                    img.classList.add('active');
                    const src = img.getAttribute('data-src') || img.src;
                    if (src && mainImageEl) {
                        mainImageEl.src = src + (src.includes('?') ? '&' : '?') + 'v=' + new Date().getTime();
                    }
                    setTimeout(updateNavButtons, 60);
                }, { passive: true });
                thumbContainer.__thumbClickAttached = true;
            }
        }

        // Drag-to-scroll with threshold to avoid eating clicks
        // Replace existing enableDragScroll / enableDragScrollSafe with this version
        function enableDragScrollSafe(track) {
            if (!track) return;
            if (track.__dragAttached) return;
            let isDown = false, startX = 0, scrollLeft = 0, moved = 0;
            const threshold = 6;

            track.addEventListener('pointerdown', function (e) {
                // don't capture pointer — capturing can cause click to be suppressed in some browsers
                isDown = true; moved = 0;
                startX = e.clientX;
                scrollLeft = track.scrollLeft;
                track.style.cursor = 'grabbing';
            }, { passive: true });

            track.addEventListener('pointermove', function (e) {
                if (!isDown) return;
                const dx = e.clientX - startX;
                moved += Math.abs(dx);
                if (Math.abs(dx) > threshold) {
                    // update scroll based on delta from start
                    track.scrollLeft = scrollLeft - dx;
                    track.__justDragged = true;
                    updateNavButtons();
                }
            }, { passive: true });

            track.addEventListener('pointerup', function (e) {
                isDown = false;
                track.style.cursor = 'grab';
                if (track.__justDragged) {
                    setTimeout(() => { track.__justDragged = false; }, 40);
                }
                updateNavButtons();
            }, { passive: true });

            track.addEventListener('pointercancel', function () { isDown = false; track.style.cursor = 'grab'; updateNavButtons(); }, { passive: true });
            track.addEventListener('pointerleave', function () { isDown = false; updateNavButtons(); }, { passive: true });

            track.style.cursor = 'grab';
            track.__dragAttached = true;
        }

        enableDragScrollSafe(thumbContainer);

        // prev/next buttons
        document.getElementById('thumbPrev')?.addEventListener('click', function () {
            if (!thumbContainer) return;
            thumbContainer.scrollBy({ left: -Math.round(thumbContainer.clientWidth * 0.7), behavior: 'smooth' });
            setTimeout(updateNavButtons, 250);
        });
        document.getElementById('thumbNext')?.addEventListener('click', function () {
            if (!thumbContainer) return;
            thumbContainer.scrollBy({ left: Math.round(thumbContainer.clientWidth * 0.7), behavior: 'smooth' });
            setTimeout(updateNavButtons, 250);
        });

        // color swatches logic (delegation)
        const colorOptionsRoot = document.getElementById('colorOptions');
        if (colorOptionsRoot) {
            colorOptionsRoot.addEventListener('click', function (e) {
                const option = e.target.closest('.color-option');
                if (!option) return;

                const color = option.getAttribute('data-color');
                if (!color) return;

                currentColor = color; // cập nhật màu hiện tại

                // set active cho màu
                document.querySelectorAll('#colorOptions .color-option').forEach(o => {
                    o.classList.remove('active');
                    o.style.outline = '';
                });
                option.classList.add('active');
                option.style.outline = '2px solid #000';

                // đổi ảnh
                renderThumbsForColor(color);

                // cập nhật tên màu
                const name = option.getAttribute('data-name') || `Màu ${color}`;
                const colorNameEl = document.getElementById('selectedColorName');
                if (colorNameEl) {
                    colorNameEl.textContent = name;
                }

                // 🔥 QUAN TRỌNG: cập nhật tồn kho theo color + size đang chọn
                updateStockLabel();

            }, { passive: true });
        }

        // size buttons
        document.querySelectorAll('.size-option').forEach(btn => {
            if (!btn.__sizeAttached) {
                btn.addEventListener('click', function () {
                    if (btn.disabled) return;
                    document.querySelectorAll('.size-option').forEach(b => b.classList.remove('active'));
                    btn.classList.add('active');

                    // cập nhật tồn kho khi đổi size
                    updateStockLabel();
                });
                btn.__sizeAttached = true;
            }
        });


        // popup cart logic (kept as-is)
        let currentQty = 1, unitPrice = 0, originalUnitPrice = 0;
        let currentMaxQty = 1;   // tối đa cho variant hiện tại
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
        function changeQty(change) {
            const qtyEl = document.getElementById('popupQty');
            const hfQty = document.getElementById('hfQty');
            const plusBtn = document.getElementById('btnQtyPlus');
            const minusBtn = document.getElementById('btnQtyMinus');
            const warningEl = document.getElementById('qtyWarning');

            if (warningEl) warningEl.classList.add('hidden');

            // ====== Nếu hết hàng ======
            if (currentMaxQty <= 0) {
                if (warningEl) {
                    warningEl.textContent = 'Không thể thêm do số lượng hàng hóa không đủ.';
                    warningEl.classList.remove('hidden');
                }
                return;
            }

            // ====== Nếu bấm + mà đã đạt max ======
            if (change > 0 && currentQty >= currentMaxQty) {
                if (warningEl) {
                    warningEl.textContent = 'Không thể thêm do số lượng hàng hóa không đủ.';
                    warningEl.classList.remove('hidden');
                }
                if (plusBtn) {
                    plusBtn.disabled = true;
                    plusBtn.classList.add('qty-btn-disabled');
                }
                return;
            }

            // ====== Tính số lượng ======
            const nextQty = currentQty + change;
            currentQty = Math.min(currentMaxQty, Math.max(1, nextQty));

            // cập nhật UI
            if (qtyEl) qtyEl.textContent = currentQty;
            if (hfQty) hfQty.value = currentQty;

            // ====== Cập nhật trạng thái nút + ======
            if (plusBtn) {
                const disablePlus = currentQty >= currentMaxQty;
                plusBtn.disabled = disablePlus;
                plusBtn.classList.toggle('qty-btn-disabled', disablePlus);
            }

            // ====== Cập nhật trạng thái nút – ======
            if (minusBtn) {
                const disableMinus = currentQty <= 1;
                minusBtn.disabled = disableMinus;
                minusBtn.classList.toggle('qty-btn-disabled', disableMinus);
            }

            updateCartSummary();
        }
        window.changeQty = changeQty;

        window.changeQty = changeQty;


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

            // LATER FIX: Populate hidden form fields (Điền dữ liệu vào form ẩn để chuẩn bị submit)
            document.getElementById('hfColor') && (document.getElementById('hfColor').value = selectedColor || '');
            document.getElementById('hfSize') && (document.getElementById('hfSize').value = selectedSize || '');
            document.getElementById('hfQty') && (document.getElementById('hfQty').value = currentQty);

            // Cập nhật ảnh cho form //fix
            if (document.getElementById('hfImagePro')) {
                // Chỉ lấy tên file nếu imageSrc là đường dẫn đầy đủ, hoặc để nguyên tùy logic controller
                // Ở đây ta cứ gán nguyên, Controller sẽ xử lý hoặc dùng variant lookup
                // Tuy nhiên, để khớp với DB thường chỉ lưu tên file, ta có thể cắt chuỗi nếu cần.
                // Nhưng tốt nhất là để Controller tự tra cứu theo Color/Size.
                // Dòng này để đáp ứng yêu cầu "truyền ảnh đang chọn"
                var filename = imageSrc.substring(imageSrc.lastIndexOf('/') + 1);
                document.getElementById('hfImagePro').value = filename;
            }

            updateCartSummary();
            const popup = document.getElementById('cartPopup'); if (popup) { popup.classList.remove('hidden'); popup.classList.add('flex'); }
        }
        window.openCartPopup = openCartPopup;
        window.closeCartPopup = function () { const popup = document.getElementById('cartPopup'); if (popup) { popup.classList.add('hidden'); popup.classList.remove('flex'); } };
        window.confirmAddToCart = function () {
            // LATER FIX: Submit form #orderForm thay vì chuyển trang
            document.getElementById("orderForm").submit();
        };

        const addToCartBtn = document.querySelector('.btn-primary');
        if (addToCartBtn && !addToCartBtn.__attached) {
            addToCartBtn.addEventListener('click', function () {
                const selectedColor = document.querySelector('.color-option.active img')?.alt || 'Mặc định';
                const selectedSize = document.querySelector('.size-option.active')?.innerText || 'Chưa chọn';
                const imageSrc = document.getElementById('mainImage')?.src || '';
                openCartPopup(selectedColor, selectedSize, imageSrc);
            });
            addToCartBtn.__attached = true;
        }

        // set tên màu lần đầu theo alt của color đầu tiên
        // initial render
        // initial render
        renderThumbsForColor(currentColor);
        updateStockLabel();


        // set tên màu lần đầu theo color-option đầu tiên
        const firstColorNameEl = document.getElementById('selectedColorName');
        if (firstColorNameEl) {
            // tìm color-option tương ứng với defaultColor
            const firstOption = document.querySelector(`#colorOptions .color-option[data-color="${currentColor}"]`)
                || document.querySelector('#colorOptions .color-option');

            if (firstOption) {
                const initialName = firstOption.getAttribute('data-name');
                if (initialName) {
                    firstColorNameEl.textContent = initialName;
                }
            }
        }




    }); // DOMContentLoaded end
})(); // IIFE end



