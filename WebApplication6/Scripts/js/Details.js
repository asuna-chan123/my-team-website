// Scripts/js/Details.js
(function () {
    // Chống load trùng file
    if (window.__DETAILS_INITIALIZED) return;
    window.__DETAILS_INITIALIZED = true;

    document.addEventListener("DOMContentLoaded", function () {
        // ====== DATA TỪ RAZOR ======
        const imagesByColor = window.imagesByColor || {};
        const colorNames = window.colorNames || {};
        const variantStocks = window.variantStocks || [];

        let currentColor =
            window.defaultColor ||
            Object.keys(imagesByColor)[0] ||
            null;

        let currentQty = 1;
        let currentMaxQty = 0;
        let unitPrice = 0;
        let originalUnitPrice = 0;

        // ====== DOM ELEMENTS ======
        const mainImageEl = document.getElementById("mainImage");
        const thumbContainer = document.getElementById("thumbContainer");
        const thumbPrevBtn = document.getElementById("thumbPrev");
        const thumbNextBtn = document.getElementById("thumbNext");
        const stockTextEl = document.getElementById("stockText");
        const addToCartBtn = document.querySelector(".btn-primary");

        const popup = document.getElementById("cartPopup");
        const popupImg = document.getElementById("popupImage");
        const popupName = document.getElementById("popupName");
        const popupUnitPrice = document.getElementById("popupUnitPrice");
        const popupColor = document.getElementById("popupColor");
        const popupSize = document.getElementById("popupSize");
        const popupQty = document.getElementById("popupQty");
        const popupSubtotal = document.getElementById("popupSubtotal");
        const popupGrandTotal = document.getElementById("popupGrandTotal");
        const popupOriginal = document.getElementById("popupOriginal");
        const popupItemCount = document.getElementById("popupItemCount");
        const rowOriginal = document.getElementById("rowOriginal");
        const qtyWarning = document.getElementById("qtyWarning");

        const btnQtyPlus = document.getElementById("btnQtyPlus");
        const btnQtyMinus = document.getElementById("btnQtyMinus");
        const hfColor = document.getElementById("hfColor");
        const hfSize = document.getElementById("hfSize");
        const hfQty = document.getElementById("hfQty");

        const colorOptionsRoot = document.getElementById("colorOptions");

        // ====== HELPER FUNCTIONS ======
        function getCurrentSizeName() {
            const active = document.querySelector(".size-option.active");
            return active ? active.innerText.trim() : null;
        }

        function formatVND(n) {
            return (n || 0).toLocaleString("vi-VN") + "₫";
        }

        function parseVND(text) {
            return parseInt(String(text || "").replace(/\D/g, ""), 10) || 0;
        }

        function clearChildren(el) {
            if (!el) return;
            while (el.firstChild) el.removeChild(el.firstChild);
        }

        function normalizeSmallSrc(src) {
            return src || "/Content/images/placeholder-60.png";
        }

        function normalizeLargeSrc(src) {
            if (!src) return "/Content/images/placeholder-600.png";
            if (src.includes("height=600")) return src;
            if (src.includes("height=200")) {
                return src.replace(/height=\d+&width=\d+/, "height=600&width=600");
            }
            return src;
        }

        // ====== STOCK & NÚT + / - ======
        function updateQtyButtonsState() {
            if (!btnQtyPlus || !btnQtyMinus) return;

            const disablePlus =
                currentMaxQty <= 0 || currentQty >= currentMaxQty;
            const disableMinus = currentQty <= 1;

            btnQtyPlus.disabled = disablePlus;
            btnQtyPlus.classList.toggle("qty-btn-disabled", disablePlus);

            btnQtyMinus.disabled = disableMinus;
            btnQtyMinus.classList.toggle("qty-btn-disabled", disableMinus);
        }

        function updateStockLabel() {
            if (!stockTextEl || !addToCartBtn) return;

            // reset warning
            if (qtyWarning) qtyWarning.classList.add("hidden");

            const sizeName = getCurrentSizeName();
            if (!sizeName || !currentColor) {
                stockTextEl.textContent = "Hết hàng.";
                addToCartBtn.disabled = true;
                addToCartBtn.classList.add("disabled-btn");
                currentMaxQty = 0;
                currentQty = 0;
                if (popupQty) popupQty.textContent = currentQty;
                updateQtyButtonsState();
                return;
            }

            const variant = variantStocks.find(
                v =>
                    String(v.ColorID) === String(currentColor) &&
                    v.SizeName === sizeName
            );

            if (!variant || variant.StockQty <= 0) {
                stockTextEl.textContent = "Hết hàng.";
                addToCartBtn.disabled = true;
                addToCartBtn.classList.add("disabled-btn");
                currentMaxQty = 0;
                currentQty = 0;
                if (popupQty) popupQty.textContent = currentQty;
            } else {
                stockTextEl.textContent =
                    "Còn " + variant.StockQty + " sản phẩm trong kho.";
                addToCartBtn.disabled = false;
                addToCartBtn.classList.remove("disabled-btn");
                currentMaxQty = variant.StockQty;

                // luôn reset về 1 khi phát hiện lại hàng
                if (currentQty < 1 || currentQty > currentMaxQty) {
                    currentQty = 1;
                    if (popupQty) popupQty.textContent = currentQty;
                    if (hfQty) hfQty.value = currentQty;
                }
            }

            updateQtyButtonsState();
        }

        // ====== THUMB CAROUSEL ======
        function updateThumbNavButtons() {
            if (!thumbContainer || !thumbPrevBtn || !thumbNextBtn) return;

            if (thumbContainer.scrollWidth <= thumbContainer.clientWidth + 4) {
                thumbPrevBtn.disabled = true;
                thumbNextBtn.disabled = true;
                return;
            }

            thumbPrevBtn.disabled = thumbContainer.scrollLeft <= 0;
            thumbNextBtn.disabled =
                thumbContainer.scrollLeft + thumbContainer.clientWidth >=
                thumbContainer.scrollWidth - 1;
        }

        function renderThumbsForColor(color) {
            const list = imagesByColor[color] || [];

            clearChildren(thumbContainer);

            if (!thumbContainer) return;

            if (list.length === 0) {
                const placeholder = document.createElement("img");
                placeholder.className = "thumb active";
                placeholder.src = "/Content/images/placeholder-60.png";
                placeholder.setAttribute(
                    "data-src",
                    "/Content/images/placeholder-600.png"
                );
                thumbContainer.appendChild(placeholder);

                if (mainImageEl) {
                    mainImageEl.src = "/Content/images/placeholder-600.png";
                }

                setTimeout(updateThumbNavButtons, 50);
                return;
            }

            list.forEach((src, index) => {
                const img = document.createElement("img");
                img.className = "thumb" + (index === 0 ? " active" : "");
                img.src = normalizeSmallSrc(src);
                img.setAttribute("data-src", normalizeLargeSrc(src));
                img.style.width = "125px";
                img.style.height = "125px";
                img.style.objectFit = "cover";
                img.style.padding = "3px";
                img.style.cursor = "pointer";
                thumbContainer.appendChild(img);
            });

            // đặt ảnh chính = ảnh đầu tiên
            if (mainImageEl) {
                const first = normalizeLargeSrc(list[0]);
                mainImageEl.src =
                    first +
                    (first.includes("?") ? "&" : "?") +
                    "v=" +
                    new Date().getTime();
            }

            // reset scroll
            thumbContainer.scrollLeft = 0;
            setTimeout(updateThumbNavButtons, 80);
        }

        // drag scroll an toàn cho thumb
        function enableDragScroll(track) {
            if (!track || track.__dragAttached) return;

            let isDown = false;
            let startX = 0;
            let scrollLeft = 0;
            let moved = 0;
            const threshold = 6;

            track.addEventListener(
                "pointerdown",
                function (e) {
                    isDown = true;
                    moved = 0;
                    startX = e.clientX;
                    scrollLeft = track.scrollLeft;
                    track.style.cursor = "grabbing";
                },
                { passive: true }
            );

            track.addEventListener(
                "pointermove",
                function (e) {
                    if (!isDown) return;
                    const dx = e.clientX - startX;
                    moved += Math.abs(dx);
                    if (Math.abs(dx) > threshold) {
                        track.scrollLeft = scrollLeft - dx;
                        track.__justDragged = true;
                        updateThumbNavButtons();
                    }
                },
                { passive: true }
            );

            function endDrag() {
                isDown = false;
                track.style.cursor = "grab";
                if (track.__justDragged) {
                    setTimeout(function () {
                        track.__justDragged = false;
                    }, 40);
                }
                updateThumbNavButtons();
            }

            track.addEventListener("pointerup", endDrag, { passive: true });
            track.addEventListener("pointerleave", endDrag, { passive: true });
            track.addEventListener("pointercancel", endDrag, { passive: true });

            track.style.cursor = "grab";
            track.__dragAttached = true;
        }

        // click vào thumb (event delegation)
        function attachThumbClick() {
            if (!thumbContainer || thumbContainer.__thumbClickAttached) return;

            thumbContainer.addEventListener(
                "click",
                function (e) {
                    const img = e.target.closest(".thumb");
                    if (!img) return;

                    // nếu vừa drag xong thì bỏ click
                    if (thumbContainer.__justDragged) {
                        thumbContainer.__justDragged = false;
                        return;
                    }

                    document
                        .querySelectorAll(".thumb")
                        .forEach(t => t.classList.remove("active"));
                    img.classList.add("active");

                    const src = img.getAttribute("data-src") || img.src;
                    if (mainImageEl && src) {
                        mainImageEl.src =
                            src +
                            (src.includes("?") ? "&" : "?") +
                            "v=" +
                            new Date().getTime();
                    }
                    setTimeout(updateThumbNavButtons, 50);
                },
                { passive: true }
            );

            thumbContainer.__thumbClickAttached = true;
        }

        // prev/next button
        function attachThumbNavButtons() {
            if (thumbPrevBtn) {
                thumbPrevBtn.addEventListener("click", function () {
                    if (!thumbContainer) return;
                    thumbContainer.scrollBy({
                        left: -Math.round(thumbContainer.clientWidth * 0.7),
                        behavior: "smooth"
                    });
                    setTimeout(updateThumbNavButtons, 250);
                });
            }

            if (thumbNextBtn) {
                thumbNextBtn.addEventListener("click", function () {
                    if (!thumbContainer) return;
                    thumbContainer.scrollBy({
                        left: Math.round(thumbContainer.clientWidth * 0.7),
                        behavior: "smooth"
                    });
                    setTimeout(updateThumbNavButtons, 250);
                });
            }
        }

        // ====== COLOR & SIZE ======
        function initColorOptions() {
            if (!colorOptionsRoot) return;

            colorOptionsRoot.addEventListener(
                "click",
                function (e) {
                    const option = e.target.closest(".color-option");
                    if (!option) return;

                    const color = option.getAttribute("data-color");
                    if (!color) return;

                    currentColor = color;

                    // active border
                    document
                        .querySelectorAll("#colorOptions .color-option")
                        .forEach(o => {
                            o.classList.remove("active");
                            o.style.outline = "";
                        });
                    option.classList.add("active");
                    option.style.outline = "2px solid #000";

                    // đổi ảnh
                    renderThumbsForColor(color);

                    // đổi tên màu
                    const name =
                        option.getAttribute("data-name") ||
                        colorNames[color] ||
                        "Màu " + color;
                    const nameEl = document.getElementById("selectedColorName");
                    if (nameEl) nameEl.textContent = name;

                    // cập nhật tồn kho
                    updateStockLabel();
                },
                { passive: true }
            );
        }

        function initSizeOptions() {
            const sizeBtns = document.querySelectorAll(".size-option");
            sizeBtns.forEach(btn => {
                if (btn.__sizeAttached) return;

                btn.addEventListener("click", function () {
                    if (btn.disabled) return;
                    sizeBtns.forEach(b => b.classList.remove("active"));
                    btn.classList.add("active");
                    updateStockLabel();
                });

                btn.__sizeAttached = true;
            });
        }

        // ====== CART POPUP ======
        function updateCartSummary() {
            const subtotal = unitPrice * currentQty;

            if (popupSubtotal)
                popupSubtotal.textContent = formatVND(subtotal);
            if (popupGrandTotal)
                popupGrandTotal.textContent = formatVND(subtotal);

            if (originalUnitPrice > unitPrice) {
                if (popupOriginal)
                    popupOriginal.textContent = formatVND(
                        originalUnitPrice * currentQty
                    );
                if (rowOriginal) rowOriginal.classList.remove("hidden");
            } else if (rowOriginal) {
                rowOriginal.classList.add("hidden");
            }

            if (popupItemCount)
                popupItemCount.textContent = currentQty + " mặt hàng";
        }

        function changeQty(delta) {
            if (qtyWarning) qtyWarning.classList.add("hidden");

            if (currentMaxQty <= 0) {
                if (qtyWarning) {
                    qtyWarning.textContent =
                        "Không thể thêm do số lượng hàng hóa không đủ.";
                    qtyWarning.classList.remove("hidden");
                }
                return;
            }

            if (delta > 0 && currentQty >= currentMaxQty) {
                if (qtyWarning) {
                    qtyWarning.textContent =
                        "Không thể thêm do số lượng hàng hóa không đủ.";
                    qtyWarning.classList.remove("hidden");
                }
                updateQtyButtonsState();
                return;
            }

            const next = currentQty + delta;
            currentQty = Math.min(
                currentMaxQty,
                Math.max(1, next)
            );

            if (popupQty) popupQty.textContent = currentQty;
            if (hfQty) hfQty.value = currentQty;

            updateQtyButtonsState();
            updateCartSummary();
        }

        // expose để dùng trong onclick HTML
        window.changeQty = changeQty;

        function openCartPopup(selectedColor, selectedSize, imageSrc) {
            const nameEl = document.querySelector(".product-title");
            const priceEl = document.querySelector(".product-price");

            const name =
                (nameEl && nameEl.textContent.trim()) || "Sản phẩm";
            const priceText = (priceEl && priceEl.textContent) || "0₫";

            unitPrice = parseVND(priceText);

            const dataOriginal =
                priceEl && priceEl.getAttribute("data-original-price");
            const originalFromAttr = dataOriginal
                ? parseInt(dataOriginal, 10)
                : 0;
            const originalFromDom = parseVND(
                document.querySelector(".product-price-original")?.textContent ||
                "0"
            );

            originalUnitPrice = Math.max(
                originalFromAttr || 0,
                originalFromDom || 0,
                unitPrice
            );

            currentQty = 1;
            if (popupImg) popupImg.src = imageSrc || "";
            if (popupName) popupName.textContent = name;
            if (popupUnitPrice) popupUnitPrice.textContent = priceText;
            if (popupColor) popupColor.textContent = selectedColor || "Không xác định";
            if (popupSize) popupSize.textContent = selectedSize || "Chưa chọn";
            if (popupQty) popupQty.textContent = currentQty;

            if (hfColor) hfColor.value = selectedColor || "";
            if (hfSize) hfSize.value = selectedSize || "";
            if (hfQty) hfQty.value = currentQty;

            updateQtyButtonsState();
            updateCartSummary();

            if (popup) {
                popup.classList.remove("hidden");
                popup.classList.add("flex");
            }
        }
        window.openCartPopup = openCartPopup;

        window.closeCartPopup = function () {
            if (popup) {
                popup.classList.add("hidden");
                popup.classList.remove("flex");
            }
        };

        window.confirmAddToCart = function () {
            const form = document.getElementById("orderForm");
            if (form) form.submit();
        };

        function initAddToCartButton() {
            if (!addToCartBtn || addToCartBtn.__attached) return;

            addToCartBtn.addEventListener("click", function () {
                const colorImg =
                    document.querySelector(".color-option.active img");
                const selectedColor =
                    (colorImg && colorImg.alt) || "Mặc định";
                const selectedSizeEl =
                    document.querySelector(".size-option.active");
                const selectedSize =
                    (selectedSizeEl && selectedSizeEl.innerText) ||
                    "Chưa chọn";
                const imageSrc =
                    (mainImageEl && mainImageEl.src) || "";

                openCartPopup(selectedColor, selectedSize, imageSrc);
            });

            addToCartBtn.__attached = true;
        }

        function initQtyButtons() {
            if (btnQtyPlus && !btnQtyPlus.__attached) {
                btnQtyPlus.addEventListener("click", function () {
                    changeQty(1);
                });
                btnQtyPlus.__attached = true;
            }

            if (btnQtyMinus && !btnQtyMinus.__attached) {
                btnQtyMinus.addEventListener("click", function () {
                    changeQty(-1);
                });
                btnQtyMinus.__attached = true;
            }
        }

        // ====== INITIALIZE ======
        renderThumbsForColor(currentColor);
        enableDragScroll(thumbContainer);
        attachThumbClick();
        attachThumbNavButtons();
        initColorOptions();
        initSizeOptions();
        initAddToCartButton();
        initQtyButtons();
        updateStockLabel();

        // set tên màu lần đầu nếu có
        const selectedColorNameEl = document.getElementById("selectedColorName");
        if (selectedColorNameEl) {
            const defaultOption =
                document.querySelector(
                    '#colorOptions .color-option[data-color="' +
                    currentColor +
                    '"]'
                ) ||
                document.querySelector("#colorOptions .color-option");

            if (defaultOption) {
                const n =
                    defaultOption.getAttribute("data-name") ||
                    colorNames[currentColor];
                if (n) selectedColorNameEl.textContent = n;
            }
        }

    });

})();

