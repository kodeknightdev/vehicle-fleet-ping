import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild } from '@angular/core';
import * as L from 'leaflet';

import { VehiclePosition } from '../../models/vehicle-position';
import { PositionService } from '../../services/position.service';

const DEPOT: L.LatLngTuple = [45.548, -122.661];

@Component({
  selector: 'app-fleet-map',
  templateUrl: './fleet-map.component.html',
  styleUrls: ['./fleet-map.component.scss']
})
export class FleetMapComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mapEl') mapEl: ElementRef<HTMLDivElement>;

  positions: VehiclePosition[] = [];
  private map: L.Map;
  private markers = L.layerGroup();

  constructor(private positionService: PositionService) { }

  ngAfterViewInit(): void {
    this.map = L.map(this.mapEl.nativeElement).setView(DEPOT, 12);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(this.map);
    this.markers.addTo(this.map);

    this.positionService.lastKnown().subscribe(p => {
      this.positions = p;
      this.render();
    });
  }

  ngOnDestroy(): void {
    if (this.map) {
      this.map.remove();
    }
  }

  private render(): void {
    this.markers.clearLayers();
    for (const p of this.positions) {
      L.marker([p.latitude, p.longitude])
        .bindPopup(`<strong>${p.vehicleName}</strong><br>${p.speedKph} km/h`)
        .addTo(this.markers);
    }
  }
}
