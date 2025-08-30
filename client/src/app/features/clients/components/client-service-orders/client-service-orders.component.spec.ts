import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { Component } from '@angular/core';
import { ClientServiceOrdersComponent } from './client-service-orders.component';
import { ServiceOrdersService } from '../../../service-order/services/service-order.service';

// Componente wrapper para testar o componente com input obrigatório
@Component({
  template: '<app-client-service-orders [clientId]="1"></app-client-service-orders>',
  standalone: true,
  imports: [ClientServiceOrdersComponent]
})
class TestWrapperComponent {}

describe('ClientServiceOrdersComponent', () => {
  let component: ClientServiceOrdersComponent;
  let fixture: ComponentFixture<TestWrapperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        TestWrapperComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [ServiceOrdersService]
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
