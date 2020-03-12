import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Vehicle } from '../../models/vehicle';
import { VehicleService } from '../../services/vehicle.service';

@Component({
  selector: 'app-vehicle-detail',
  templateUrl: './vehicle-detail.component.html',
  styleUrls: ['./vehicle-detail.component.scss']
})
export class VehicleDetailComponent implements OnInit {
  vehicle: Vehicle | null = null;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private vehicleService: VehicleService
  ) { }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.vehicleService.get(id).subscribe(
      v => this.vehicle = v,
      () => this.error = 'Vehicle not found.'
    );
  }
}
