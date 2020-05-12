import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { DashboardSummary } from '../models';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class DashboardService extends ApiService {
  constructor(http: HttpClient) {
    super(http);
  }

  summary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(this.url('dashboard'));
  }
}
