import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { DashboardCalendarComponent } from './dashboard-calendar.component';
import { ScheduleService } from '../../../core/services/schedule.service';

describe('DashboardCalendarComponent', () => {
  let component: DashboardCalendarComponent;
  let fixture: ComponentFixture<DashboardCalendarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        DashboardCalendarComponent,
        HttpClientTestingModule,
        NoopAnimationsModule
      ],
      providers: [ScheduleService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardCalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
