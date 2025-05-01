import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { product } from '../../shared/models/product';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  baseUrl = 'https://localhost:5001/api/';
  private http = inject(HttpClient);
  types: string[] = [];
  brands: string[] = [];

  getProducts(shopParams: ShopParams) {
    let params = new HttpParams(); //this allow to us to build query string

    if (shopParams.brands.length > 0)
      params = params.append('brands', shopParams.brands.join(','));

    if (shopParams.types.length > 0)
      params = params.append('types', shopParams.types.join(','));

    if (shopParams.sort.length > 0)
      params = params.append('sort', shopParams.sort);

    if (shopParams.search) params = params.append('search', shopParams.search);

    params = params.append('pageSize', shopParams.pageSize);

    params = params.append('pageIndex', shopParams.pageNumber);

    return this.http.get<Pagination<product>>(this.baseUrl + 'Products', {
      params,
    });
  }

  getBrands() {
    if (this.brands.length > 0) return; //will execute one time when application start
    return this.http.get<string[]>(this.baseUrl + 'products/brands').subscribe({
      next: (response) => (this.brands = response),
      error(err) {
        console.log('get brand error');
      },
    });
  }

  getTypes() {
    if (this.types.length > 0) return;
    return this.http.get<string[]>(this.baseUrl + 'products/types').subscribe({
      next: (response) => (this.types = response),
    });
  }
}
