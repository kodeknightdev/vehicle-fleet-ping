export type VehicleStatus = 'Active' | 'InShop' | 'Retired';

export const VEHICLE_STATUSES: VehicleStatus[] = ['Active', 'InShop', 'Retired'];

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
  serviceIntervalKm: number;
  lastServiceKm?: number | null;
  lastServiceDate?: string | null;
  nextServiceKm?: number;
  kmUntilService?: number;
  serviceDue?: boolean;
}
