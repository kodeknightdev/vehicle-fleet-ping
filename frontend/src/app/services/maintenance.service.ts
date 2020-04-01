import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { MaintenanceLog } from '../models/maintenance-log';

@Injectable({
  providedIn: 'root'
})
export class MaintenanceService {
  constructor(private http: HttpClient) { }

  list(vehicleId: number): Observable<MaintenanceLog[]> {
    return this.http.get<MaintenanceLog[]>(this.url(vehicleId));
  }

  get(vehicleId: number, id: number): Observable<MaintenanceLog> {
    return this.http.get<MaintenanceLog>(`${this.url(vehicleId)}/${id}`);
  }

  create(vehicleId: number, log: Partial<MaintenanceLog>): Observable<MaintenanceLog> {
    return this.http.post<MaintenanceLog>(this.url(vehicleId), log);
  }

  update(vehicleId: number, id: number, log: Partial<MaintenanceLog>): Observable<void> {
    return this.http.put<void>(`${this.url(vehicleId)}/${id}`, log);
  }

  delete(vehicleId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.url(vehicleId)}/${id}`);
  }

  private url(vehicleId: number): string {
    return `${environment.apiUrl}/vehicles/${vehicleId}/maintenance`;
  }
}
