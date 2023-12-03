import { TestBed } from '@angular/core/testing';

import { GameSeviceService } from './game-sevice.service';

describe('GameSeviceService', () => {
  let service: GameSeviceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GameSeviceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
