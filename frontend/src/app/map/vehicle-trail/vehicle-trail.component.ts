import { AfterViewInit, Component, ElementRef, Input, OnDestroy, ViewChild } from '@angular/core';
import * as L from 'leaflet';

import { GpsPing } from '../../core/models';
import { PositionService } from '../../core/services';

@Component({
  selector: 'app-vehicle-trail',
  templateUrl: './vehicle-trail.component.html',
  styleUrls: ['./vehicle-trail.component.scss']
})
export class VehicleTrailComponent implements AfterViewInit, OnDestroy {
  @Input() vehicleId: number;
  @ViewChild('mapEl') mapEl: ElementRef<HTMLDivElement>;

  readonly ranges = [1, 6, 24];
  hours = 6;
  pings: GpsPing[] = [];
  loading = false;

  private map: L.Map;
  private layer = L.featureGroup();

  constructor(private positionService: PositionService) { }

  ngAfterViewInit(): void {
    this.map = L.map(this.mapEl.nativeElement, { zoomControl: true }).setView([45.548, -122.661], 12);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(this.map);
    this.layer.addTo(this.map);
    this.load();
  }

  ngOnDestroy(): void {
    if (this.map) {
      this.map.remove();
    }
  }

  get distanceKm(): number {
    let total = 0;
    for (let i = 1; i < this.pings.length; i++) {
      const a = L.latLng(this.pings[i - 1].latitude, this.pings[i - 1].longitude);
      const b = L.latLng(this.pings[i].latitude, this.pings[i].longitude);
      total += a.distanceTo(b);
    }
    return total / 1000;
  }

  setRange(hours: number): void {
    this.hours = hours;
    this.load();
  }

  load(): void {
    const from = new Date(Date.now() - this.hours * 3600 * 1000);
    this.loading = true;
    this.positionService.history(this.vehicleId, from).subscribe(pings => {
      this.pings = pings;
      this.loading = false;
      this.draw();
    });
  }

  private draw(): void {
    this.layer.clearLayers();
    if (!this.map || !this.pings.length) {
      return;
    }

    const points = this.pings.map(p => [p.latitude, p.longitude] as L.LatLngTuple);
    L.polyline(points, { color: '#1c6ed4', weight: 4, opacity: 0.8 }).addTo(this.layer);

    const first = this.pings[0];
    const last = this.pings[this.pings.length - 1];
    L.circleMarker([first.latitude, first.longitude], { radius: 5, color: '#666', fillColor: '#fff', fillOpacity: 1 })
      .bindTooltip(`Start ${new Date(first.recordedAt).toLocaleTimeString()}`)
      .addTo(this.layer);
    L.circleMarker([last.latitude, last.longitude], { radius: 8, color: '#1e7e34', fillColor: '#34c759', fillOpacity: 1 })
      .bindTooltip(`Last seen ${new Date(last.recordedAt).toLocaleTimeString()}`)
      .addTo(this.layer);

    this.map.fitBounds(this.layer.getBounds().pad(0.15), { maxZoom: 16 });
  }
}
