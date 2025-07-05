import { ComponentFixture, TestBed } from '@angular/core/testing';

import {  SectorModalComponent } from './sector-modal.component';

describe('SectorForm', () => {
  let component: SectorModalComponent;
  let fixture: ComponentFixture<SectorModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SectorModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SectorModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
