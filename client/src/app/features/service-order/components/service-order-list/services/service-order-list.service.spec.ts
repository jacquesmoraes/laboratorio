import { TestBed } from '@angular/core/testing';

import { ServiceOrderListService } from './service-order-list.service';

describe('ServiceOrderListService', () => {
  let service: ServiceOrderListService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ServiceOrderListService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
