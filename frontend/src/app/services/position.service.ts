import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { GpsPing } from '../models/gps-ping';
import { VehiclePosition } from '../models/vehicle-position';

@Injectable({
  providedIn: 'root'
})
export class PositionService {
  constructor(private http: HttpClient) { }

  lastKnown(): Observable<VehiclePosition[]> {
    return this.http.get<VehiclePosition[]>(`${environment.apiUrl}/positions`);
  }

  forVehicle(vehicleId: number): Observable<VehiclePosition> {
    return this.http.get<VehiclePosition>(`${environment.apiUrl}/vehicles/${vehicleId}/position`);
  }

  history(vehicleId: number, from: Date, to?: Date): Observable<GpsPing[]> {
    let params = new HttpParams().set('from', from.toISOString());
    if (to) {
      params = params.set('to', to.toISOString());
    }
    return this.http.get<GpsPing[]>(`${environment.apiUrl}/vehicles/${vehicleId}/pings`, { params });
  }
}
