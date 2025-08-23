import { Component, ChangeDetectionStrategy } from '@angular/core';
import { TryInAlertsComponent } from '../try-in-alerts/try-in-alerts.component';
import { DashboardCalendarComponent } from '../dashboard-calendar/dashboard-calendar.component';

@Component({
  selector: 'app-app-admin-dashboard',
  standalone: true,
  imports: [TryInAlertsComponent,DashboardCalendarComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent {
  // Component logic can be added here
}