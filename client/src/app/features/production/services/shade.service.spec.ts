import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ShadeService } from './shade.service';

describe('ShadeService', () => {
  let service: ShadeService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ShadeService]
    });
    service = TestBed.inject(ShadeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
