import { Component, OnInit } from '@angular/core';

import { Vehicle } from '../../core/models';
import { VehicleService } from '../../core/services';

@Component({
  selector: 'app-vehicle-list',
  templateUrl: './vehicle-list.component.html',
  styleUrls: ['./vehicle-list.component.scss']
})
export class VehicleListComponent implements OnInit {
  vehicles: Vehicle[] = [];
  loading = false;
  error = '';

  constructor(private vehicleService: VehicleService) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';
    this.vehicleService.list().subscribe(
      vehicles => {
        this.vehicles = vehicles;
        this.loading = false;
      },
      () => {
        this.error = 'Could not load vehicles.';
        this.loading = false;
      }
    );
  }
}
