import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductsviewService {
 private apiUrl='http://localhost:5000/api/products';
 constructor(private http: HttpClient) {}

  getProducts(): Observable<any[]> {
    //  GET request to backend
    return this.http.get<any[]>(this.apiUrl);
  }

  addProduct(product: any): Observable<any> {
    //  POST request to backend
    return this.http.post(this.apiUrl, product);
  }

  deleteProduct(id: string) {
  // Add responseType: 'text' to avoid JSON parse error
  return this.http.delete(`${this.apiUrl}/${id}`, { responseType: 'text' });
}


  updateProduct(id: string, product: any): Observable<any> {
    //  PUT request to backend
    return this.http.put(`${this.apiUrl}/${id}`, product);
  }
  
}
