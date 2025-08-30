import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { FirstAccessComponent } from './first-access-component';
import { AuthService } from '../../services/auth.service';

describe('FirstAccessComponent', () => {
  let component: FirstAccessComponent;
  let fixture: ComponentFixture<FirstAccessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        FirstAccessComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [AuthService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FirstAccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
