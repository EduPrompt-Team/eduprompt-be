# Supabase Storage Setup Guide

Hướng dẫn thiết lập Supabase Storage cho Eduprompt API để quản lý file upload.

## 🚀 Bước 1: Tạo Supabase Project

1. Truy cập [Supabase Dashboard](https://supabase.com/dashboard)
2. Click **"New Project"**
3. Chọn organization và đặt tên project: "Eduprompt"
4. Chọn database password mạnh
5. Chọn region gần nhất (Singapore cho Việt Nam)
6. Click **"Create new project"**

## 🔧 Bước 2: Lấy Credentials

1. Vào **Settings** > **API**
2. Copy các thông tin sau:
   - **Project URL** (ví dụ: `https://your-project.supabase.co`)
   - **anon public** key
   - **service_role** key (⚠️ Bảo mật cao!)

## 📁 Bước 3: Tạo Storage Buckets

1. Vào **Storage** trong sidebar
2. Click **"New bucket"**
3. Tạo các buckets sau:

### Bucket: `documents`
- **Name**: `documents`
- **Public**: ✅ (để có thể truy cập public URL)
- **File size limit**: 50MB
- **Allowed MIME types**: `image/*, application/pdf, text/*`

### Bucket: `avatars`
- **Name**: `avatars`
- **Public**: ✅
- **File size limit**: 5MB
- **Allowed MIME types**: `image/*`

### Bucket: `prompt-templates`
- **Name**: `prompt-templates`
- **Public**: ✅
- **File size limit**: 10MB
- **Allowed MIME types**: `image/*, application/pdf, text/*`

## ⚙️ Bước 4: Cấu hình API

### Cập nhật appsettings.json
```json
{
  "Supabase": {
    "Url": "https://your-project.supabase.co",
    "ServiceRoleKey": "your-service-role-key-here",
    "AnonKey": "your-anon-key-here"
  }
}
```

### Cấu hình RLS Policies (Row Level Security)

1. Vào **Authentication** > **Policies**
2. Tạo policies cho storage:

```sql
-- Policy cho documents bucket
CREATE POLICY "Users can upload documents" ON storage.objects
FOR INSERT WITH CHECK (bucket_id = 'documents' AND auth.role() = 'authenticated');

CREATE POLICY "Users can view documents" ON storage.objects
FOR SELECT USING (bucket_id = 'documents');

CREATE POLICY "Users can delete their documents" ON storage.objects
FOR DELETE USING (bucket_id = 'documents' AND auth.uid()::text = (storage.foldername(name))[1]);

-- Policy cho avatars bucket
CREATE POLICY "Users can upload avatars" ON storage.objects
FOR INSERT WITH CHECK (bucket_id = 'avatars' AND auth.role() = 'authenticated');

CREATE POLICY "Anyone can view avatars" ON storage.objects
FOR SELECT USING (bucket_id = 'avatars');

CREATE POLICY "Users can update their avatars" ON storage.objects
FOR UPDATE USING (bucket_id = 'avatars' AND auth.uid()::text = (storage.foldername(name))[1]);
```

## 🧪 Bước 5: Test API

### 1. Upload File
```bash
curl -X POST "https://localhost:7000/api/storage/upload" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@test.jpg" \
  -F "bucketName=documents" \
  -F "folderPath=user-uploads"
```

### 2. Get File List
```bash
curl -X GET "https://localhost:7000/api/storage/list?bucketName=documents" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 3. Get Public URL
```bash
curl -X GET "https://localhost:7000/api/storage/url?bucketName=documents&fileName=test.jpg"
```

## 📋 API Endpoints

### Storage Controller

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/storage/upload` | Upload single file | ✅ |
| POST | `/api/storage/upload-multiple` | Upload multiple files | ✅ |
| DELETE | `/api/storage/delete` | Delete file | ✅ |
| GET | `/api/storage/list` | Get file list | ✅ |
| GET | `/api/storage/url` | Get public URL | ❌ |
| GET | `/api/storage/exists` | Check file exists | ✅ |

### Request Examples

#### Upload File
```javascript
const formData = new FormData();
formData.append('file', fileInput.files[0]);
formData.append('bucketName', 'documents');
formData.append('folderPath', 'user-uploads');

fetch('/api/storage/upload', {
  method: 'POST',
  headers: {
    'Authorization': 'Bearer ' + token
  },
  body: formData
})
.then(response => response.json())
.then(data => console.log(data));
```

#### Delete File
```javascript
fetch('/api/storage/delete', {
  method: 'DELETE',
  headers: {
    'Authorization': 'Bearer ' + token,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    bucketName: 'documents',
    fileName: 'file-to-delete.jpg'
  })
});
```

## 🔒 Security Features

### 1. File Validation
- ✅ File size limit (10MB default)
- ✅ File type validation
- ✅ Unique file names (UUID prefix)
- ✅ Authentication required

### 2. Bucket Organization
- ✅ Separate buckets for different file types
- ✅ Folder structure support
- ✅ Public/private access control

### 3. Error Handling
- ✅ Comprehensive error messages
- ✅ Logging for debugging
- ✅ Graceful failure handling

## 📊 Monitoring & Logs

### 1. Supabase Dashboard
- Monitor storage usage
- View file uploads/downloads
- Check error logs

### 2. Application Logs
```csharp
_logger.LogInformation("File uploaded: {FileName} to bucket: {BucketName}", fileName, bucketName);
_logger.LogError(ex, "Upload failed: {FileName}", fileName);
```

## 🚨 Troubleshooting

### Common Issues

#### 1. "Invalid credentials"
- ✅ Check Supabase URL and keys in appsettings.json
- ✅ Verify service_role key (not anon key)

#### 2. "Bucket not found"
- ✅ Create buckets in Supabase Dashboard
- ✅ Check bucket name spelling

#### 3. "File too large"
- ✅ Check file size limits in bucket settings
- ✅ Update API validation limits

#### 4. "Permission denied"
- ✅ Check RLS policies
- ✅ Verify user authentication
- ✅ Check bucket public/private settings

### Debug Steps
1. Check Supabase Dashboard logs
2. Check application logs
3. Verify network connectivity
4. Test with small files first
5. Check CORS settings

## 📚 Additional Resources

- [Supabase Storage Documentation](https://supabase.com/docs/guides/storage)
- [Supabase C# Client](https://github.com/supabase/supabase-csharp)
- [File Upload Best Practices](https://supabase.com/docs/guides/storage/security/access-control)

## ✅ Checklist

- [ ] Supabase project created
- [ ] Storage buckets created
- [ ] RLS policies configured
- [ ] API credentials updated
- [ ] File upload working
- [ ] File deletion working
- [ ] Public URLs accessible
- [ ] Error handling tested
- [ ] Security policies verified
