document.addEventListener("DOMContentLoaded", function () {
    const resetBtn = document.getElementById("resetBtn");
    const resetNotice = document.getElementById("resetNotice");

    const gender = document.getElementById("gender");
    const category = document.getElementById("category");
    const priceSort = document.getElementById("priceSort");

    // show/hide helpers (giữ tăng/thu chiều cao, không đổi chiều rộng)
    function showNotice() {
        resetNotice.style.maxHeight = "56px";
        resetNotice.style.paddingTop = "8px";
        resetNotice.style.paddingBottom = "8px";
        resetNotice.style.opacity = "1";
    }
    function hideNotice() {
        resetNotice.style.maxHeight = "0";
        resetNotice.style.paddingTop = "0";
        resetNotice.style.paddingBottom = "0";
        resetNotice.style.opacity = "0";
    }

    // gán event cho Reset (vanilla JS)
    if (resetBtn) {
        resetBtn.addEventListener("click", function (e) {
            // reset value nhưng KHÔNG submit
            if (gender) gender.value = "";
            if (category) category.value = "";
            if (priceSort) priceSort.value = "";

            // hiện thông báo (form sẽ phóng to theo chiều cao)
            showNotice();

            // ẩn thông báo sau 6s (nếu muốn)
            if (resetBtn._hideTimer) clearTimeout(resetBtn._hideTimer);
            resetBtn._hideTimer = setTimeout(hideNotice, 6000);
        });
    }

    // ẩn thông báo khi user thay đổi bất kỳ filter
    [gender, category, priceSort].forEach(function (el) {
        if (el) el.addEventListener("change", hideNotice);
    });

    // --- thay thế các đoạn jQuery nhỏ: remove loader / preloader ---
    // Remove known overlays if present
    ['#loader', '.preloader', '.loading-overlay', '.spinner', '.page-mask'].forEach(selector => {
        document.querySelectorAll(selector).forEach(el => {
            try {
                if (el._loaderTimeout) clearTimeout(el._loaderTimeout);
            } catch (e) { /*ignore*/ }
            if (el && el.parentNode) el.parentNode.removeChild(el);
        });
    });

    // restore body classes/styles if present
    document.body.classList.remove('is-loading', 'preloading', 'loading');
    document.body.style.pointerEvents = '';
    document.body.style.overflow = '';
    document.body.style.filter = '';
    try { window.ontouchmove = null; window.onwheel = null; } catch (e) { /*ignore*/ }
});

document.addEventListener("DOMContentLoaded", function () {
    // existing code... (your reset/filters code)
    // ---------- gallery code ----------
    try {
        var imagesByColor = window.imagesByColor || {};
        var defaultColor = window.defaultColor || Object.keys(imagesByColor)[0];

        var mainImage = document.getElementById("mainImage");
        var thumbContainer = document.getElementById("thumbContainer");
        var colorOptions = document.getElementById("colorOptions");

        function clearChildren(el) {
            while (el && el.firstChild) el.removeChild(el.firstChild);
        }

        function renderThumbsForColor(color) {
            if (!thumbContainer) return;
            clearChildren(thumbContainer);
            var imgs = imagesByColor[color] || [];
            imgs.forEach(function (src) {
                var im = document.createElement("img");
                im.className = "thumb";
                im.src = src;
                im.style = "width:125px;height:125px;object-fit:cover;padding:3px;cursor:pointer;";
                im.addEventListener("click", function () {
                    if (mainImage) mainImage.src = src;
                });
                thumbContainer.appendChild(im);
            });
            if (imgs.length && mainImage) mainImage.src = imgs[0];
        }

        // render color swatches
        if (colorOptions) {
            // clear existing content (if you had server-rendered items)
            // keep if you prefer server-rendered color boxes
            // clearChildren(colorOptions);

            // if server already rendered color boxes it's ok — just attach click handlers
            // We'll attach click handlers to any `.color-option` elements in DOM (server rendered)
            var swatches = colorOptions.querySelectorAll('.color-option');
            if (swatches && swatches.length) {
                swatches.forEach(function (el) {
                    el.addEventListener('click', function () {
                        var color = el.dataset.color;
                        renderThumbsForColor(color);
                    });
                });
            } else {
                // render swatches from JS
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
                    div.addEventListener('click', function () {
                        renderThumbsForColor(color);
                    });
                    colorOptions.appendChild(div);
                });
            }
        }

        // initial
        if (defaultColor) renderThumbsForColor(defaultColor);
    } catch (e) {
        console.error("Gallery error:", e);
    }
});

