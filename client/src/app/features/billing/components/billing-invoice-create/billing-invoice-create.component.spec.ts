import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BillingInvoiceCreateComponent } from './billing-invoice-create.component';

describe('BillingInvoiceCreateComponent', () => {
  let component: BillingInvoiceCreateComponent;
  let fixture: ComponentFixture<BillingInvoiceCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BillingInvoiceCreateComponent]
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
