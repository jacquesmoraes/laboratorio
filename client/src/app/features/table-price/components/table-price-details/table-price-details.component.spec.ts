import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { TablePriceDetailsComponent } from './table-price-details.component';
import { TablePriceService } from '../../services/table-price.services';

describe('TablePriceDetailsComponent', () => {
  let component: TablePriceDetailsComponent;
  let fixture: ComponentFixture<TablePriceDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        TablePriceDetailsComponent,
        HttpClientTestingModule,
        RouterTestingModule,
        NoopAnimationsModule
      ],
      providers: [TablePriceService]
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
