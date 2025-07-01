import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkTypeListComponent } from './work-type-list-component';

describe('WorkTypeListComponent', () => {
  let component: WorkTypeListComponent;
  let fixture: ComponentFixture<WorkTypeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkTypeListComponent]
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
