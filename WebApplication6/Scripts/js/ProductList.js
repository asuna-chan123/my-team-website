class Carousel {
    constructor() {
        this.carousel = document.getElementById('carousel');
        this.prevBtn = document.getElementById('prevBtn');
        this.nextBtn = document.getElementById('nextBtn');
        this.cards = document.querySelectorAll('.card');
        this.currentIndex = 0;
        this.cardWidth = 360;
        this.visibleCards = this.getVisibleCards();
        this.maxIndex = Math.max(0, this.cards.length - this.visibleCards);

        this.init();
    }

    getVisibleCards() {
        const containerWidth = document.querySelector('.carousel-container').offsetWidth;
        return Math.floor(containerWidth / this.cardWidth);
    }

    init() {
        this.prevBtn.addEventListener('click', () => this.prev());
        this.nextBtn.addEventListener('click', () => this.next());

        window.addEventListener('resize', () => {
            this.visibleCards = this.getVisibleCards();
            this.maxIndex = Math.max(0, this.cards.length - this.visibleCards);
            this.updateCarousel();
        });

        let startX = 0;
        let isDragging = false;

        this.carousel.addEventListener('touchstart', (e) => {
            startX = e.touches[0].clientX;
            isDragging = true;
        });

        this.carousel.addEventListener('touchmove', (e) => {
            if (!isDragging) return;
            e.preventDefault();
        });

        this.carousel.addEventListener('touchend', (e) => {
            if (!isDragging) return;
            isDragging = false;

            const endX = e.changedTouches[0].clientX;
            const diff = startX - endX;

            if (Math.abs(diff) > 50) {
                if (diff > 0) {
                    this.next();
                } else {
                    this.prev();
                }
            }
        });

        this.updateButtons();
    }

    prev() {
        if (this.currentIndex > 0) {
            this.currentIndex--;
            this.updateCarousel();
        }
    }

    next() {
        if (this.currentIndex < this.maxIndex) {
            this.currentIndex++;
            this.updateCarousel();
        }
    }

    updateCarousel() {
        const translateX = -this.currentIndex * this.cardWidth;
        this.carousel.style.transform = `translateX(${translateX}px)`;
        this.updateButtons();
    }

    updateButtons() {
        this.prevBtn.disabled = this.currentIndex === 0;
        this.nextBtn.disabled = this.currentIndex >= this.maxIndex;
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new Carousel();
});

document.querySelectorAll('.card-button').forEach(button => {
    button.addEventListener('click', (e) => {
        e.preventDefault();

        button.style.transform = 'scale(0.95)';
        setTimeout(() => {
            button.style.transform = 'scale(1)';
        }, 150);

        console.log('Navigating to product page...');
    });
});


function navigateTo(category) {
    console.log('[v0] Navigating to category:', category);
    alert(`Điều hướng đến: ${category}`);
}




class BannerButton extends HTMLElement {
    connectedCallback() {
        this.innerHTML = `
                                                                                                                                                                                                                            <div class="banner-button"
                                                                                                                                                                                                                                ">
                                                                                                                                                                                                                                <p class="banner-title"
                                                                                                                                                                                                                                   style="opacity: 0; margin-right: 300px; font-size: 15px; background: #fff; color: #000; font-weight: bold; border: 2px solid #000;">
                                                                                                                                                                                                                                    LIGHT THE BAY
                                                                                                                                                                                                                                </p>
                                                                                                                                                                                                                                <p class="banner-subtitle"
                                                                                                                                                                                                                                   style="opacity: 0; margin-bottom: 20px; font-size: 15px; background: #fff; color: #000; font-weight: bold; border: 2px solid #000;">
                                                                                                                                                                                                                                    Sẵn sàng tăng tốc cùng bộ sưu tập adidas x Mercedes-<br />AMG PETRONAS F1 Team
                                                                                                                                                                                                                                </p>
                                                                                                                                                                                                                                <p href="../Product/Product-nam.html" class="btn" style="margin-right: 300px;">
                                                                                                                                                                                                                                    Mua ngay →
                                                                                                                                                                                                                                </p>
                                                                                                                                                                                                                            </div>

                                                                                                                                                                                                                                                        `;
    }
}
customElements.define('banner-button', BannerButton);




document.addEventListener("DOMContentLoaded", () => {
    const template = document.getElementById("adidas-card-template");

    document.querySelectorAll("section[id^='carousel-']").forEach((section, index) => {
        const clone = template.content.cloneNode(true);

        // tên
        clone.querySelector(".section-title").textContent = section.dataset.title;

        // cả cái card
        const products = JSON.parse(section.dataset.products);
        const wrapper = clone.querySelector(".carousel-wrapper");

        products.forEach(p => {
            //có thể nhấn toàn bộ card
            const cardLink = document.createElement("a");
            cardLink.className = "product-card-link";

            //link
            cardLink.href = "../Products/Index";

            // HTML
            cardLink.innerHTML = `
                                                                                                                                                                                                                        <div class="card" style="height:650px;">
                                                                                                                                                                                                                            <img src="${p.img}" class="card-image" style="height: ${p.height || '380px'}; object-fit: cover;" />
                                                                                                                                                                                                                            <div class="card-content">
                                                                                                                                                                                                                                <h3 class="card-title">${p.price}</h3>
                                                                                                                                                                                                                                <p class="card-description">${p.desc}</p>

                                                                                                                                                                                                                                <span class="card-button">Mua ngay</span>
                                                                                                                                                                                                                            </div>
                                                                                                                                                                                                                        </div>
                                                                                                                                                                                                                    `;

            // a
            wrapper.appendChild(cardLink);
        });


        section.appendChild(clone);

        //logic carousel
        const carousel = section.querySelector(".carousel-wrapper");
        const prevBtn = section.querySelector(".nav-button.prev");
        const nextBtn = section.querySelector(".nav-button.next");
        const cards = section.querySelectorAll(".card");
        let currentIndex = 0;
        const cardWidth = 360;
        const visibleCards = Math.floor(section.offsetWidth / cardWidth);
        const maxIndex = Math.max(0, cards.length - visibleCards);

        function updateCarousel() {
            carousel.style.transform = `translateX(${-currentIndex * cardWidth}px)`;
            prevBtn.disabled = currentIndex === 0;
            nextBtn.disabled = currentIndex >= maxIndex;
        }

        prevBtn.addEventListener("click", () => {
            if (currentIndex > 0) {
                currentIndex--;
                updateCarousel();
            }
        });

        nextBtn.addEventListener("click", () => {
            if (currentIndex < maxIndex) {
                currentIndex++;
                updateCarousel();
            }
        });

        updateCarousel();
    });
});
function handleClick(url) {
    window.location.href = url;
}