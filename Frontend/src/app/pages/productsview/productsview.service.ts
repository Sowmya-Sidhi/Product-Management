import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductsviewService {
  private apiUrl = 'http://localhost:5000/api/products';
  private apiUrl1='http://localhost:5000/api/categories';

  constructor(private http: HttpClient) {}

  // Authorization header is added by `authInterceptor` globally.

  // GET all products
  getProducts(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getCategories(): Observable<any[]> {
  return this.http.get<any[]>(this.apiUrl1);
}


  // POST add product
  addProduct(product: any): Observable<any> {
    return this.http.post(this.apiUrl, product);
  }

  // PUT update product
  updateProduct(code: string, product: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${code}`, product);
  }
  
  //  DELETE product
  deleteProduct(code: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${code}`, { responseType: 'text' as 'json' });
  }
}
