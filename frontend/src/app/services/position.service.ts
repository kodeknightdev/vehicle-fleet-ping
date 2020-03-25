import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
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
}
