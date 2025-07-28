import { TestBed } from '@angular/core/testing';

import { WorkSectionService } from './works-section.service';

describe('WorkSectionService', () => {
  let service: WorkSectionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WorkSectionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});