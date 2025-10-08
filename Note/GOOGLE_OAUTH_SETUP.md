# Google OAuth Setup Guide

Hướng dẫn thiết lập Google OAuth cho Eduprompt API với access token và refresh token.

## 🔧 Backend Setup

### 1. Cài đặt Packages
Các packages đã được thêm vào `Eduprompt.API.csproj`:
- `Microsoft.AspNetCore.Authentication.Google` (8.0.5)
- `Google.Apis.Auth` (1.68.0)

### 2. Cấu hình appsettings.json
Cập nhật `appsettings.json` với thông tin Google OAuth:

```json
{
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID_HERE",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET_HERE"
  }
}
```

### 3. Database Migration
Cần tạo migration để thêm các trường mới vào User table:

```sql
ALTER TABLE Users ADD GoogleId NVARCHAR(255) NULL;
ALTER TABLE Users ADD RefreshToken NVARCHAR(MAX) NULL;
ALTER TABLE Users ADD RefreshTokenExpiryTime DATETIME2 NULL;
```

## 🌐 Google Cloud Console Setup

### 1. Tạo Project
1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo project mới hoặc chọn project hiện có
3. Đặt tên project: "Eduprompt"

### 2. Enable APIs
1. Vào **APIs & Services** > **Library**
2. Tìm và enable:
   - **Google+ API** (hoặc **Google Identity API**)
   - **Google OAuth2 API**

### 3. Tạo OAuth 2.0 Credentials
1. Vào **APIs & Services** > **Credentials**
2. Click **Create Credentials** > **OAuth 2.0 Client IDs**
3. Chọn **Web application**
4. Đặt tên: "Eduprompt Web Client"

### 4. Cấu hình Authorized Origins
Thêm các origins sau:
- `http://localhost:5000`
- `https://localhost:7000`
- `http://localhost:3000` (nếu có frontend riêng)
- Domain production của bạn

### 5. Cấu hình Authorized Redirect URIs
Thêm các redirect URIs:
- `http://localhost:5000/signin-google`
- `https://localhost:7000/signin-google`
- Domain production của bạn + `/signin-google`

### 6. Lấy Credentials
1. Sau khi tạo xong, copy **Client ID** và **Client Secret**
2. Cập nhật vào `appsettings.json`

## 🚀 API Endpoints

### Authentication Endpoints

#### 1. Google Login
```http
POST /api/auth/google-login
Content-Type: application/json

{
  "idToken": "google_id_token",
  "accessToken": "google_access_token"
}
```

**Response:**
```json
{
  "accessToken": "jwt_access_token",
  "refreshToken": "refresh_token",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "user": {
    "userId": 1,
    "fullName": "John Doe",
    "email": "john@example.com",
    "role": "User"
  }
}
```

#### 2. Refresh Token
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "refreshToken": "refresh_token"
}
```

#### 3. Revoke Token (Logout)
```http
POST /api/auth/revoke-token
Authorization: Bearer {access_token}
Content-Type: application/json

{
  "refreshToken": "refresh_token"
}
```

#### 4. Get Current User
```http
GET /api/auth/me
Authorization: Bearer {access_token}
```

## 💻 Client-Side Implementation

### 1. HTML Setup
```html
<!-- Load Google API -->
<script src="https://apis.google.com/js/api.js"></script>
<script src="/js/google-auth.js"></script>
```

### 2. JavaScript Usage
```javascript
// Initialize (auto-initialized)
const googleAuth = new GoogleAuth({
    apiBaseUrl: 'https://localhost:7000/api',
    googleClientId: 'YOUR_GOOGLE_CLIENT_ID'
});

// Sign in
await googleAuth.signInWithGoogle();

// Check authentication
if (googleAuth.isAuthenticated()) {
    const user = googleAuth.getCurrentUser();
    const token = await googleAuth.getAccessToken();
}

// Make authenticated requests
const response = await googleAuth.makeAuthenticatedRequest('/api/auth/me');

// Sign out
await googleAuth.signOut();
```

### 3. Token Management
- **Access Token**: Tự động refresh khi hết hạn
- **Refresh Token**: Lưu trong localStorage, tự động quản lý
- **User Info**: Lưu trong localStorage, tự động sync

## 🔒 Security Features

### 1. Token Security
- Access token có thời hạn 1 giờ
- Refresh token có thời hạn 7 ngày
- Tự động refresh access token khi cần
- Revoke refresh token khi logout

### 2. Google Token Verification
- Verify Google ID token trên server
- Validate Google access token
- Kiểm tra email verification status

### 3. Database Security
- Lưu trữ GoogleId để liên kết tài khoản
- Refresh token được mã hóa trong database
- Tự động cleanup expired tokens

## 🧪 Testing

### 1. Demo Page
Truy cập: `https://localhost:7000/google-auth-demo.html`

### 2. Test Flow
1. Click "Sign in with Google"
2. Complete Google OAuth flow
3. Verify user info hiển thị
4. Test API calls với authenticated requests
5. Test refresh token functionality
6. Test logout

### 3. API Testing với Swagger
1. Truy cập: `https://localhost:7000/swagger`
2. Sử dụng `/api/auth/google-login` để lấy token
3. Authorize với token trong Swagger UI
4. Test các endpoints khác

## 🐛 Troubleshooting

### 1. Common Issues

#### "Google API not initialized"
- Kiểm tra Google Client ID đã đúng chưa
- Kiểm tra network connection
- Kiểm tra console errors

#### "Invalid Google ID token"
- Kiểm tra Google Client ID trong appsettings.json
- Kiểm tra Google Cloud Console configuration
- Kiểm tra authorized origins

#### "Token refresh failed"
- Kiểm tra refresh token còn valid không
- Kiểm tra database connection
- Kiểm tra JWT configuration

### 2. Debug Steps
1. Kiểm tra browser console logs
2. Kiểm tra network requests trong DevTools
3. Kiểm tra server logs
4. Verify Google Cloud Console settings

## 📚 Additional Resources

- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [Google Identity Platform](https://developers.google.com/identity)
- [JWT Bearer Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/)

## ✅ Checklist

- [ ] Google Cloud Console project created
- [ ] Google OAuth 2.0 credentials created
- [ ] Authorized origins configured
- [ ] Authorized redirect URIs configured
- [ ] Client ID và Secret updated in appsettings.json
- [ ] Database migration applied
- [ ] API server running
- [ ] Demo page accessible
- [ ] Google sign-in working
- [ ] Token refresh working
- [ ] API calls with authentication working
- [ ] Logout working
