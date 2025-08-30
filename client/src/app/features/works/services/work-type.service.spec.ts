import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { WorkTypeService } from './work-type.service';

describe('WorkTypeService', () => {
  let service: WorkTypeService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [WorkTypeService]
    });
    service = TestBed.inject(WorkTypeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
