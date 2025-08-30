import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { WorkSectionService } from './works-section.service';

describe('WorkSectionService', () => {
  let service: WorkSectionService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [WorkSectionService]
    });
    service = TestBed.inject(WorkSectionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});