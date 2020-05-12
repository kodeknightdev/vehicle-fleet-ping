import { Vehicle } from './vehicle';

export interface DashboardSummary {
  totalVehicles: number;
  active: number;
  inShop: number;
  retired: number;
  reporting: number;
  reportingWindowMinutes: number;
  lastPingAt: string | null;
  maintenanceCost30d: number;
  maintenanceCount30d: number;
  serviceDue: Vehicle[];
}
