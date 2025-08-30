import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { BillingInvoiceCreateComponent } from './billing-invoice-create.component';
import { BillingInvoiceService } from '../../services/billing-invoice.service';
import { ServiceOrdersService } from '../../../service-order/services/service-order.service';

describe('BillingInvoiceCreateComponent', () => {
  let component: BillingInvoiceCreateComponent;
  let fixture: ComponentFixture<BillingInvoiceCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        BillingInvoiceCreateComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [
        BillingInvoiceService,
        ServiceOrdersService
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BillingInvoiceCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
