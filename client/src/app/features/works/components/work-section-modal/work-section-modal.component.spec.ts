import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkSectionModalComponent } from './work-section-modal.component';

describe('WorkSectionFormComponent', () => {
  let component: WorkSectionModalComponent;
  let fixture: ComponentFixture<WorkSectionModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkSectionModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkSectionModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
