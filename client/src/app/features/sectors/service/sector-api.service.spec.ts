import { TestBed } from '@angular/core/testing';

import { SectorApi } from './sector-api.service';

describe('SectorApi', () => {
  let service: SectorApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SectorApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
