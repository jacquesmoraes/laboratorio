import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BillingInvoiceListComponent } from './billing-invoice-list.component';

describe('BillingInvoiceListComponent', () => {
  let component: BillingInvoiceListComponent;
  let fixture: ComponentFixture<BillingInvoiceListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BillingInvoiceListComponent]
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
