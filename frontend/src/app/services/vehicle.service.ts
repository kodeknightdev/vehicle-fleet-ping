import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { Vehicle } from '../models/vehicle';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private readonly url = `${environment.apiUrl}/vehicles`;

  constructor(private http: HttpClient) { }

  list(): Observable<Vehicle[]> {
    return this.http.get<Vehicle[]>(this.url);
  }

  get(id: number): Observable<Vehicle> {
    return this.http.get<Vehicle>(`${this.url}/${id}`);
  }

  create(vehicle: Partial<Vehicle>): Observable<Vehicle> {
    return this.http.post<Vehicle>(this.url, vehicle);
  }

  update(id: number, vehicle: Partial<Vehicle>): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, vehicle);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
