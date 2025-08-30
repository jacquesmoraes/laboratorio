import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { LoadingService } from '../services/loading.service';

describe('LoadingInterceptor', () => {
  let loadingService: jasmine.SpyObj<LoadingService>;

  beforeEach(() => {
    const loadingServiceSpy = jasmine.createSpyObj('LoadingService', ['show', 'hide']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        {
          provide: LoadingService,
          useValue: loadingServiceSpy
        }
      ]
    });

    loadingService = TestBed.inject(LoadingService) as jasmine.SpyObj<LoadingService>;
  });

  it('should be created', () => {
    expect(loadingService).toBeTruthy();
  });

  it('should have show and hide methods', () => {
    expect(loadingService.show).toBeDefined();
    expect(loadingService.hide).toBeDefined();
  });
});
