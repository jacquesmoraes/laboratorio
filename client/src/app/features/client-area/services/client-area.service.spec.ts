import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ClientAreaService } from './client-area.service';

describe('ClientAreaService', () => {
  let service: ClientAreaService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ClientAreaService]
    });
    service = TestBed.inject(ClientAreaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});