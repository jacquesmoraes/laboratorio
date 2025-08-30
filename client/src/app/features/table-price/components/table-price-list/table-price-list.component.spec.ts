import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { TablePriceListComponent } from './table-price-list.component';
import { TablePriceService } from '../../services/table-price.services';

describe('TablePriceListComponent', () => {
  let component: TablePriceListComponent;
  let fixture: ComponentFixture<TablePriceListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        TablePriceListComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [TablePriceService]
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
