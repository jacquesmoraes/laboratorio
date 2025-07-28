import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { ServiceOrderDetailsComponent } from './service-order-details.component';

describe('ServiceOrderDetailsComponent', () => {
  let component: ServiceOrderDetailsComponent;
  let fixture: ComponentFixture<ServiceOrderDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ServiceOrderDetailsComponent,
        NoopAnimationsModule
      ],
      providers: [
        provideHttpClient(withFetch()),
        provideRouter([])
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServiceOrderDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});