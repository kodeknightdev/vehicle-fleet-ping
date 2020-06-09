import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { MaintenanceLog, MaintenanceType } from '../models';
import { ApiService } from './api.service';

export interface MaintenanceFilter {
  type?: MaintenanceType | '';
  from?: string;
  to?: string;
}

@Injectable({
  providedIn: 'root'
})
export class MaintenanceService extends ApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  list(vehicleId: number, filter: MaintenanceFilter = {}): Observable<MaintenanceLog[]> {
    let params = new HttpParams();
    if (filter.type) {
      params = params.set('type', filter.type);
    }
    if (filter.from) {
      params = params.set('from', filter.from);
    }
    if (filter.to) {
      params = params.set('to', filter.to);
    }
    return this.http.get<MaintenanceLog[]>(this.logsUrl(vehicleId), { params });
  }

  get(vehicleId: number, id: number): Observable<MaintenanceLog> {
    return this.http.get<MaintenanceLog>(`${this.logsUrl(vehicleId)}/${id}`);
  }

  create(vehicleId: number, log: Partial<MaintenanceLog>): Observable<MaintenanceLog> {
    return this.http.post<MaintenanceLog>(this.logsUrl(vehicleId), log);
  }

  update(vehicleId: number, id: number, log: Partial<MaintenanceLog>): Observable<void> {
    return this.http.put<void>(`${this.logsUrl(vehicleId)}/${id}`, log);
  }

  delete(vehicleId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.logsUrl(vehicleId)}/${id}`);
  }

  private logsUrl(vehicleId: number): string {
    return this.url(`vehicles/${vehicleId}/maintenance`);
  }
}
