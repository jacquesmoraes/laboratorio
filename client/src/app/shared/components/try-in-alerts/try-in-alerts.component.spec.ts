import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { TryInAlertsComponent } from './try-in-alerts.component';
import { ServiceOrdersService } from '../../../features/service-order/services/service-order.service';

describe('TryInAlertsComponent', () => {
  let component: TryInAlertsComponent;
  let fixture: ComponentFixture<TryInAlertsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        TryInAlertsComponent,
        HttpClientTestingModule,
        NoopAnimationsModule
      ],
      providers: [ServiceOrdersService]
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
