# 🔍 Public Discovery APIs - User-Facing Endpoints

## 📋 Overview
Đây là các API **CÔNG KHAI** (không cần authentication) cho phép giáo viên và người dùng khám phá, tìm kiếm prompt templates.

---

## 🌐 Public Endpoints

### 1. Get Categories (Menu/Navigation)
**Endpoint:** `GET /api/discovery/categories`  
**Access:** Public  
**Description:** Lấy danh sách danh mục để hiển thị menu, navigation

**Response Example:**
```json
[
  {
    "categoryId": 1,
    "categoryName": "Toán học",
    "description": "Các prompt về toán học",
    "numberOfTemplates": 25,
    "subCategories": [
      {
        "categoryId": 2,
        "categoryName": "Đại số",
        "numberOfTemplates": 10
      },
      {
        "categoryId": 3,
        "categoryName": "Hình học",
        "numberOfTemplates": 15
      }
    ]
  }
]
```

---

### 2. Get All Published Templates
**Endpoint:** `GET /api/discovery/templates`  
**Access:** Public  
**Description:** Lấy tất cả prompt templates đã publish

**Response Example:**
```json
[
  {
    "templateId": 1,
    "categoryId": 2,
    "templateName": "Giải bài toán đại số",
    "description": "Prompt giúp giải bài toán đại số cấp 2",
    "previewUrl": "https://example.com/preview.png",
    "tags": "toán, đại số, cấp 2",
    "price": 50000,
    "publishedDate": "2024-01-15",
    "categoryName": "Đại số",
    "creatorUserName": "Nguyễn Văn A",
    "reviewCount": 12,
    "averageRating": 4.5
  }
]
```

---

### 3. Get Template Details
**Endpoint:** `GET /api/discovery/templates/{id}`  
**Access:** Public  
**Description:** Xem chi tiết một prompt template khi user click vào

**Example:** `GET /api/discovery/templates/1`

**Response Example:**
```json
{
  "templateId": 1,
  "categoryId": 2,
  "creatorUserId": 5,
  "templateName": "Giải bài toán đại số",
  "description": "Prompt giúp giải bài toán đại số cấp 2...",
  "previewUrl": "https://example.com/preview.png",
  "tags": "toán, đại số, cấp 2",
  "price": 50000,
  "publishedDate": "2024-01-15",
  "categoryName": "Đại số",
  "creatorUserName": "Nguyễn Văn A",
  "reviewCount": 12,
  "averageRating": 4.5
}
```

---

### 4. Get Templates by Category
**Endpoint:** `GET /api/discovery/templates/category/{categoryId}`  
**Access:** Public  
**Description:** Lấy tất cả templates thuộc một danh mục

**Example:** `GET /api/discovery/templates/category/2`

**Response:** Array of PromptTemplateDto (chỉ những template đã publish)

---

### 5. Search Templates ⭐
**Endpoint:** `GET /api/discovery/templates/search?q={keyword}`  
**Access:** Public  
**Description:** Tìm kiếm template theo tên, mô tả, hoặc tags

**Example:** `GET /api/discovery/templates/search?q=toán`

**Response Example:**
```json
{
  "keyword": "toán",
  "count": 5,
  "results": [
    {
      "templateId": 1,
      "templateName": "Giải bài toán đại số",
      "description": "Prompt giúp giải bài toán...",
      "price": 50000,
      "categoryName": "Đại số",
      "reviewCount": 12,
      "averageRating": 4.5
    }
  ]
}
```

---

### 6. Get Featured Templates ⭐
**Endpoint:** `GET /api/discovery/templates/featured?limit=10`  
**Access:** Public  
**Description:** Lấy các template nổi bật (rating cao nhất)

**Query Parameters:**
- `limit` (optional): Số lượng templates trả về (default: 10)

**Example:** `GET /api/discovery/templates/featured?limit=5`

---

### 7. Get Newest Templates 🆕
**Endpoint:** `GET /api/discovery/templates/newest?limit=10`  
**Access:** Public  
**Description:** Lấy các template mới nhất

**Query Parameters:**
- `limit` (optional): Số lượng templates trả về (default: 10)

**Example:** `GET /api/discovery/templates/newest?limit=8`

---

## 🔒 Admin Endpoints (Đã có từ trước)

### Categories Management
- `POST /api/categories` - Create category (Admin only)
- `GET /api/categories` - Get all categories (Admin only)
- `GET /api/categories/{id}` - Get category by ID (Admin only)
- `PUT /api/categories/{id}` - Update category (Admin only)
- `DELETE /api/categories/{id}` - Delete category (Admin only)

### Prompt Templates Management
- `POST /api/prompt-templates` - Create template (Admin only)
- `GET /api/prompt-templates` - Get all templates (Admin only)
- `GET /api/prompt-templates/{id}` - Get template by ID (Admin only)
- `PUT /api/prompt-templates/{id}` - Update template (Admin only)
- `DELETE /api/prompt-templates/{id}` - Delete template (Admin only)

---

## 🎯 Use Cases

### 1. Homepage - Display Featured & Newest
```javascript
// Get featured templates
GET /api/discovery/templates/featured?limit=6

// Get newest templates
GET /api/discovery/templates/newest?limit=6
```

### 2. Category Navigation Menu
```javascript
// Get all categories with subcategories
GET /api/discovery/categories
```

### 3. Category Page - Show Templates by Category
```javascript
// When user clicks on "Toán học" category (ID: 1)
GET /api/discovery/templates/category/1
```

### 4. Search Feature
```javascript
// User searches for "giải toán"
GET /api/discovery/templates/search?q=giải toán
```

### 5. Template Detail Page
```javascript
// When user clicks to view template details (ID: 5)
GET /api/discovery/templates/5
```

---

## 📊 Template Status Flow

```
Draft → Published → (visible in public APIs)
       ↓
    Unpublished (not visible in public APIs)
```

**Chỉ templates có `Status = "Published"` mới hiển thị trong Public Discovery APIs**

---

## 🚀 Testing in Swagger

1. Mở Swagger UI: `http://localhost:5217/swagger`
2. Tìm section **PublicDiscovery**
3. Test các endpoint (không cần authorize cho public APIs)

---

## 💡 Frontend Integration Example

### React Example
```jsx
// Fetch categories for navigation
const fetchCategories = async () => {
  const response = await fetch('http://localhost:5217/api/discovery/categories');
  const categories = await response.json();
  setCategories(categories);
};

// Search templates
const searchTemplates = async (keyword) => {
  const response = await fetch(
    `http://localhost:5217/api/discovery/templates/search?q=${keyword}`
  );
  const data = await response.json();
  setSearchResults(data.results);
};

// Get template details
const getTemplateDetails = async (id) => {
  const response = await fetch(
    `http://localhost:5217/api/discovery/templates/${id}`
  );
  const template = await response.json();
  setTemplate(template);
};
```

---

## 🔥 Key Features

✅ **Public Access** - Không cần authentication  
✅ **Full-text Search** - Tìm kiếm theo name, description, tags  
✅ **Category Filtering** - Lọc theo danh mục  
✅ **Featured Templates** - Templates có rating cao nhất  
✅ **Newest Templates** - Templates mới publish  
✅ **Review Stats** - Hiển thị số lượng reviews và rating trung bình  
✅ **Published Only** - Chỉ hiển thị templates đã publish  

---

## 📝 Next Steps

Bạn có thể mở rộng thêm:
- Pagination cho danh sách templates
- Sorting options (price, rating, date)
- Filtering theo giá, rating
- Related templates
- Popular searches

**Happy Coding! 🎉** 