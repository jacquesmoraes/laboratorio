import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { ScaleListComponent } from './scale-list.component';
import { ScaleService } from '../../../services/scale.service';

describe('ScaleListComponent', () => {
  let component: ScaleListComponent;
  let fixture: ComponentFixture<ScaleListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ScaleListComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [ScaleService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScaleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
