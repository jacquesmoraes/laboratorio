import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SectorListComponent } from './sector-list.component';

describe('SectorList', () => {
  let component: SectorListComponent;
  let fixture: ComponentFixture<SectorListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SectorListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SectorListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
