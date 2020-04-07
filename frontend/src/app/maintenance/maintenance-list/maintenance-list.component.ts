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
  @Input() currentOdometer = 0;

  logs: MaintenanceLog[] = [];
  loading = false;
  showForm = false;
  editing: MaintenanceLog | null = null;

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

  add(): void {
    this.editing = null;
    this.showForm = true;
  }

  edit(log: MaintenanceLog): void {
    this.editing = log;
    this.showForm = true;
  }

  onSaved(log: MaintenanceLog): void {
    const idx = this.logs.findIndex(l => l.id === log.id);
    if (idx >= 0) {
      this.logs[idx] = log;
    } else {
      this.logs = [log, ...this.logs];
    }
    this.closeForm();
  }

  remove(log: MaintenanceLog): void {
    if (!confirm(`Delete ${log.type} entry from ${log.date.substring(0, 10)}?`)) {
      return;
    }
    this.maintenanceService.delete(this.vehicleId, log.id).subscribe(() => {
      this.logs = this.logs.filter(l => l.id !== log.id);
    });
  }

  closeForm(): void {
    this.showForm = false;
    this.editing = null;
  }
}
