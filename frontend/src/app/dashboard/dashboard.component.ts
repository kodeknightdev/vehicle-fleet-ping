import { Component, OnInit } from '@angular/core';

import { DashboardSummary } from '../core/models';
import { DashboardService } from '../core/services';

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
