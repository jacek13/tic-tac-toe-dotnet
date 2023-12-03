import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GameService } from '../game-service/game-sevice.service';  // Przyjmij, że masz taki serwis
import { NgFor, NgIf } from '@angular/common';

@Component({
  standalone: true,
  imports: [NgIf, NgFor],
  selector: 'app-game-list',
  templateUrl: './game-list.component.html',
  styleUrls: ['./game-list.component.css']
})
export class GameListComponent implements OnInit {
  public games: any[] = []; // TODO make model for it

  constructor(private gameService: GameService, private router: Router) { }

  ngOnInit(): void {
    console.log("init games")
    this.getGames();
    console.log(this.games)
  }

  getGames(): void {
    this.gameService.getGames()
      .subscribe(games => {
        this.games = games;
      });
  }

  joinGame(gameId: string): void {
    this.router.navigate(['/game', gameId]);
  }
}
