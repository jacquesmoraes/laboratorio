import { TestBed } from '@angular/core/testing';

import { BillingInvoiceService } from './billing-invoice.service';

describe('BillingInvoiceService', () => {
  let service: BillingInvoiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BillingInvoiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
