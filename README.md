##🏪 AdidasStore Website

📁 Cấu trúc thư mục dự án
```
AdidasStore/
├── Content/
│   ├── assets/             #ảnh, video nội dung
│   ├── adidascard          #ảnh, video nền
│   └── file.css            #file.css
│        
├── partials/               #modul có thể tái sử dụng
│   ├── iconheader/         #icon, ảnh
│   ├── footer              #file footer
│   └── header              #file header
│ 
├── Scripts/
│   └── filejs              #file.js
│
├── Home                    #Trang chủ
├── Product                 #Danh sách sản phẩm
├── Product-detail          #Thông tin sản phẩm       
├── Cart                    #Giỏ hàng
├── Checkout                #Thanh toán
├── Order-history           #Lịch sử mua hàng
├── login                   #Đăng nhập
├── register                #Đăng kí
└── ...
```

##📌 Giải thích nhanh:

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
##📅 Thời gian hoàn thành dự án
```
## 📅 Tiến độ dự án

|           Giai đoạn          |        Nội dung            |   Bắt đầu  |       Kết thúc       |    Tình trạng    |
|------------------------------|----------------------------|------------|----------------------|------------------|
| 🧑‍💻 Báo cáo tiến độ lần 1     | Phát triển Front-End (FE) | 01/10/2025 | 14/10/2025 - 10:00 AM | 🟡 Đang bắt đầu |
| 🛠️ Báo cáo tiến độ lần 2     | Phát triển Back-End (BE)  | 09/09/2025 | 16/09/2025 - 12:00 AM | 🔵 Sắp bắt đầu  |
| 📄 Nộp báo cáo & thi vấn đáp | Tổng kết toàn bộ dự án    | 20/11/2025 | 05/12/2025 - 9:59 PM  | ⚪ Chưa bắt đầu |

```
##👥 Thành viên Nhóm
```
## 👥 Thành viên

|           Tên          |        Vai trò             |  Ghi chú   |
|------------------------|----------------------------|------------|
| Nguyễn Ngọc Duy Khánh  |                            |            | 
| Trần Dức An            |                            |            | 
| Đặng Minh Khôi         |                            |            | 


```
