import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardCalendarComponent } from './dashboard-calendar.component';

describe('DashboardCalendarComponent', () => {
  let component: DashboardCalendarComponent;
  let fixture: ComponentFixture<DashboardCalendarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardCalendarComponent]
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
