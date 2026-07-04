export interface Product {
  id: string;
  name: string;
  colour: string;
  price: number;
  createdAtUtc: string;
}

export interface ProductCreateRequest {
  name: string;
  colour: string;
  price: number;
}

export interface ProductQuery {
  colour?: string;
  minPrice?: number;
  maxPrice?: number;
  page?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
