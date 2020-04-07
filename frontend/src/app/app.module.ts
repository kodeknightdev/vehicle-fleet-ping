import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MaintenanceFormComponent } from './maintenance/maintenance-form/maintenance-form.component';
import { MaintenanceListComponent } from './maintenance/maintenance-list/maintenance-list.component';
import { FleetMapComponent } from './map/fleet-map/fleet-map.component';
import { VehicleDetailComponent } from './vehicles/vehicle-detail/vehicle-detail.component';
import { VehicleListComponent } from './vehicles/vehicle-list/vehicle-list.component';

@NgModule({
  declarations: [
    AppComponent,
    VehicleListComponent,
    VehicleDetailComponent,
    FleetMapComponent,
    MaintenanceListComponent,
    MaintenanceFormComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    ReactiveFormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
