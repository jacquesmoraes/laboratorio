import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TryInAlertsComponent } from './try-in-alerts.component';

describe('TryInAlertsComponent', () => {
  let component: TryInAlertsComponent;
  let fixture: ComponentFixture<TryInAlertsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TryInAlertsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TryInAlertsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
