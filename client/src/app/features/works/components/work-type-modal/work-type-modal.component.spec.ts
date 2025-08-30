import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { WorkTypeModalComponent } from './work-type-modal.component';
import { WorkTypeService } from '../../services/work-type.service';

describe('WorkTypeModalComponent', () => {
  let component: WorkTypeModalComponent;
  let fixture: ComponentFixture<WorkTypeModalComponent>;
  let mockDialogRef: jasmine.SpyObj<MatDialogRef<WorkTypeModalComponent>>;

  beforeEach(async () => {
    const dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [
        WorkTypeModalComponent,
        HttpClientTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [
        WorkTypeService,
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: {} }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkTypeModalComponent);
    component = fixture.componentInstance;
    mockDialogRef = TestBed.inject(MatDialogRef) as jasmine.SpyObj<MatDialogRef<WorkTypeModalComponent>>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
