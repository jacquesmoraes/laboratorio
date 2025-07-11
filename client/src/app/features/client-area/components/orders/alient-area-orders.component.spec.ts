import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClientAreaOrdersComponent } from './client-area-orders.component';



describe('OrdersComponent', () => {
  let component: ClientAreaOrdersComponent;
  let fixture: ComponentFixture<ClientAreaOrdersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientAreaOrdersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientAreaOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
