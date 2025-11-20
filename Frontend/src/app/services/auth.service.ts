import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'http://localhost:5000/api/auth';

  constructor(private http: HttpClient) {}

  // ===== API CALLS =====
  login(credentials: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, credentials);
  }

  register(userData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, userData);
  }

  // ===== TOKEN HANDLING =====
  saveToken(token: string) {
    sessionStorage.setItem('token', token);

    const decoded = this.decodeToken(token);
    const role =
      decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      decoded?.role ||
      null;

    if (role) {
      sessionStorage.setItem('role', role);
    }
  }
  resetPassword(email: string, newPassword: string) {
  // Backend reset endpoint lives under /api/users/reset-password
  return this.http.post(`http://localhost:5000/api/users/reset-password`, { email, newPassword });
}


  getToken(): string | null {
    return this.isBrowser() ? sessionStorage.getItem('token') : null;
  }

  logout() {
    if (this.isBrowser()) {
      sessionStorage.removeItem('token');
      sessionStorage.removeItem('role');
    }
  }

  // ===== ROLE HANDLING =====
  getRole(): string | null {
    return this.isBrowser() ? sessionStorage.getItem('role') : null;
  }

  // ===== TOKEN DECODING =====
  private decodeToken(token: string): any {
    try {
      const payload = token.split('.')[1];
      return JSON.parse(atob(payload));
    } catch (e) {
      console.error('Error decoding token', e);
      return null;
    }
  }

  // Optional: if you still need it separately
  getUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;

    const decoded = this.decodeToken(token);
    return decoded?.sub || decoded?.nameid || null;
  }


  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  // ===== Helper to check if running in browser (avoid SSR issues) =====
  private isBrowser(): boolean {
    return typeof window !== 'undefined' && !!window.sessionStorage;
  }
}
