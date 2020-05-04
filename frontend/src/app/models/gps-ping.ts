export interface GpsPing {
  id: number;
  vehicleId: number;
  latitude: number;
  longitude: number;
  speedKph: number;
  headingDeg: number;
  recordedAt: string;
  receivedAt: string;
}
