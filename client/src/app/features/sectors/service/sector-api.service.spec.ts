import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { SectorService } from './sector.service';


describe('SectorService', () => {
  let service: SectorService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [SectorService]
    });
    service = TestBed.inject(SectorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
