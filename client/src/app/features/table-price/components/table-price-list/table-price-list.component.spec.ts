import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TablePriceListComponent } from './table-price-list.component';

describe('TablePriceListComponent', () => {
  let component: TablePriceListComponent;
  let fixture: ComponentFixture<TablePriceListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TablePriceListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TablePriceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
