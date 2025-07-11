import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClientAreaInvoicesComponent } from './client-area-invoices.component';



describe('InvoicesComponent', () => {
  let component: ClientAreaInvoicesComponent;
  let fixture: ComponentFixture<ClientAreaInvoicesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientAreaInvoicesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientAreaInvoicesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
