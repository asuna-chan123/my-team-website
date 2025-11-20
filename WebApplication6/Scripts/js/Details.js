// Details.js - pure JS (no Razor). Assumes window.imagesByColor & window.defaultColor exist
(function () {
    if (window.__DETAILS_INITIALIZED) return;
    window.__DETAILS_INITIALIZED = true;

    document.addEventListener('DOMContentLoaded', function () {
        const imagesByColor = window.imagesByColor || {};
        let currentColor = window.defaultColor || Object.keys(imagesByColor)[0] || 'default';

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
            // attach once
            if (!colorOptionsRoot.__swatchAttached) {
                colorOptionsRoot.addEventListener('click', function (e) {
                    const option = e.target.closest('.color-option');
                    if (!option) return;
                    const color = option.getAttribute('data-color');
                    if (!color) return;
                    currentColor = color;
                    document.querySelectorAll('#colorOptions .color-option').forEach(o => { o.classList.remove('active'); o.style.outline = ''; });
                    option.classList.add('active'); option.style.outline = '2px solid #000';
                    renderThumbsForColor(color);
                    const colorNameEl = document.querySelector('.color-name'); if (colorNameEl) colorNameEl.textContent = color;
                }, { passive: true });
                colorOptionsRoot.__swatchAttached = true;
            }

            // If server rendered swatches are missing, render them
            const swatches = colorOptionsRoot.querySelectorAll('.color-option');
            if (!swatches || swatches.length === 0) {
                Object.keys(imagesByColor).forEach(function (color) {
                    const kv = imagesByColor[color] || [];
                    const thumb = kv[0] || '/Content/images/placeholder-60.png';
                    const div = document.createElement('div');
                    div.className = 'color-option';
                    div.dataset.color = color;
                    div.style.cursor = 'pointer';
                    div.style.textAlign = 'center';
                    div.style.marginRight = '8px';
                    const img = document.createElement('img');
                    img.src = thumb;
                    img.style.width = '75px';
                    img.style.height = '75px';
                    img.style.objectFit = 'cover';
                    img.style.padding = '4px';
                    const lbl = document.createElement('div');
                    lbl.style.fontSize = '12px';
                    lbl.innerText = color;
                    div.appendChild(img);
                    div.appendChild(lbl);
                    colorOptionsRoot.appendChild(div);
                });
            }
        }

        // size buttons
        document.querySelectorAll('.size-option').forEach(btn => {
            if (!btn.__sizeAttached) {
                btn.addEventListener('click', function () {
                    if (btn.disabled) return;
                    document.querySelectorAll('.size-option').forEach(b => b.classList.remove('active'));
                    btn.classList.add('active');
                });
                btn.__sizeAttached = true;
            }
        });

        // popup cart logic (kept as-is)
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
        if (addToCartBtn && !addToCartBtn.__attached) {
            addToCartBtn.addEventListener('click', function () {
                const selectedColor = document.querySelector('.color-option.active img')?.alt || 'Mặc định';
                const selectedSize = document.querySelector('.size-option.active')?.innerText || 'Chưa chọn';
                const imageSrc = document.getElementById('mainImage')?.src || '';
                openCartPopup(selectedColor, selectedSize, imageSrc);
            });
            addToCartBtn.__attached = true;
        }

        // initial render
        renderThumbsForColor(currentColor);

    }); // DOMContentLoaded end
})(); // IIFE end
