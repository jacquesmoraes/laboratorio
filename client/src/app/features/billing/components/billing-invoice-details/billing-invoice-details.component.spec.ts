import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BillingInvoiceDetailsComponent } from './billing-invoice-details.component';

describe('BillingInvoiceDetailsComponent', () => {
  let component: BillingInvoiceDetailsComponent;
  let fixture: ComponentFixture<BillingInvoiceDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BillingInvoiceDetailsComponent]
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
