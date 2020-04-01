import { Component, Input, OnChanges } from '@angular/core';

import { MaintenanceLog } from '../../models/maintenance-log';
import { MaintenanceService } from '../../services/maintenance.service';

@Component({
  selector: 'app-maintenance-list',
  templateUrl: './maintenance-list.component.html',
  styleUrls: ['./maintenance-list.component.scss']
})
export class MaintenanceListComponent implements OnChanges {
  @Input() vehicleId: number;

  logs: MaintenanceLog[] = [];
  loading = false;

  constructor(private maintenanceService: MaintenanceService) { }

  ngOnChanges(): void {
    this.load();
  }

  get totalCost(): number {
    return this.logs.reduce((sum, l) => sum + l.cost, 0);
  }

  load(): void {
    if (!this.vehicleId) {
      return;
    }
    this.loading = true;
    this.maintenanceService.list(this.vehicleId).subscribe(logs => {
      this.logs = logs;
      this.loading = false;
    });
  }
}
