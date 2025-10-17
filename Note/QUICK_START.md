# Quick Start - API Fixes

## ⚡ Rapid Setup (3 Steps)

### Step 1: Run Database Scripts
Open SSMS and execute in order:
```sql
-- 1. Add new columns to Posts table
USE Eduprompt;
EXEC('D:/eduprompt-be/Note/add_post_fields.sql')

-- 2. Seed development data
EXEC('D:/eduprompt-be/Note/seed_dev.sql')
```

### Step 2: Rebuild Backend
```powershell
cd D:\eduprompt-be\Eduprompt.API
dotnet clean
dotnet build
dotnet run
```

### Step 3: Test Accounts
Login with these credentials:
- `admin@eduprompt.dev` / `Password@123`
- `teacher01@school.edu` / `Password@123`

## ✅ What Was Fixed

| Issue | Status | Details |
|-------|--------|---------|
| 400 GET AIHistory | ✅ Fixed | Added GetAll endpoint |
| 400 GET Transaction | ✅ Fixed | Added GetAll endpoint |
| 400 GET PaymentMethod | ✅ Fixed | Added GetAll endpoint |
| 404 GET Order/{id} | ✅ Working | Returns 404 correctly |
| 500 GET storage-templates/my-storage | ✅ Fixed | Added Package include |
| 500 GET ExpectedOutput/instance/{id} | ✅ Fixed | Added proper seeding |
| 500 GET TemplateArchitecture/instance/{id} | ✅ Fixed | Added proper seeding |
| 500 POST storage-templates | ✅ Fixed | Added Package validation |
| 500 PUT Users/{id} | ✅ Working | DTO mapping correct |
| Post search case-sensitive | ✅ Fixed | Now case-insensitive |
| Post null postType/tags | ✅ Fixed | Added fields to entity |
| Wishlist check returns false | ✅ Fixed | Added seed data |

## 📝 Files Changed

### Controllers (3 files)
- AIHistoryController.cs - Added GetAll
- TransactionController.cs - Added GetAll
- PaymentMethodController.cs - Added GetAll

### Services (2 files)
- PostService.cs - Fixed search, added PostType/Tags
- StorageTemplateService.cs - Added Package validation

### Repositories (1 file)
- StorageTemplateRepository.cs - Added Package includes

### Entities (1 file)
- Post.cs - Added PostType and Tags properties

### Database Scripts (2 files)
- add_post_fields.sql - Migration for Post table
- seed_dev.sql - Comprehensive test data

## 🔧 Quick Test Commands

### Test GET Endpoints (Swagger or Postman)
```
GET /api/AIHistory (Authorization: Bearer {admin_token})
GET /api/Transaction (Authorization: Bearer {admin_token})
GET /api/PaymentMethod (Authorization: Bearer {admin_token})
GET /api/storage-templates/my-storage (Authorization: Bearer {user_token})
GET /api/Post/search?searchTerm=toán
GET /api/Wishlists/check/1
```

### Test POST/PUT
```
POST /api/storage-templates
Body: { "packageID": 1 }

PUT /api/Users/1
Body: { "fullName": "Updated Name", "phone": "+84 999 999 999" }
```

## 📚 Full Documentation
See `API_FIXES_SUMMARY.md` for complete details.

