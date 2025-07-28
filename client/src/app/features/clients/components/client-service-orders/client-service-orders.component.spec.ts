import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClientServiceOrdersComponent } from './client-service-orders.component';

describe('ClientServiceOrdersComponent', () => {
  let component: ClientServiceOrdersComponent;
  let fixture: ComponentFixture<ClientServiceOrdersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientServiceOrdersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientServiceOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
