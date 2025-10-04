🏪 AdidasStore Website

Dự án AdidasStore là một website bán hàng giao diện tĩnh mô phỏng trang web thương mại điện tử của Adidas. Dự án tập trung vào phần Front-End (HTML/CSS/JS) và có cấu trúc rõ ràng để các thành viên dễ dàng phát triển và mở rộng tính năng.

📁 Cấu trúc thư mục dự án
```
AdidasStore/
├── Content/
│   ├── assets/
│   │   ├── Product-detail-Yeezy-Boost.css
│   │   └── Trangchu.css
│   └── partials/
│       ├── iconheader/
│       │   ├── adidas-black-logo.png
│       │   ├── heart_.png
│       │   ├── search_.png
│       │   ├── shop_.png
│       │   ├── user_.png
│       │   └── video.mp4
│       ├── adidascard.html
│       ├── footer.css
│       ├── footer.html
│       ├── header.css
│       ├── header.html
│       └── header.js
├── Scripts/
│   ├── Product-detail-Yeezy-Boost.js
│   └── Trangchu.js
├── Product-detail-Yeezy-Boost.html
├── Trangchu.html
├── Web.config
├── packages.config
└── src.csproj
```

📌 Giải thích nhanh:

Content/assets/ → chứa CSS cho từng trang

Content/partials/ → header, footer và các phần giao diện tái sử dụng

Scripts/ → chứa các file JavaScript

Trangchu.html, Product-detail-Yeezy-Boost.html → các trang chính của website

👩‍💻 Hướng dẫn thành viên clone & phát triển dự án
1️⃣ Clone dự án về máy
```
git clone https://github.com/asuna-chan123/my-team-website.git
```

Sau khi clone xong:
```
cd my-team-website
```
2️⃣ Tạo nhánh cá nhân để phát triển

Mỗi thành viên nên tạo nhánh riêng để phát triển tính năng và tránh xung đột code:
```
git checkout -b feature/<ten-tinh-nang>
```

Ví dụ:
```
git checkout -b feature/add-cart-page
```
3️⃣ Commit & push code lên GitHub

Khi đã hoàn thành tính năng:
```
git add .
git commit -m "Thêm tính năng giỏ hàng"
git push origin feature/add-cart-page
```
4️⃣ Tạo Pull Request (PR)

Vào trang GitHub của repo

Chọn “Compare & pull request”

Gửi PR để nhóm review code và merge vào main

🛠️ Nếu gặp lỗi khi push (ví dụ: “fetch first”)

Chạy lệnh sau để cập nhật code mới nhất trước khi push:
```
git pull origin main --allow-unrelated-histories
```

Sau đó merge, giải quyết xung đột (nếu có), rồi push lại:
```
git push origin feature/<ten-tinh-nang>
```
📅 Thời gian hoàn thành dự án
Giai đoạn	Nội dung	Bắt đầu	Deadline
```
🧑‍💻 Báo cáo tiến độ lần 1	Phát triển Front-End (FE)	01/10/2025	14/10/2025 - 10:00 AM
🛠️ Báo cáo tiến độ lần 2	Phát triển Back-End (BE)	09/09/2025	16/09/2025 - 12:00 AM
📄 Nộp báo cáo & thi vấn đáp	Tổng kết toàn bộ dự án	20/11/2025	05/12/2025 - 9:59 PM
```
👥 Thành viên phát triển
Tên	Vai trò	Ghi chú
🧑‍💻 Thành viên 1	FE Developer	Phát triển giao diện người dùng
🧑‍💻 Thành viên 2	BE Developer	Xử lý logic backend
🧑‍💻 Thành viên 3	QA / Tester	Kiểm thử và tối ưu UI/UX
