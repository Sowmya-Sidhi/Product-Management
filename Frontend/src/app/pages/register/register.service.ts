import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RegisterService {
  private apiUrl = 'http://localhost:5000/api/auth';

  constructor(private http:HttpClient) { }
  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credentials);
  }

  getToken(): string | null {
    // Use sessionStorage to match `AuthService` behavior
    return sessionStorage.getItem('token');
  }

  saveToken(token: string) {
    sessionStorage.setItem('token', token);
  }

  logout() {
    sessionStorage.removeItem('token');
  }
}
