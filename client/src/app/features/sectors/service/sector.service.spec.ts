import { TestBed } from '@angular/core/testing';

import { Sector } from './sector.service';

describe('Sector', () => {
  let service: Sector;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Sector);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
