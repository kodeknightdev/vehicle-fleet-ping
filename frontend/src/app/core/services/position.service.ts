import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { GpsPing, VehiclePosition } from '../models';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class PositionService extends ApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  lastKnown(): Observable<VehiclePosition[]> {
    return this.http.get<VehiclePosition[]>(this.url('positions'));
  }

  forVehicle(vehicleId: number): Observable<VehiclePosition> {
    return this.http.get<VehiclePosition>(this.url(`vehicles/${vehicleId}/position`));
  }

  history(vehicleId: number, from: Date, to?: Date): Observable<GpsPing[]> {
    let params = new HttpParams().set('from', from.toISOString());
    if (to) {
      params = params.set('to', to.toISOString());
    }
    return this.http.get<GpsPing[]>(this.url(`vehicles/${vehicleId}/pings`), { params });
  }
}
