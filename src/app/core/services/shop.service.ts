import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { product } from '../../shared/models/product';

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  baseUrl = 'https://localhost:7196/api/';
  // constructor(private _HttpClient: HttpClient) {} // old inject
  private http = inject(HttpClient);
  types: string[] = [];
  brands: string[] = [];

  getProducts() {
    // return this.http.get<Pagination<product>>(this.baseUrl + 'Products');
    return this.http.get<Pagination<product>>(
      this.baseUrl + 'Products?PageSize=20'
    );
  }

  getBrand() {
    if (this.brands.length > 0) return; //will execute one time when application start
    return this.http.get<any[]>(this.baseUrl + 'Products/Brands').subscribe({
      next: (response) => (this.brands = response.map((obj) => obj.name)),
      error(err) {
        console.log('get brand error');
      },
    });
  }

  getTypes() {
    if (this.types.length > 0) return;
    return this.http.get<any[]>(this.baseUrl + 'Products/Types').subscribe({
      next: (response) => (this.types = response.map((obj) => obj.name)),
    });
  }
}
