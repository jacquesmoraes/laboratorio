import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { WorkTypeListComponent } from './work-type-list-component';
import { WorkTypeService } from '../../services/work-type.service';

describe('WorkTypeListComponent', () => {
  let component: WorkTypeListComponent;
  let fixture: ComponentFixture<WorkTypeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        WorkTypeListComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [WorkTypeService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
