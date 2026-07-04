import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-config';
import { PagedResult, Product, ProductCreateRequest, ProductQuery } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private readonly http: HttpClient) {}

  getAll(query: ProductQuery = {}): Observable<PagedResult<Product>> {
    let params = new HttpParams();

    if (query.colour) {
      params = params.set('colour', query.colour);
    }
    if (query.minPrice != null) {
      params = params.set('minPrice', query.minPrice);
    }
    if (query.maxPrice != null) {
      params = params.set('maxPrice', query.maxPrice);
    }
    params = params.set('page', query.page ?? 1);
    params = params.set('pageSize', query.pageSize ?? 20);

    return this.http.get<PagedResult<Product>>(`${API_BASE_URL}/products`, { params });
  }

  create(product: ProductCreateRequest): Observable<Product> {
    return this.http.post<Product>(`${API_BASE_URL}/products`, product);
  }
}
