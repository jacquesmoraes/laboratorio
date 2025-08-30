import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { UserManagementService } from './user-management.service';

describe('UserManagementService', () => {
  let service: UserManagementService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserManagementService]
    });
    service = TestBed.inject(UserManagementService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
