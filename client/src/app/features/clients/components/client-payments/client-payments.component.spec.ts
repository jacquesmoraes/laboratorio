import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { Component } from '@angular/core';
import { ClientPaymentsComponent } from './client-payments.component';
import { PaymentService } from '../../../payments/services/payment.service';

// Componente wrapper para testar o componente com input obrigatório
@Component({
  template: '<app-client-payments [clientId]="1"></app-client-payments>',
  standalone: true,
  imports: [ClientPaymentsComponent]
})
class TestWrapperComponent {}

describe('ClientPaymentsComponent', () => {
  let component: ClientPaymentsComponent;
  let fixture: ComponentFixture<TestWrapperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        TestWrapperComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [PaymentService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestWrapperComponent);
    component = fixture.debugElement.children[0].componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
