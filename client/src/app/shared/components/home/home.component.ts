import { Component, ChangeDetectionStrategy } from '@angular/core';
import { TryInAlertsComponent } from '../try-in-alerts/try-in-alerts.component';
import { DashboardCalendarComponent } from '../dashboard-calendar/dashboard-calendar.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [TryInAlertsComponent,DashboardCalendarComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  // Component logic can be added here
}