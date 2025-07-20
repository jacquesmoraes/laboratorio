import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleDeliveryModalComponent } from './schedule-delivery-modal.component';

describe('ScheduleDeliveryModalComponent', () => {
  let component: ScheduleDeliveryModalComponent;
  let fixture: ComponentFixture<ScheduleDeliveryModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScheduleDeliveryModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScheduleDeliveryModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
