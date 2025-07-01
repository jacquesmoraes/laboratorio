import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkSectionFormComponent } from './work-section-form-component';

describe('WorkSectionFormComponent', () => {
  let component: WorkSectionFormComponent;
  let fixture: ComponentFixture<WorkSectionFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkSectionFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkSectionFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
