import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TablePriceDetailsComponent } from './table-price-details.component';

describe('TablePriceDetailsComponent', () => {
  let component: TablePriceDetailsComponent;
  let fixture: ComponentFixture<TablePriceDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TablePriceDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TablePriceDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
