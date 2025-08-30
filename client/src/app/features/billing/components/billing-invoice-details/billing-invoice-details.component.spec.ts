import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { BillingInvoiceDetailsComponent } from './billing-invoice-details.component';
import { BillingInvoiceService } from '../../services/billing-invoice.service';

describe('BillingInvoiceDetailsComponent', () => {
  let component: BillingInvoiceDetailsComponent;
  let fixture: ComponentFixture<BillingInvoiceDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        BillingInvoiceDetailsComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [BillingInvoiceService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BillingInvoiceDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
