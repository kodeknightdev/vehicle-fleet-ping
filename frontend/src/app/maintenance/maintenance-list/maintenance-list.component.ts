import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

import { MAINTENANCE_TYPES, MaintenanceLog } from '../../core/models';
import { MaintenanceService } from '../../core/services';

@Component({
  selector: 'app-maintenance-list',
  templateUrl: './maintenance-list.component.html',
  styleUrls: ['./maintenance-list.component.scss']
})
export class MaintenanceListComponent implements OnInit, OnChanges, OnDestroy {
  @Input() vehicleId: number;
  @Input() currentOdometer = 0;

  logs: MaintenanceLog[] = [];
  loading = false;
  showForm = false;
  editing: MaintenanceLog | null = null;

  types = MAINTENANCE_TYPES;
  filters = this.fb.group({ type: [''], from: [''], to: [''] });
  private filterSub: Subscription;

  constructor(
    private fb: FormBuilder,
    private maintenanceService: MaintenanceService
  ) { }

  ngOnInit(): void {
    this.filterSub = this.filters.valueChanges
      .pipe(debounceTime(250))
      .subscribe(() => this.load());
  }

  ngOnChanges(): void {
    this.load();
  }

  ngOnDestroy(): void {
    if (this.filterSub) {
      this.filterSub.unsubscribe();
    }
  }

  get filtered(): boolean {
    const f = this.filters.value;
    return !!(f.type || f.from || f.to);
  }

  clearFilters(): void {
    this.filters.setValue({ type: '', from: '', to: '' });
  }

  get totalCost(): number {
    return this.logs.reduce((sum, l) => sum + l.cost, 0);
  }

  load(): void {
    if (!this.vehicleId) {
      return;
    }
    this.loading = true;
    this.maintenanceService.list(this.vehicleId, this.filters.value).subscribe(logs => {
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
