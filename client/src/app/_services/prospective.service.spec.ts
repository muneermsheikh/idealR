import { TestBed } from '@angular/core/testing';

import { ProspectiveService } from './prospective.service';

describe('ProspectiveService', () => {
  let service: ProspectiveService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProspectiveService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
