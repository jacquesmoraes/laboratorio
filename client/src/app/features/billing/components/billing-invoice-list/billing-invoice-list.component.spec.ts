import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { BillingInvoiceListComponent } from './billing-invoice-list.component';
import { BillingInvoiceService } from '../../services/billing-invoice.service';

describe('BillingInvoiceListComponent', () => {
  let component: BillingInvoiceListComponent;
  let fixture: ComponentFixture<BillingInvoiceListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        BillingInvoiceListComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [BillingInvoiceService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BillingInvoiceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
