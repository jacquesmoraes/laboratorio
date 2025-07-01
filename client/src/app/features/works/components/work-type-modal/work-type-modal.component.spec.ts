import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkTypeModalComponent } from './work-type-modal.component';

describe('WorkTypeFormComponent', () => {
  let component: WorkTypeModalComponent;
  let fixture: ComponentFixture<WorkTypeModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkTypeModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WorkTypeModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
