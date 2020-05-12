import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild } from '@angular/core';
import { interval } from 'rxjs';
import { startWith, switchMap } from 'rxjs/operators';
import * as L from 'leaflet';

import { VehiclePosition } from '../../core/models';
import { PositionService } from '../../core/services';

const DEPOT: L.LatLngTuple = [45.548, -122.661];
const REFRESH_MS = 10000;
const STALE_MINUTES = 15;

// Leaflet guesses icon urls from its css, which breaks once the cli bundles it.
// Point it at the images copied in angular.json instead.
delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconUrl: 'assets/leaflet/marker-icon.png',
  iconRetinaUrl: 'assets/leaflet/marker-icon-2x.png',
  shadowUrl: 'assets/leaflet/marker-shadow.png'
});

type MarkerState = 'moving' | 'stopped' | 'stale';

@Component({
  selector: 'app-fleet-map',
  templateUrl: './fleet-map.component.html',
  styleUrls: ['./fleet-map.component.scss']
})
export class FleetMapComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mapEl') mapEl: ElementRef<HTMLDivElement>;

  positions: VehiclePosition[] = [];
  lastUpdated: Date | null = null;
  selectedId: number | null = null;

  private map: L.Map;
  private markers = new Map<number, L.CircleMarker>();
  private fitted = false;

  constructor(private positionService: PositionService) { }

  ngAfterViewInit(): void {
    this.map = L.map(this.mapEl.nativeElement).setView(DEPOT, 12);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 19,
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(this.map);

    L.marker(DEPOT, { title: 'Depot' }).bindTooltip('Depot').addTo(this.map);

    interval(REFRESH_MS).pipe(
      startWith(0),
      switchMap(() => this.positionService.lastKnown())
    ).subscribe(p => {
      this.positions = p;
      this.lastUpdated = new Date();
      this.render();
    });
  }

  ngOnDestroy(): void {
    if (this.map) {
      this.map.remove();
    }
  }

  state(p: VehiclePosition): MarkerState {
    const ageMin = (Date.now() - new Date(p.recordedAt).getTime()) / 60000;
    if (p.source !== 'gps' || ageMin > STALE_MINUTES) {
      return 'stale';
    }
    return p.speedKph > 0 ? 'moving' : 'stopped';
  }

  focus(p: VehiclePosition): void {
    this.selectedId = p.vehicleId;
    this.map.flyTo([p.latitude, p.longitude], 15, { duration: 0.6 });
    const marker = this.markers.get(p.vehicleId);
    if (marker) {
      marker.openPopup();
    }
  }

  private render(): void {
    if (!this.map) {
      return;
    }

    const seen = new Set<number>();
    for (const p of this.positions) {
      seen.add(p.vehicleId);
      const latLng: L.LatLngTuple = [p.latitude, p.longitude];
      const style = this.styleFor(this.state(p));
      const popup = `<strong>${p.vehicleName}</strong><br>`
        + `${p.speedKph} km/h, heading ${p.headingDeg}&deg;<br>`
        + `<small>${new Date(p.recordedAt).toLocaleTimeString()} (${p.source})</small><br>`
        + `<a href="/vehicles/${p.vehicleId}">Details</a>`;

      const existing = this.markers.get(p.vehicleId);
      if (existing) {
        existing.setLatLng(latLng).setStyle(style).setPopupContent(popup);
      } else {
        const marker = L.circleMarker(latLng, { radius: 9, weight: 2, ...style })
          .bindPopup(popup)
          .bindTooltip(p.vehicleName, { direction: 'top', offset: [0, -8] })
          .addTo(this.map);
        this.markers.set(p.vehicleId, marker);
      }
    }

    // drop markers for vehicles that were removed or retired
    this.markers.forEach((marker, id) => {
      if (!seen.has(id)) {
        marker.remove();
        this.markers.delete(id);
      }
    });

    if (!this.fitted && this.positions.length) {
      const bounds = L.latLngBounds(this.positions.map(p => [p.latitude, p.longitude] as L.LatLngTuple));
      this.map.fitBounds(bounds.pad(0.2), { maxZoom: 14 });
      this.fitted = true;
    }
  }

  private styleFor(state: MarkerState): L.PathOptions {
    switch (state) {
      case 'moving': return { color: '#1e7e34', fillColor: '#34c759', fillOpacity: 0.9 };
      case 'stopped': return { color: '#b8860b', fillColor: '#ffcc00', fillOpacity: 0.9 };
      default: return { color: '#666', fillColor: '#bbb', fillOpacity: 0.8 };
    }
  }
}
