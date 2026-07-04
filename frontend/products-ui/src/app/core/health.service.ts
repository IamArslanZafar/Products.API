import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api-config';

export interface HealthStatus {
  status: string;
  uptime: string;
}

@Injectable({ providedIn: 'root' })
export class HealthService {
  constructor(private readonly http: HttpClient) {}

  check(): Observable<HealthStatus> {
    return this.http.get<HealthStatus>(`${API_BASE_URL}/health`);
  }
}
