import { TestBed } from '@angular/core/testing';

import { TablePriceServices } from './table-price.services';

describe('TablePriceServices', () => {
  let service: TablePriceServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TablePriceServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
