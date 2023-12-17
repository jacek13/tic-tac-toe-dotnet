import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GameService } from '../game-service/game-sevice.service';
import { NgFor, NgIf } from '@angular/common';
import { UserService } from '../user/user-service.component';

@Component({
  standalone: true,
  imports: [NgIf, NgFor],
  selector: 'app-game-score-board',
  templateUrl: './game-score-board.component.html'
})
export class ScoreBoardComponent implements OnInit {
  public scoreBoard: any[] = []; // TODO make model for it

  constructor(
    private gameService: GameService,
    private userService: UserService,
    private router: Router) { }

  ngOnInit(): void {
    console.log("init score board")
    this.getScoreBoard();
    console.log(this.scoreBoard)
  }

  getScoreBoard(): void {
    this.userService.getGlobalScoreBoard()
      .subscribe(response => {
        this.scoreBoard = response;
      });
  }
}
