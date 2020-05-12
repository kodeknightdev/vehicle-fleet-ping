export interface VehiclePosition {
  vehicleId: number;
  vehicleName: string;
  latitude: number;
  longitude: number;
  speedKph: number;
  headingDeg: number;
  recordedAt: string;
  source: string;
}
