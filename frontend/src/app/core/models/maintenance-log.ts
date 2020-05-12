export type MaintenanceType = 'OilChange' | 'Tyres' | 'Brakes' | 'Inspection' | 'Repair' | 'Other';

export const MAINTENANCE_TYPES: MaintenanceType[] = ['OilChange', 'Tyres', 'Brakes', 'Inspection', 'Repair', 'Other'];

export interface MaintenanceLog {
  id: number;
  vehicleId: number;
  date: string;
  type: MaintenanceType;
  odometerKm: number;
  cost: number;
  mechanic?: string;
  notes?: string;
}
