import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { WorkSectionListComponent } from './work-section-list.component';
import { WorkSectionService } from '../../services/works-section.service';

describe('WorkSectionListComponent', () => {
  let component: WorkSectionListComponent;
  let fixture: ComponentFixture<WorkSectionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        WorkSectionListComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [WorkSectionService]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkSectionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
