import { Component, OnInit } from '@angular/core';

import { DashboardSummary } from '../models/dashboard-summary';
import { DashboardService } from '../services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  summary: DashboardSummary | null = null;
  error = '';

  constructor(private dashboardService: DashboardService) { }

  ngOnInit(): void {
    this.dashboardService.summary().subscribe(
      s => this.summary = s,
      () => this.error = 'Could not load the dashboard.'
    );
  }
}
