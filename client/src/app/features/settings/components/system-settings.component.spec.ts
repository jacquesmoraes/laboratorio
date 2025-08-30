import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { SystemSettingsComponent } from './system-settings.component';
import { SettingsService } from '../services/system-settings.service';

describe('SystemSettingsComponent', () => {
  let component: SystemSettingsComponent;
  let fixture: ComponentFixture<SystemSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        SystemSettingsComponent,
        HttpClientTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [SettingsService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SystemSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
