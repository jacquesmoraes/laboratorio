import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SectorModalComponent } from './sector-modal.component';
import { SectorService } from '../service/sector.service';

describe('SectorModalComponent', () => {
  let component: SectorModalComponent;
  let fixture: ComponentFixture<SectorModalComponent>;
  let mockDialogRef: jasmine.SpyObj<MatDialogRef<SectorModalComponent>>;

  beforeEach(async () => {
    const dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [
        SectorModalComponent,
        HttpClientTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [
        SectorService,
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: {} }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SectorModalComponent);
    component = fixture.componentInstance;
    mockDialogRef = TestBed.inject(MatDialogRef) as jasmine.SpyObj<MatDialogRef<SectorModalComponent>>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
