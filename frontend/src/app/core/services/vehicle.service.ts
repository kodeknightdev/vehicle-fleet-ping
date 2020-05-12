import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Vehicle } from '../models';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class VehicleService extends ApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  list(): Observable<Vehicle[]> {
    return this.http.get<Vehicle[]>(this.url('vehicles'));
  }

  get(id: number): Observable<Vehicle> {
    return this.http.get<Vehicle>(this.url(`vehicles/${id}`));
  }

  create(vehicle: Partial<Vehicle>): Observable<Vehicle> {
    return this.http.post<Vehicle>(this.url('vehicles'), vehicle);
  }

  update(id: number, vehicle: Partial<Vehicle>): Observable<void> {
    return this.http.put<void>(this.url(`vehicles/${id}`), vehicle);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(this.url(`vehicles/${id}`));
  }
}
