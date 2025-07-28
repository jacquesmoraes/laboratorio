import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { ScheduleDeliveryModalComponent } from './schedule-delivery-modal.component';

describe('ScheduleDeliveryModalComponent', () => {
  let component: ScheduleDeliveryModalComponent;
  let fixture: ComponentFixture<ScheduleDeliveryModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ScheduleDeliveryModalComponent,
        NoopAnimationsModule
      ],
      providers: [
        provideHttpClient(withFetch()),
        provideRouter([]),
        { provide: MatDialogRef, useValue: {} },
        { provide: MAT_DIALOG_DATA, useValue: {} }
      ]
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