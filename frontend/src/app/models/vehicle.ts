export type VehicleStatus = 'Active' | 'InShop' | 'Retired';

export interface Vehicle {
  id: number;
  name: string;
  plate: string;
  make?: string;
  model?: string;
  year: number;
  odometerKm: number;
  status: VehicleStatus;
  createdAt?: string;
}
