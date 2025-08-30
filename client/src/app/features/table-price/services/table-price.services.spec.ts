import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TablePriceService } from './table-price.services';

describe('TablePriceService', () => {
  let service: TablePriceService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TablePriceService]
    });
    service = TestBed.inject(TablePriceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});