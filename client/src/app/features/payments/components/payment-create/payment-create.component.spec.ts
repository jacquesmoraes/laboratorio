import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { PaymentCreateComponent } from './payment-create.component';
import { PaymentService } from '../../services/payment.service';

describe('PaymentCreateComponent', () => {
  let component: PaymentCreateComponent;
  let fixture: ComponentFixture<PaymentCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        PaymentCreateComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [PaymentService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
