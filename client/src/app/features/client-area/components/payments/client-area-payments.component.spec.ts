import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientAreaPaymentsComponent } from './client-area-payments.component';

describe('PaymentsComponent', () => {
  let component: ClientAreaPaymentsComponent;
  let fixture: ComponentFixture<ClientAreaPaymentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientAreaPaymentsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientAreaPaymentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
