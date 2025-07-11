import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClientAreaDashboardComponent } from './client-area-dashboard.component';



describe('DashboardComponent', () => {
  let component: ClientAreaDashboardComponent;
  let fixture: ComponentFixture<ClientAreaDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClientAreaDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClientAreaDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
