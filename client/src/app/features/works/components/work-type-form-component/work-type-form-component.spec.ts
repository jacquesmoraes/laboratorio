import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkTypeFormComponent } from './work-type-form-component';

describe('WorkTypeFormComponent', () => {
  let component: WorkTypeFormComponent;
  let fixture: ComponentFixture<WorkTypeFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkTypeFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkTypeFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
