import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ShadeModalComponent } from './shade-modal.component';
import { ShadeService } from '../../../services/shade.service';

describe('ShadeModalComponent', () => {
  let component: ShadeModalComponent;
  let fixture: ComponentFixture<ShadeModalComponent>;
  let mockDialogRef: jasmine.SpyObj<MatDialogRef<ShadeModalComponent>>;

  beforeEach(async () => {
    const dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);

    await TestBed.configureTestingModule({
      imports: [
        ShadeModalComponent,
        HttpClientTestingModule,
        ReactiveFormsModule,
        NoopAnimationsModule
      ],
      providers: [
        ShadeService,
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: {} }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShadeModalComponent);
    component = fixture.componentInstance;
    mockDialogRef = TestBed.inject(MatDialogRef) as jasmine.SpyObj<MatDialogRef<ShadeModalComponent>>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
