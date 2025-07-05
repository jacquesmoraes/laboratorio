import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScaleModalComponent } from './scale-modal.component';

describe('ScaleModalComponent', () => {
  let component: ScaleModalComponent;
  let fixture: ComponentFixture<ScaleModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScaleModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ScaleModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
