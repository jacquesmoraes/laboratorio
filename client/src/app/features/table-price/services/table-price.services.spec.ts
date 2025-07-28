import { TestBed } from '@angular/core/testing';

import { TablePriceService } from './table-price.services';

describe('TablePriceService', () => {
  let service: TablePriceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TablePriceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});