import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { ServiceOrderListService } from './service-order-list.service';

describe('ServiceOrderListService', () => {
  let service: ServiceOrderListService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(ServiceOrderListService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});