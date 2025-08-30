import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { PaymentListComponent } from './payment-list.component';
import { PaymentService } from '../../services/payment.service';

describe('PaymentListComponent', () => {
  let component: PaymentListComponent;
  let fixture: ComponentFixture<PaymentListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        PaymentListComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [PaymentService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
