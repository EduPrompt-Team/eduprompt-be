/**
 * Google Authentication Client
 * Handles Google OAuth flow and token management
 */

class GoogleAuth {
    constructor(config) {
        this.apiBaseUrl = config.apiBaseUrl || 'https://localhost:7000/api';
        this.googleClientId = config.googleClientId;
        this.isInitialized = false;
        this.currentUser = null;
        this.accessToken = null;
        this.refreshToken = null;
        
        // Initialize Google API
        this.initializeGoogleAPI();
    }

    /**
     * Initialize Google API
     */
    async initializeGoogleAPI() {
        try {
            // Load Google API script
            await this.loadGoogleAPI();
            
            // Initialize Google API
            gapi.load('auth2', () => {
                gapi.auth2.init({
                    client_id: this.googleClientId,
                    scope: 'email profile'
                }).then(() => {
                    this.isInitialized = true;
                    console.log('Google API initialized successfully');
                    this.loadStoredTokens();
                });
            });
        } catch (error) {
            console.error('Failed to initialize Google API:', error);
        }
    }

    /**
     * Load Google API script
     */
    loadGoogleAPI() {
        return new Promise((resolve, reject) => {
            if (window.gapi) {
                resolve();
                return;
            }

            const script = document.createElement('script');
            script.src = 'https://apis.google.com/js/api.js';
            script.onload = resolve;
            script.onerror = reject;
            document.head.appendChild(script);
        });
    }

    /**
     * Load stored tokens from localStorage
     */
    loadStoredTokens() {
        try {
            const storedData = localStorage.getItem('eduprompt_auth');
            if (storedData) {
                const authData = JSON.parse(storedData);
                this.accessToken = authData.accessToken;
                this.refreshToken = authData.refreshToken;
                this.currentUser = authData.user;
                
                // Check if access token is expired
                if (this.isTokenExpired()) {
                    this.refreshAccessToken();
                }
            }
        } catch (error) {
            console.error('Failed to load stored tokens:', error);
            this.clearStoredTokens();
        }
    }

    /**
     * Store tokens in localStorage
     */
    storeTokens(authData) {
        try {
            const dataToStore = {
                accessToken: authData.accessToken,
                refreshToken: authData.refreshToken,
                user: authData.user,
                timestamp: Date.now()
            };
            localStorage.setItem('eduprompt_auth', JSON.stringify(dataToStore));
            
            this.accessToken = authData.accessToken;
            this.refreshToken = authData.refreshToken;
            this.currentUser = authData.user;
        } catch (error) {
            console.error('Failed to store tokens:', error);
        }
    }

    /**
     * Clear stored tokens
     */
    clearStoredTokens() {
        localStorage.removeItem('eduprompt_auth');
        this.accessToken = null;
        this.refreshToken = null;
        this.currentUser = null;
    }

    /**
     * Check if access token is expired
     */
    isTokenExpired() {
        if (!this.accessToken) return true;
        
        try {
            const payload = JSON.parse(atob(this.accessToken.split('.')[1]));
            const currentTime = Math.floor(Date.now() / 1000);
            return payload.exp < currentTime;
        } catch (error) {
            return true;
        }
    }

    /**
     * Sign in with Google
     */
    async signInWithGoogle() {
        if (!this.isInitialized) {
            throw new Error('Google API not initialized');
        }

        try {
            const authInstance = gapi.auth2.getAuthInstance();
            const googleUser = await authInstance.signIn();
            
            const idToken = googleUser.getAuthResponse().id_token;
            const accessToken = googleUser.getAuthResponse().access_token;

            // Send to backend for verification and token generation
            const response = await fetch(`${this.apiBaseUrl}/auth/google-login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    idToken: idToken,
                    accessToken: accessToken
                })
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.message || 'Google login failed');
            }

            const authData = await response.json();
            this.storeTokens(authData);
            
            return authData;
        } catch (error) {
            console.error('Google sign-in failed:', error);
            throw error;
        }
    }

    /**
     * Refresh access token
     */
    async refreshAccessToken() {
        if (!this.refreshToken) {
            throw new Error('No refresh token available');
        }

        try {
            const response = await fetch(`${this.apiBaseUrl}/auth/refresh-token`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    refreshToken: this.refreshToken
                })
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.message || 'Token refresh failed');
            }

            const authData = await response.json();
            this.storeTokens(authData);
            
            return authData;
        } catch (error) {
            console.error('Token refresh failed:', error);
            this.clearStoredTokens();
            throw error;
        }
    }

    /**
     * Sign out
     */
    async signOut() {
        try {
            // Revoke refresh token on server
            if (this.refreshToken) {
                await fetch(`${this.apiBaseUrl}/auth/revoke-token`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${this.accessToken}`
                    },
                    body: JSON.stringify({
                        refreshToken: this.refreshToken
                    })
                });
            }

            // Sign out from Google
            if (this.isInitialized) {
                const authInstance = gapi.auth2.getAuthInstance();
                await authInstance.signOut();
            }

            // Clear local storage
            this.clearStoredTokens();
        } catch (error) {
            console.error('Sign out failed:', error);
            // Still clear local storage even if server call fails
            this.clearStoredTokens();
        }
    }

    /**
     * Get current user
     */
    getCurrentUser() {
        return this.currentUser;
    }

    /**
     * Check if user is authenticated
     */
    isAuthenticated() {
        return this.currentUser !== null && this.accessToken !== null && !this.isTokenExpired();
    }

    /**
     * Get access token (refresh if needed)
     */
    async getAccessToken() {
        if (!this.isAuthenticated()) {
            throw new Error('User not authenticated');
        }

        if (this.isTokenExpired()) {
            await this.refreshAccessToken();
        }

        return this.accessToken;
    }

    /**
     * Make authenticated API request
     */
    async makeAuthenticatedRequest(url, options = {}) {
        const token = await this.getAccessToken();
        
        const headers = {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
            ...options.headers
        };

        return fetch(url, {
            ...options,
            headers
        });
    }
}

// Auto-initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Configuration - replace with your actual values
    const config = {
        apiBaseUrl: 'https://localhost:7000/api',
        googleClientId: 'YOUR_GOOGLE_CLIENT_ID_HERE' // Replace with your Google Client ID
    };

    // Initialize Google Auth
    window.googleAuth = new GoogleAuth(config);
    
    // Example usage
    window.signInWithGoogle = () => window.googleAuth.signInWithGoogle();
    window.signOut = () => window.googleAuth.signOut();
    window.getCurrentUser = () => window.googleAuth.getCurrentUser();
    window.isAuthenticated = () => window.googleAuth.isAuthenticated();
});

// Export for module usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = GoogleAuth;
}
