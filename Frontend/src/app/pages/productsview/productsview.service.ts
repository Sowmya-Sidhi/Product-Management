import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductsviewService {
  private apiUrl = 'http://localhost:5000/api/products';
  private apiUrl1='http://localhost:5000/api/categories';

  constructor(private http: HttpClient) {}

  // Helper function to attach JWT token
  private getAuthHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('token'); // token saved during login
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    });
  }

  // GET all products
  getProducts(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl, {
      headers: this.getAuthHeaders()
    });
  }

  getCategories(): Observable<any[]> {
  return this.http.get<any[]>(this.apiUrl1,{
    headers:this.getAuthHeaders()
  
  });
}


  // POST add product
  addProduct(product: any): Observable<any> {
    return this.http.post(this.apiUrl, product, {
      headers: this.getAuthHeaders()
    });
  }

  // PUT update product
  updateProduct(code: string, product: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${code}`, product, {
      headers: this.getAuthHeaders()
    });
  }
  
  //  DELETE product
  deleteProduct(code: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${code}`, {
      headers: this.getAuthHeaders(),
      responseType: 'text' as 'json' // fix JSON parse issue
    });
  }
}
