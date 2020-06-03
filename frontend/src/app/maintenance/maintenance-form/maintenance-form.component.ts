import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { MAINTENANCE_TYPES, MaintenanceLog } from '../../core/models';
import { MaintenanceService } from '../../core/services';

@Component({
  selector: 'app-maintenance-form',
  templateUrl: './maintenance-form.component.html',
  styleUrls: ['./maintenance-form.component.scss']
})
export class MaintenanceFormComponent implements OnInit {
  @Input() vehicleId: number;
  @Input() log: MaintenanceLog | null = null;
  @Input() currentOdometer = 0;
  @Output() saved = new EventEmitter<MaintenanceLog>();
  @Output() cancelled = new EventEmitter<void>();

  types = MAINTENANCE_TYPES;
  saving = false;
  error = '';
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private maintenanceService: MaintenanceService
  ) { }

  ngOnInit(): void {
    const log = this.log;
    this.form = this.fb.group({
      date: [log ? log.date.substring(0, 10) : this.today(), Validators.required],
      type: [log ? log.type : 'OilChange', Validators.required],
      odometerKm: [log ? log.odometerKm : this.currentOdometer, [Validators.required, Validators.min(0)]],
      cost: [log ? log.cost : 0, [Validators.required, Validators.min(0)]],
      mechanic: [log ? log.mechanic : '', Validators.maxLength(80)],
      notes: [log ? log.notes : '', Validators.maxLength(1000)]
    });
  }

  get isEdit(): boolean {
    return !!this.log;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    // Send the date as a plain yyyy-MM-dd. Turning it into a Date made it UTC midnight,
    // which showed up as the previous day for anyone west of Greenwich.
    const payload: Partial<MaintenanceLog> = { ...this.form.value };

    this.saving = true;
    this.error = '';

    if (this.log) {
      const id = this.log.id;
      this.maintenanceService.update(this.vehicleId, id, payload).subscribe(
        () => this.done({ ...this.log, ...payload, id } as MaintenanceLog),
        () => this.fail()
      );
    } else {
      this.maintenanceService.create(this.vehicleId, payload).subscribe(
        created => this.done(created),
        () => this.fail()
      );
    }
  }

  private today(): string {
    const d = new Date();
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`;
  }

  private done(log: MaintenanceLog): void {
    this.saving = false;
    this.saved.emit(log);
  }

  private fail(): void {
    this.saving = false;
    this.error = 'Saving failed. Check the values and try again.';
  }
}
