# 👤 User Features APIs - Reviews, Wishlists & Storage

## 📋 Overview
Đây là các API cho phép user **tương tác** với prompt templates:
- **Reviews**: Đánh giá và rating templates
- **Wishlists**: Lưu templates yêu thích
- **Storage Templates**: Bộ sưu tập cá nhân (sau khi mua)

---

## ⭐ Reviews API

### 1. Get Reviews for Template (PUBLIC)
**Endpoint:** `GET /api/reviews/template/{templateId}`  
**Access:** Public  
**Description:** Lấy tất cả đánh giá của một template

**Example:** `GET /api/reviews/template/5`

**Response:**
```json
[
  {
    "reviewId": 1,
    "userId": 10,
    "templateId": 5,
    "comment": "Rất hữu ích cho việc dạy toán!",
    "ratingScore": 5.0,
    "createdDate": "2024-01-15T10:30:00",
    "status": "Active",
    "userName": "Nguyễn Văn A",
    "templateName": "Giải bài toán đại số"
  }
]
```

### 2. Get My Reviews (AUTH)
**Endpoint:** `GET /api/reviews/my-reviews`  
**Access:** Authenticated users  
**Description:** Lấy tất cả đánh giá của user hiện tại

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

### 3. Create Review (AUTH)
**Endpoint:** `POST /api/reviews`  
**Access:** Authenticated users  
**Description:** Tạo đánh giá mới cho một template

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Request Body:**
```json
{
  "templateId": 5,
  "comment": "Template này rất tốt!",
  "ratingScore": 4.5
}
```

**Validation:**
- `ratingScore`: Phải từ 1 đến 5
- Mỗi user chỉ được review 1 lần cho mỗi template

### 4. Update Review (AUTH)
**Endpoint:** `PUT /api/reviews/{id}`  
**Access:** Review owner only  
**Description:** Cập nhật đánh giá của mình

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Request Body:**
```json
{
  "comment": "Cập nhật: Template tuyệt vời!",
  "ratingScore": 5.0
}
```

### 5. Delete Review (AUTH)
**Endpoint:** `DELETE /api/reviews/{id}`  
**Access:** Review owner only  
**Description:** Xóa đánh giá của mình

---

## ❤️ Wishlists API

### 1. Get My Wishlist (AUTH)
**Endpoint:** `GET /api/wishlists/my-wishlist`  
**Access:** Authenticated users  
**Description:** Lấy danh sách wishlist của user

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Response:**
```json
[
  {
    "wishlistId": 1,
    "userId": 10,
    "templateId": 5,
    "wishlistName": "My Favorites",
    "createdDate": "2024-01-15T10:30:00",
    "status": "Active",
    "templateName": "Giải bài toán đại số",
    "templateDescription": "Prompt giúp giải toán...",
    "templatePrice": 50000,
    "templatePreviewUrl": "https://example.com/preview.png"
  }
]
```

### 2. Add to Wishlist (AUTH)
**Endpoint:** `POST /api/wishlists`  
**Access:** Authenticated users  
**Description:** Thêm template vào wishlist

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Request Body:**
```json
{
  "templateId": 5,
  "wishlistName": "Toán học yêu thích"
}
```

**Business Rules:**
- Mỗi template chỉ được add 1 lần vào wishlist
- Tự động kiểm tra template có tồn tại không

### 3. Remove from Wishlist (AUTH)
**Endpoint:** `DELETE /api/wishlists/{id}`  
**Access:** Wishlist owner only  
**Description:** Xóa template khỏi wishlist

### 4. Check if in Wishlist (AUTH)
**Endpoint:** `GET /api/wishlists/check/{templateId}`  
**Access:** Authenticated users  
**Description:** Kiểm tra template đã có trong wishlist chưa

**Response:**
```json
{
  "templateId": 5,
  "isInWishlist": true
}
```

---

## 📚 Storage Templates API (Personal Library)

### 1. Get My Storage (AUTH)
**Endpoint:** `GET /api/storage-templates/my-storage`  
**Access:** Authenticated users  
**Description:** Lấy bộ sưu tập cá nhân (templates đã mua)

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Response:**
```json
[
  {
    "storageId": 1,
    "userId": 10,
    "templateId": 5,
    "uploadDate": "2024-01-15T10:30:00",
    "status": "Active",
    "templateName": "Giải bài toán đại số",
    "templateDescription": "Prompt giúp giải toán...",
    "templatePrice": 50000,
    "templatePreviewUrl": "https://example.com/preview.png"
  }
]
```

### 2. Add to Storage (AUTH)
**Endpoint:** `POST /api/storage-templates`  
**Access:** Authenticated users  
**Description:** Thêm template vào storage (sau khi mua)

**Headers:**
```
Authorization: Bearer {your-jwt-token}
```

**Request Body:**
```json
{
  "templateId": 5
}
```

**Use Case:** Được gọi sau khi user thanh toán thành công

### 3. Remove from Storage (AUTH)
**Endpoint:** `DELETE /api/storage-templates/{id}`  
**Access:** Storage owner only  
**Description:** Xóa template khỏi storage

### 4. Check if in Storage (AUTH)
**Endpoint:** `GET /api/storage-templates/check/{templateId}`  
**Access:** Authenticated users  
**Description:** Kiểm tra template đã có trong storage chưa

**Response:**
```json
{
  "templateId": 5,
  "isInStorage": true
}
```

---

## 🎯 User Flow Examples

### Flow 1: User xem và đánh giá template
```javascript
// 1. User xem chi tiết template (public)
GET /api/discovery/templates/5

// 2. Xem reviews của template (public)
GET /api/reviews/template/5

// 3. User login
POST /api/auth/login

// 4. User tạo review
POST /api/reviews
{
  "templateId": 5,
  "comment": "Rất hữu ích!",
  "ratingScore": 5
}
```

### Flow 2: User lưu wishlist
```javascript
// 1. User thấy template thích
GET /api/discovery/templates/5

// 2. Check xem đã trong wishlist chưa
GET /api/wishlists/check/5

// 3. Add vào wishlist
POST /api/wishlists
{
  "templateId": 5,
  "wishlistName": "Toán học"
}

// 4. Xem tất cả wishlist
GET /api/wishlists/my-wishlist
```

### Flow 3: User mua template
```javascript
// 1. User chọn template để mua
GET /api/discovery/templates/5

// 2. Thanh toán (Payment API - sẽ làm sau)
POST /api/orders/create

// 3. Sau khi thanh toán thành công → Add vào Storage
POST /api/storage-templates
{
  "templateId": 5
}

// 4. Xem tất cả templates đã mua
GET /api/storage-templates/my-storage
```

---

## 🔒 Authorization

Tất cả endpoints có `[AUTH]` yêu cầu JWT token:

```javascript
// Add token to request header
headers: {
  'Authorization': 'Bearer ' + token,
  'Content-Type': 'application/json'
}
```

---

## ✅ Business Rules

### Reviews
- ✅ Mỗi user chỉ review 1 lần cho mỗi template
- ✅ Rating từ 1 đến 5
- ✅ Chỉ owner mới edit/delete review của mình
- ✅ Public có thể xem tất cả reviews

### Wishlists
- ✅ Mỗi template chỉ add 1 lần vào wishlist
- ✅ Chỉ owner mới xóa items trong wishlist của mình
- ✅ Private - chỉ user mới xem wishlist của mình

### Storage Templates
- ✅ Template được add sau khi thanh toán
- ✅ Mỗi template chỉ add 1 lần vào storage
- ✅ Private - chỉ user mới xem storage của mình

---

## 🧪 Testing

### 1. Test with Swagger
1. Mở: `http://localhost:5217/swagger`
2. Login để lấy token: `POST /api/auth/login`
3. Click **🔓 Authorize**, nhập `Bearer {token}`
4. Test các endpoints

### 2. Test with Postman

**Collection Variables:**
- `baseUrl`: `http://localhost:5217`
- `token`: (auto-set sau login)

**Example Requests:**

```javascript
// Get My Wishlist
GET {{baseUrl}}/api/wishlists/my-wishlist
Authorization: Bearer {{token}}

// Add to Wishlist
POST {{baseUrl}}/api/wishlists
Authorization: Bearer {{token}}
{
  "templateId": 5
}
```

---

## 📊 Database Schema

### Reviews Table
```sql
ReviewId (PK)
UserId (FK → Users)
TemplateId (FK → PromptTemplates)
Comment (nvarchar)
RatingScore (decimal 2,1)
CreatedDate (datetime)
Status (nvarchar)
```

### Wishlists Table
```sql
WishlistId (PK)
UserId (FK → Users)
TemplateId (FK → PromptTemplates)
WishlistName (nvarchar)
CreatedDate (datetime)
Status (nvarchar)
```

### StorageTemplates Table
```sql
StorageId (PK)
UserId (FK → Users)
TemplateId (FK → PromptTemplates)
UploadDate (datetime)
UpdatedDate (datetime)
Status (nvarchar)
```

---

## 🎨 Frontend Integration Example

### React Hook for Wishlist
```jsx
const useWishlist = () => {
  const addToWishlist = async (templateId) => {
    const response = await fetch('/api/wishlists', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ templateId })
    });
    return response.json();
  };

  const checkWishlist = async (templateId) => {
    const response = await fetch(`/api/wishlists/check/${templateId}`, {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    const data = await response.json();
    return data.isInWishlist;
  };

  return { addToWishlist, checkWishlist };
};
```

---

## 🚀 Summary

✅ **Reviews** - Đánh giá và rating templates  
✅ **Wishlists** - Lưu templates yêu thích  
✅ **Storage** - Bộ sưu tập cá nhân (sau khi mua)  
✅ **Full CRUD** operations  
✅ **Authorization** - Chỉ owner mới edit/delete  
✅ **Validation** - Business rules enforcement  

**Tất cả đã sẵn sàng để test! 🎉** 