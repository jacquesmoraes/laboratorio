import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SectorFormComponent } from './sector-modal.component';

describe('SectorForm', () => {
  let component: SectorFormComponent;
  let fixture: ComponentFixture<SectorFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SectorFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SectorFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
