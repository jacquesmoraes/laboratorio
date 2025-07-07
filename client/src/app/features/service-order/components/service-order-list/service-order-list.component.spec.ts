import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceOrderListComponent } from './service-order-list.component';

describe('ServiceOrderListComponent', () => {
  let component: ServiceOrderListComponent;
  let fixture: ComponentFixture<ServiceOrderListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ServiceOrderListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServiceOrderListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
