import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { ServiceOrderFormComponent } from './service-order-form.component';

describe('ServiceOrderFormComponent', () => {
  let component: ServiceOrderFormComponent;
  let fixture: ComponentFixture<ServiceOrderFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ServiceOrderFormComponent,
        NoopAnimationsModule
      ],
      providers: [
        provideHttpClient(withFetch()),
        provideRouter([])
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ServiceOrderFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});