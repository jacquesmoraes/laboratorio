import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TablePriceFormComponent } from './table-price-form.component';

describe('TablePriceFormComponent', () => {
  let component: TablePriceFormComponent;
  let fixture: ComponentFixture<TablePriceFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TablePriceFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TablePriceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
