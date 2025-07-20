import { TestBed } from '@angular/core/testing';

import { ClientAreaServices } from './client-area.services';

describe('ClientAreaServices', () => {
  let service: ClientAreaServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClientAreaServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
