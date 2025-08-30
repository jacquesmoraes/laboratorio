import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ScaleService } from './scale.service';

describe('ScaleService', () => {
  let service: ScaleService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ScaleService]
    });
    service = TestBed.inject(ScaleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
