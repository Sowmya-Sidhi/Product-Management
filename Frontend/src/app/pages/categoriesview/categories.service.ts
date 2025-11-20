import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {
  private apiUrl = 'http://localhost:5000/api/categories';

  constructor(private http: HttpClient) {}

  // ✅ GET all categories
  getCategories(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  // ✅ POST add category
  addCategory(category: any): Observable<any> {
    return this.http.post(this.apiUrl, category);
  }

  // ✅ PUT update category
  updateCategory(id: string, category: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, category);
  }

  // ✅ DELETE category
  deleteCategory(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { responseType: 'text' as 'json' });
  }
}
