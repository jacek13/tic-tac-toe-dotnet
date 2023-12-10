import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GameService } from '../game-service/game-sevice.service';  // Przyjmij, że masz taki serwis
import { NgFor, NgIf } from '@angular/common';
import { Subscription, interval } from 'rxjs';

@Component({
  standalone: true,
  imports: [NgIf, NgFor],
  selector: 'app-game-list',
  templateUrl: './game-list.component.html',
  styleUrls: ['./game-list.component.css']
})
export class GameListComponent implements OnInit {
  public games: any[] = []; // TODO make model for it
  private readonly refreshListInterval = interval(5000);
  private refreshSubsciprion : Subscription = this.refreshListInterval.subscribe(x => this.getGames());

  constructor(
    private gameService: GameService, 
    private router: Router) { }

  ngOnInit(): void {
    console.log("init games")
    this.getGames();
    console.log(this.games)
  }

  ngOnDestroy() {
    this.refreshSubsciprion.unsubscribe();
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

  findRandomGame(): void {
    this.gameService.findRandomGame()
      .subscribe(gameId => {
        this.router.navigate(['/game', gameId])
      });
  }

  createGameRoom(): void {
    this.gameService.createGame()
    .subscribe(gameId => {
      this.router.navigate(['/game', gameId])
    });
  }
}
