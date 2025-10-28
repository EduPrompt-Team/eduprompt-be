# 📋 TỔNG HỢP TẤT CẢ ENDPOINTS - EDUPROMPT API

## 🔗 Base URL
- **HTTP**: `http://localhost:5217`
- **HTTPS**: `https://localhost:7199`
- **Swagger UI**: `http://localhost:5217/swagger`

---

## 🔑 01. Authentication (Public)

### POST /api/auth/register
- **Mô tả**: Đăng ký tài khoản mới
- **Auth**: Không cần
- **Body**: RegisterRequestDto

### POST /api/auth/login
- **Mô tả**: Đăng nhập với email và password
- **Auth**: Không cần
- **Body**: LoginRequestDto
- **Response**: Access token và Refresh token

### POST /api/auth/google-login
- **Mô tả**: Đăng nhập bằng Google OAuth
- **Auth**: Không cần
- **Body**: GoogleLoginRequestDto

### POST /api/auth/refresh-token
- **Mô tả**: Làm mới access token
- **Auth**: Không cần
- **Body**: RefreshTokenRequestDto

### POST /api/auth/revoke-token
- **Mô tả**: Thu hồi refresh token (logout)
- **Auth**: Cần
- **Body**: RefreshTokenRequestDto

### GET /api/auth/me
- **Mô tả**: Lấy thông tin user hiện tại
- **Auth**: Cần

---

## 👥 02. Users

### GET /api/users
- **Mô tả**: Lấy tất cả users (Admin)
- **Auth**: Cần
- **Role**: Admin

### GET /api/users/{id}
- **Mô tả**: Lấy user theo ID
- **Auth**: Cần

### POST /api/users
- **Mô tả**: Tạo user mới (Admin)
- **Auth**: Cần
- **Body**: UserCreateDto

### PUT /api/users/{id}
- **Mô tả**: Cập nhật user
- **Auth**: Cần
- **Body**: UserUpdateDto

### DELETE /api/users/{id}
- **Mô tả**: Xóa user (Admin)
- **Auth**: Cần
- **Role**: Admin

---

## 🔐 03. Roles (Admin)

### GET /api/roles
- **Mô tả**: Lấy tất cả roles
- **Auth**: Cần

### GET /api/roles/{id}
- **Mô tả**: Lấy role theo ID
- **Auth**: Cần

### POST /api/roles
- **Mô tả**: Tạo role mới
- **Auth**: Cần
- **Body**: RoleCreateUpdateDto

### PUT /api/roles/{id}
- **Mô tả**: Cập nhật role
- **Auth**: Cần
- **Body**: RoleCreateUpdateDto

### DELETE /api/roles/{id}
- **Mô tả**: Xóa role
- **Auth**: Cần

---

## 📂 04. Categories (Admin)

### GET /api/categories
- **Mô tả**: Lấy tất cả categories
- **Auth**: Cần (Admin)

### GET /api/categories/root
- **Mô tả**: Lấy root categories
- **Auth**: Cần (Admin)

### GET /api/categories/{id}
- **Mô tả**: Lấy category theo ID
- **Auth**: Cần (Admin)

### GET /api/categories/{id}/subcategories
- **Mô tả**: Lấy subcategories
- **Auth**: Cần (Admin)

### POST /api/categories
- **Mô tả**: Tạo category mới
- **Auth**: Cần (Admin)
- **Body**: CreatePackageCategoryDto

### PUT /api/categories/{id}
- **Mô tả**: Cập nhật category
- **Auth**: Cần (Admin)
- **Body**: CreatePackageCategoryDto

### DELETE /api/categories/{id}
- **Mô tả**: Xóa category
- **Auth**: Cần (Admin)

---

## 💬 05. Conversations

### GET /api/conversation/user/{UserId}
- **Mô tả**: Lấy conversations của user
- **Auth**: Cần

### GET /api/conversation/{id}
- **Mô tả**: Lấy conversation theo ID
- **Auth**: Cần

### POST /api/conversation
- **Mô tả**: Tạo conversation mới
- **Auth**: Cần
- **Body**: CreateConversationDto

### PUT /api/conversation/{id}
- **Mô tả**: Cập nhật conversation
- **Auth**: Cần
- **Body**: CreateConversationDto

### DELETE /api/conversation/{id}
- **Mô tả**: Xóa conversation
- **Auth**: Cần

### GET /api/conversation/user/{UserId}/recent
- **Mô tả**: Lấy conversations gần đây
- **Auth**: Cần
- **Query**: count (optional)

---

## 📨 06. Messages

### GET /api/message/conversation/{ConversationId}
- **Mô tả**: Lấy messages trong conversation
- **Auth**: Cần

### GET /api/message/{id}
- **Mô tả**: Lấy message theo ID
- **Auth**: Cần

### POST /api/message
- **Mô tả**: Gửi message mới
- **Auth**: Cần
- **Body**: CreateMessageDto

### PUT /api/message/{id}
- **Mô tả**: Cập nhật message
- **Auth**: Cần
- **Body**: CreateMessageDto

### DELETE /api/message/{id}
- **Mô tả**: Xóa message
- **Auth**: Cần

### GET /api/message/conversation/{ConversationId}/recent
- **Mô tả**: Lấy messages gần đây
- **Auth**: Cần
- **Query**: count (optional, default: 50)

### GET /api/message/conversation/{ConversationId}/last
- **Mô tả**: Lấy message cuối cùng
- **Auth**: Cần

---

## 📝 07. Posts

### GET /api/post
- **Mô tả**: Lấy tất cả posts
- **Auth**: Không cần

### GET /api/post/{PostId}
- **Mô tả**: Lấy post theo ID
- **Auth**: Không cần

### GET /api/post/user/{UserId}
- **Mô tả**: Lấy posts của user
- **Auth**: Không cần

### GET /api/post/published
- **Mô tả**: Lấy posts đã publish
- **Auth**: Không cần

### GET /api/post/type/{postType}
- **Mô tả**: Lấy posts theo loại
- **Auth**: Không cần

### GET /api/post/search?searchTerm={term}
- **Mô tả**: Tìm kiếm posts
- **Auth**: Không cần

### POST /api/post
- **Mô tả**: Tạo post mới
- **Auth**: Cần
- **Body**: CreatePostDto

### PUT /api/post/{PostId}
- **Mô tả**: Cập nhật post
- **Auth**: Cần
- **Body**: CreatePostDto

### DELETE /api/post/{PostId}
- **Mô tả**: Xóa post
- **Auth**: Cần

### POST /api/post/{PostId}/like
- **Mô tả**: Like post
- **Auth**: Cần

### GET /api/post/{PostId}/rating
- **Mô tả**: Lấy rating trung bình
- **Auth**: Không cần

---

## 🛒 08. Wishlists

### GET /api/wishlists/my-wishlist
- **Mô tả**: Lấy wishlist của tôi
- **Auth**: Cần

### POST /api/wishlists
- **Mô tả**: Thêm vào wishlist
- **Auth**: Cần
- **Body**: WishlistCreateDto

### DELETE /api/wishlists/{id}
- **Mô tả**: Xóa khỏi wishlist
- **Auth**: Cần

### GET /api/wishlists/check/{PackageId}
- **Mô tả**: Kiểm tra có trong wishlist
- **Auth**: Cần

---

## 📦 09. Storage Templates

### GET /api/storage-templates/my-storage
- **Mô tả**: Lấy storage của tôi
- **Auth**: Cần

### POST /api/storage-templates
- **Mô tả**: Thêm vào storage
- **Auth**: Cần
- **Body**: StorageTemplateCreateDto

### DELETE /api/storage-templates/{id}
- **Mô tả**: Xóa khỏi storage
- **Auth**: Cần

### GET /api/storage-templates/check/{PackageId}
- **Mô tả**: Kiểm tra có trong storage
- **Auth**: Cần

---

## 💳 10. Payment Methods

### GET /api/paymentmethod
- **Mô tả**: Lấy tất cả payment methods (Admin)
- **Auth**: Cần

### GET /api/paymentmethod/user/{UserId}
- **Mô tả**: Lấy payment methods của user
- **Auth**: Cần

### GET /api/paymentmethod/{id}
- **Mô tả**: Lấy payment method theo ID
- **Auth**: Cần

### POST /api/paymentmethod
- **Mô tả**: Tạo payment method mới
- **Auth**: Cần
- **Body**: CreatePaymentMethodDto

### PUT /api/paymentmethod/{id}
- **Mô tả**: Cập nhật payment method
- **Auth**: Cần
- **Body**: CreatePaymentMethodDto

### DELETE /api/paymentmethod/{id}
- **Mô tả**: Xóa payment method
- **Auth**: Cần

### GET /api/paymentmethod/user/{UserId}/default
- **Mô tả**: Lấy payment method mặc định
- **Auth**: Cần

### POST /api/paymentmethod/{id}/set-default?UserId={userId}
- **Mô tả**: Đặt làm mặc định
- **Auth**: Cần

---

## 💰 11. Transactions

### GET /api/transaction
- **Mô tả**: Lấy tất cả transactions (Admin)
- **Auth**: Cần

### GET /api/transaction/wallet/{WalletId}
- **Mô tả**: Lấy transactions của wallet
- **Auth**: Cần

### GET /api/transaction/user/{UserId}
- **Mô tả**: Lấy transactions của user
- **Auth**: Cần

### GET /api/transaction/{id}
- **Mô tả**: Lấy transaction theo ID
- **Auth**: Cần

### POST /api/transaction
- **Mô tả**: Tạo transaction mới
- **Auth**: Cần
- **Body**: CreateTransactionDto

### PUT /api/transaction/{id}
- **Mô tả**: Cập nhật transaction
- **Auth**: Cần
- **Body**: CreateTransactionDto

### DELETE /api/transaction/{id}
- **Mô tả**: Xóa transaction
- **Auth**: Cần

---

## 🏦 12. Wallet

### GET /api/wallet/user/{UserId}
- **Mô tả**: Lấy wallet của user
- **Auth**: Cần

### GET /api/wallet/{WalletId}
- **Mô tả**: Lấy wallet theo ID
- **Auth**: Cần

### POST /api/wallet
- **Mô tả**: Tạo wallet mới
- **Auth**: Cần
- **Body**: CreateWalletDto

### PUT /api/wallet/{WalletId}
- **Mô tả**: Cập nhật wallet
- **Auth**: Cần
- **Body**: UpdateWalletDto

### DELETE /api/wallet/{WalletId}
- **Mô tả**: Xóa wallet
- **Auth**: Cần

### GET /api/wallet/balance/{UserId}
- **Mô tả**: Lấy số dư ví
- **Auth**: Cần

### POST /api/wallet/add-funds
- **Mô tả**: Nạp tiền vào ví
- **Auth**: Cần
- **Body**: AddFundsRequest { UserId, Amount }

### POST /api/wallet/deduct-funds
- **Mô tả**: Trừ tiền từ ví
- **Auth**: Cần
- **Body**: DeductFundsRequest { UserId, Amount }

---

## 📚 13. Packages

### GET /api/package
- **Mô tả**: Lấy tất cả packages
- **Auth**: Không cần

### GET /api/package/{PackageId}
- **Mô tả**: Lấy package theo ID
- **Auth**: Không cần

### GET /api/package/category/{CategoryId}
- **Mô tả**: Lấy packages theo category
- **Auth**: Không cần

### GET /api/package/active
- **Mô tả**: Lấy packages active
- **Auth**: Không cần

### GET /api/package/search?searchTerm={term}
- **Mô tả**: Tìm kiếm packages
- **Auth**: Không cần

### GET /api/package/price-range?minPrice={min}&maxPrice={max}
- **Mô tả**: Lấy packages theo khoảng giá
- **Auth**: Không cần

### POST /api/package
- **Mô tả**: Tạo package mới (Admin)
- **Auth**: Cần
- **Body**: CreatePackageDto

### PUT /api/package/{PackageId}
- **Mô tả**: Cập nhật package
- **Auth**: Cần
- **Body**: UpdatePackageDto

### DELETE /api/package/{PackageId}
- **Mô tả**: Xóa package
- **Auth**: Cần

---

## 📦 14. Orders

### POST /api/order/create-from-cart?notes={notes}&UserId={userId}
- **Mô tả**: Tạo order từ cart
- **Auth**: Cần
- **Query**: notes (optional), UserId (default: 1)

### GET /api/order
- **Mô tả**: Lấy tất cả orders (Admin)
- **Auth**: Cần

### GET /api/order/my
- **Mô tả**: Lấy orders của tôi
- **Auth**: Cần

### GET /api/order/{orderId}
- **Mô tả**: Lấy order theo ID
- **Auth**: Cần

### GET /api/order/admin/{orderId}
- **Mô tả**: Lấy order theo ID (Admin, xem bất kỳ)
- **Auth**: Cần

### POST /api/order/{orderId}/cancel
- **Mô tả**: Hủy order
- **Auth**: Cần

### PATCH /api/order/{orderId}/status?status={status}
- **Mô tả**: Cập nhật status order
- **Auth**: Cần

---

## 🤖 15. AI History

### GET /api/aihistory
- **Mô tả**: Lấy tất cả AI history (Admin)
- **Auth**: Cần

### GET /api/aihistory/user/{UserId}
- **Mô tả**: Lấy AI history của user
- **Auth**: Cần

### GET /api/aihistory/instance/{InstanceId}
- **Mô tả**: Lấy AI history theo instance
- **Auth**: Cần

### GET /api/aihistory/{id}
- **Mô tả**: Lấy AI history theo ID
- **Auth**: Cần

### POST /api/aihistory
- **Mô tả**: Tạo AI history mới
- **Auth**: Cần
- **Body**: CreateAIHistoryDto

### PUT /api/aihistory/{id}
- **Mô tả**: Cập nhật AI history
- **Auth**: Cần
- **Body**: CreateAIHistoryDto

### DELETE /api/aihistory/{id}
- **Mô tả**: Xóa AI history
- **Auth**: Cần

### GET /api/aihistory/user/{UserId}/recent?count={count}
- **Mô tả**: Lấy AI history gần đây
- **Auth**: Cần
- **Query**: count (optional, default: 10)

### GET /api/aihistory/user/{UserId}/stats
- **Mô tả**: Lấy thống kê AI history
- **Auth**: Cần

---

## 🔑 16. API Keys

### GET /api/apikey/package/{PackageId}
- **Mô tả**: Lấy API keys của package
- **Auth**: Cần

### GET /api/apikey/active/package/{PackageId}
- **Mô tả**: Lấy API keys active của package
- **Auth**: Không cần

### GET /api/apikey/provider/{provider}
- **Mô tả**: Lấy API key theo provider
- **Auth**: Không cần

### POST /api/apikey
- **Mô tả**: Tạo API key mới
- **Auth**: Cần
- **Body**: CreateAPIKeyDto

### PUT /api/apikey/{apiKeyId}
- **Mô tả**: Cập nhật API key
- **Auth**: Cần
- **Body**: CreateAPIKeyDto

### DELETE /api/apikey/{apiKeyId}
- **Mô tả**: Xóa API key
- **Auth**: Cần

---

## 📊 17. Feedback

### GET /api/feedback/post/{PostId}
- **Mô tả**: Lấy feedbacks của post
- **Auth**: Cần

### GET /api/feedback/user/{UserId}
- **Mô tả**: Lấy feedbacks của user
- **Auth**: Cần

### GET /api/feedback/{id}
- **Mô tả**: Lấy feedback theo ID
- **Auth**: Cần

### POST /api/feedback
- **Mô tả**: Tạo feedback mới
- **Auth**: Cần
- **Body**: CreateFeedbackDto

### PUT /api/feedback/{id}
- **Mô tả**: Cập nhật feedback
- **Auth**: Cần
- **Body**: CreateFeedbackDto

### DELETE /api/feedback/{id}
- **Mô tả**: Xóa feedback
- **Auth**: Cần

### GET /api/feedback/post/{PostId}/rating
- **Mô tả**: Lấy rating trung bình
- **Auth**: Cần

### GET /api/feedback/post/{PostId}/count
- **Mô tả**: Lấy số lượng feedback
- **Auth**: Cần

### GET /api/feedback/post/{PostId}/recent?count={count}
- **Mô tả**: Lấy feedbacks gần đây
- **Auth**: Cần
- **Query**: count (optional, default: 10)

---

## 🎯 18. Prompt Instances

### GET /api/promptinstance/{InstanceId}
- **Mô tả**: Lấy prompt instance theo ID
- **Auth**: Cần

### GET /api/promptinstance/user/{UserId}
- **Mô tả**: Lấy prompt instances của user
- **Auth**: Cần

### GET /api/promptinstance/template/{templateId}
- **Mô tả**: Lấy prompt instances theo template
- **Auth**: Cần

### GET /api/promptinstance/status/{status}
- **Mô tả**: Lấy prompt instances theo status
- **Auth**: Cần

### GET /api/promptinstance/recent/{UserId}?count={count}
- **Mô tả**: Lấy prompt instances gần đây
- **Auth**: Cần
- **Query**: count (optional, default: 10)

### POST /api/promptinstance
- **Mô tả**: Tạo prompt instance mới
- **Auth**: Cần
- **Body**: CreatePromptInstanceDto

### PUT /api/promptinstance/{InstanceId}
- **Mô tả**: Cập nhật prompt instance
- **Auth**: Cần
- **Body**: UpdatePromptInstanceDto

### DELETE /api/promptinstance/{InstanceId}
- **Mô tả**: Xóa prompt instance
- **Auth**: Cần

### POST /api/promptinstance/{InstanceId}/complete
- **Mô tả**: Hoàn thành instance với output data
- **Auth**: Cần
- **Body**: CompleteInstanceRequest { OutputData }

---

## 🛒 19. Cart

### GET /api/cart
- **Mô tả**: Lấy cart của user hiện tại
- **Auth**: Cần

### POST /api/cart/items
- **Mô tả**: Thêm item vào cart
- **Auth**: Cần
- **Body**: AddCartItemDto

### PUT /api/cart/items/{cartDetailId}
- **Mô tả**: Cập nhật số lượng item
- **Auth**: Cần
- **Body**: UpdateCartItemDto

### DELETE /api/cart/items/{cartDetailId}
- **Mô tả**: Xóa item khỏi cart
- **Auth**: Cần

### DELETE /api/cart
- **Mô tả**: Xóa toàn bộ cart
- **Auth**: Cần

---

## 🏗️ 20. Template Architecture

### GET /api/templatearchitecture/instance/{InstanceId}
- **Mô tả**: Lấy template architecture theo instance
- **Auth**: Không cần

### POST /api/templatearchitecture
- **Mô tả**: Tạo template architecture mới (Admin)
- **Auth**: Cần
- **Body**: CreateTemplateArchitectureDto

### PUT /api/templatearchitecture/{architectureId}
- **Mô tả**: Cập nhật template architecture
- **Auth**: Cần
- **Body**: CreateTemplateArchitectureDto

### DELETE /api/templatearchitecture/{architectureId}
- **Mô tả**: Xóa template architecture
- **Auth**: Cần

---

## 🔚 21. Package Details & Categories

### POST /api/packagedetail
- **Mô tả**: Tạo package detail mới
- **Auth**: Cần

### POST /api/packagecategory
- **Mô tả**: Tạo package category mới
- **Auth**: Cần

### POST /api/promptinstancedetail
- **Mô tả**: Tạo prompt instance detail mới
- **Auth**: Cần

---

## 📋 TỔNG KẾT

### Số lượng Endpoints theo nhóm:
- **Authentication**: 6 endpoints
- **Users**: 5 endpoints
- **Roles**: 5 endpoints
- **Categories**: 7 endpoints
- **Conversations**: 6 endpoints
- **Messages**: 7 endpoints
- **Posts**: 11 endpoints
- **Wishlists**: 4 endpoints
- **Storage Templates**: 4 endpoints
- **Payment Methods**: 8 endpoints
- **Transactions**: 7 endpoints
- **Wallet**: 8 endpoints
- **Packages**: 9 endpoints
- **Orders**: 6 endpoints
- **AI History**: 9 endpoints
- **API Keys**: 6 endpoints
- **Feedback**: 9 endpoints
- **Prompt Instances**: 9 endpoints
- **Cart**: 5 endpoints
- **Template Architecture**: 4 endpoints
- **Package Details & Categories**: 3 endpoints

### **TỔNG CỘNG: ~130+ ENDPOINTS**

---

## 🔐 Authentication Flow

1. **Register**: `POST /api/auth/register`
2. **Login**: `POST /api/auth/login` → Lấy token
3. **Sử dụng**: Thêm `Authorization: Bearer {token}` vào header
4. **Refresh**: `POST /api/auth/refresh-token` khi token hết hạn
5. **Logout**: `POST /api/auth/revoke-token`

---

## 📌 Ghi chú:

- **Auth**: Cần xác thực (Bearer Token)
- **Admin**: Cần quyền Admin
- **Public**: Không cần auth
- **Query**: Query parameters
- **Body**: Request body

---

**Generated at**: $(Get-Date)
**API Version**: v1.0
**Base URL**: http://localhost:5217

