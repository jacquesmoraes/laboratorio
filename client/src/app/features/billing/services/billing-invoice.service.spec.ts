import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { BillingInvoiceService } from './billing-invoice.service';

describe('BillingInvoiceService', () => {
  let service: BillingInvoiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BillingInvoiceService]
    });
    service = TestBed.inject(BillingInvoiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
