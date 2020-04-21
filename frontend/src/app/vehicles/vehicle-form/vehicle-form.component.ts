import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';

import { Vehicle, VEHICLE_STATUSES } from '../../models/vehicle';
import { VehicleService } from '../../services/vehicle.service';

@Component({
  selector: 'app-vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.scss']
})
export class VehicleFormComponent implements OnInit {
  statuses = VEHICLE_STATUSES;
  vehicleId: number | null = null;
  saving = false;
  error = '';

  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(60)]],
    plate: ['', [Validators.required, Validators.maxLength(16)]],
    make: ['', Validators.maxLength(40)],
    model: ['', Validators.maxLength(40)],
    year: [new Date().getFullYear(), [Validators.required, Validators.min(1980), Validators.max(2100)]],
    odometerKm: [0, [Validators.required, Validators.min(0)]],
    serviceIntervalKm: [15000, [Validators.required, Validators.min(1000), Validators.max(100000)]],
    status: ['Active', Validators.required]
  });

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private vehicleService: VehicleService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.vehicleId = Number(id);
      this.vehicleService.get(this.vehicleId).subscribe(v => this.form.patchValue(v));
    }
  }

  get isEdit(): boolean {
    return this.vehicleId !== null;
  }

  invalid(name: string): boolean {
    const c = this.form.get(name);
    return !!c && c.touched && c.invalid;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = '';
    const value = this.form.value;

    const request: Observable<Vehicle | void> = this.vehicleId !== null
      ? this.vehicleService.update(this.vehicleId, value)
      : this.vehicleService.create(value);

    request.subscribe(
      (created: Vehicle | void) => {
        const id = this.vehicleId !== null ? this.vehicleId : (created as Vehicle).id;
        this.router.navigate(['/vehicles', id]);
      },
      () => {
        this.saving = false;
        this.error = 'Could not save the vehicle.';
      }
    );
  }
}
