import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ScaleModalComponent } from './scale-modal.component';
import { ScaleService } from '../../../services/scale.service';

describe('ScaleModalComponent', () => {
  let component: ScaleModalComponent;
  let fixture: ComponentFixture<ScaleModalComponent>;
  let mockDialogRef: jasmine.SpyObj<MatDialogRef<ScaleModalComponent>>;

  beforeEach(async () => {
    const dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [
        ScaleModalComponent,
        HttpClientTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [
        ScaleService,
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: {} }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScaleModalComponent);
    component = fixture.componentInstance;
    mockDialogRef = TestBed.inject(MatDialogRef) as jasmine.SpyObj<MatDialogRef<ScaleModalComponent>>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
