import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkSectionListComponent } from './work-section-list.component';

describe('WorkSectionListComponent', () => {
  let component: WorkSectionListComponent;
  let fixture: ComponentFixture<WorkSectionListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkSectionListComponent]
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
