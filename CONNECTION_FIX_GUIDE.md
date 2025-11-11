# Hướng Dẫn Sửa Lỗi Connection Refused

## 🔴 VẤN ĐỀ

Frontend không thể kết nối đến Backend API:
- `ERR_CONNECTION_REFUSED` khi gọi API
- Frontend đang cố kết nối đến `https://localhost:7199` nhưng backend có thể chạy trên `http://localhost:5217`

## ✅ GIẢI PHÁP

### 1. **Kiểm Tra Backend Đang Chạy**

Backend có thể chạy trên:
- **HTTP**: `http://localhost:5217` (mặc định)
- **HTTPS**: `https://localhost:7199` (nếu chạy với profile "https")

**Cách kiểm tra:**
```bash
# Mở browser và truy cập:
http://localhost:5217/swagger
# hoặc
https://localhost:7199/swagger
```

### 2. **Cấu Hình Frontend**

Đã tạo file `.env` và `.env.development` với:
```
VITE_API_BASE_URL=http://localhost:5217
```

**Lưu ý:**
- Nếu backend chạy trên HTTPS (port 7199), đổi thành: `VITE_API_BASE_URL=https://localhost:7199`
- Sau khi sửa `.env`, **restart Vite dev server**:
  ```bash
  # Dừng server (Ctrl+C)
  # Chạy lại:
  npm run dev
  ```

### 3. **Sửa CORS và COOP trong Backend**

Đã cập nhật `Program.cs`:
- ✅ CORS: Cho phép `http://localhost:5173` (Vite default port)
- ✅ COOP: Set `same-origin-allow-popups` để Google OAuth hoạt động
- ✅ COEP: Set `unsafe-none` để tránh block resources

### 4. **Kiểm Tra Backend Đang Chạy**

**Chạy Backend:**
```bash
cd E:\eduprompt-be\Eduprompt.API
dotnet run
# hoặc
dotnet run --launch-profile http  # Chạy trên HTTP (port 5217)
dotnet run --launch-profile https # Chạy trên HTTPS (port 7199)
```

**Kiểm tra log:**
- Backend sẽ hiển thị URL đang chạy, ví dụ:
  ```
  Now listening on: http://localhost:5217
  Now listening on: https://localhost:7199
  ```

### 5. **Troubleshooting**

#### Nếu vẫn lỗi `ERR_CONNECTION_REFUSED`:

1. **Kiểm tra backend có đang chạy:**
   ```bash
   # PowerShell
   netstat -ano | findstr :5217
   netstat -ano | findstr :7199
   ```

2. **Kiểm tra firewall:**
   - Đảm bảo Windows Firewall không block port 5217 hoặc 7199

3. **Kiểm tra port đã bị chiếm:**
   ```bash
   # Nếu port bị chiếm, kill process:
   # Tìm PID từ netstat
   taskkill /PID <PID> /F
   ```

4. **Kiểm tra cấu hình API_BASE trong frontend:**
   - Mở DevTools → Console
   - Kiểm tra log: `VITE_API_BASE_URL = ...`
   - Đảm bảo đúng với backend URL

#### Nếu lỗi CORS:

1. **Kiểm tra CORS policy trong `Program.cs`:**
   - Đảm bảo có `http://localhost:5173` trong `WithOrigins()`

2. **Kiểm tra frontend URL:**
   - Frontend phải chạy trên `http://localhost:5173` (Vite default)
   - Nếu chạy trên port khác, thêm vào CORS policy

#### Nếu lỗi Google OAuth (COOP):

1. **Đã sửa COOP header:**
   - `Cross-Origin-Opener-Policy: same-origin-allow-popups`
   - `Cross-Origin-Embedder-Policy: unsafe-none`

2. **Kiểm tra Google OAuth redirect URI:**
   - Đảm bảo redirect URI trong Google Console là `http://localhost:5173`

---

## 📋 CHECKLIST

- [ ] Backend đang chạy (kiểm tra Swagger)
- [ ] File `.env` có `VITE_API_BASE_URL` đúng
- [ ] Restart Vite dev server sau khi sửa `.env`
- [ ] CORS policy cho phép frontend origin
- [ ] COOP header đã được set
- [ ] Port không bị chiếm bởi process khác
- [ ] Firewall không block port

---

## 🚀 QUICK FIX

**Nếu backend chạy trên HTTP (port 5217):**
1. Tạo file `.env` trong `E:\eduprompt-fe\`:
   ```
   VITE_API_BASE_URL=http://localhost:5217
   ```
2. Restart Vite: `npm run dev`
3. Kiểm tra backend: `http://localhost:5217/swagger`

**Nếu backend chạy trên HTTPS (port 7199):**
1. Tạo file `.env` trong `E:\eduprompt-fe\`:
   ```
   VITE_API_BASE_URL=https://localhost:7199
   ```
2. Restart Vite: `npm run dev`
3. Kiểm tra backend: `https://localhost:7199/swagger`

---

## ✅ KẾT LUẬN

Sau khi sửa:
1. ✅ Backend chạy trên port đúng
2. ✅ Frontend cấu hình đúng API_BASE_URL
3. ✅ CORS cho phép frontend origin
4. ✅ COOP header cho Google OAuth
5. ✅ Restart cả backend và frontend

**Code đã sẵn sàng!** 🎉

