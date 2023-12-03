import { NgFor, NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [NgIf, NgFor],
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent implements OnInit {
  private hubConnection: signalR.HubConnection;
  public board: any[][];
  public gameId: string | null;
  public isMyMove: any;
  public clientFieldType: any;

  constructor(private route: ActivatedRoute) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hub/game', { withCredentials: false }) // TODO problem with CORS - origin null and move url to const data
      .build();

    this.board = [];
    this.gameId = '';

    this.hubConnection.on('SetMover', (isMyTurn: any) => {
      console.log(isMyTurn);
      if (isMyTurn) {
        this.isMyMove = isMyTurn;
      }
    });

    this.hubConnection.on('SetMover', (fieldType: any) => {
      console.log(fieldType);
      if (fieldType) {
        this.clientFieldType = fieldType;
      }
    });

    this.hubConnection.on('GetGame', (board: any) => {
      console.log(board);
      if (board) {
        this.board = JSON.parse(board);
      }
    });

    this.hubConnection.on('UpdateBoard', (board: any) => {
      console.log(board);
      if (board) {
        this.board = JSON.parse(board);
      }
    });

    this.hubConnection.on('GameEnded', (result: string) => {
      console.log('Game Ended:', result);
      // Handle game end logic here
    });

    this.hubConnection.on('Error', (error: string) => {
      console.error('Error:', error);
      // Handle error logic here
    });
  }

  ngOnInit(): void {
    // Odbieranie wartości przekazanej przez router z URL
    this.route.paramMap.subscribe(params => {
      this.gameId = params.get('id');
      console.log('Received value:', this.gameId);
    });

    this.hubConnection.start().then(() => {
      console.log('SignalR connection started');
    }).catch((err: any) => {
      console.error('Error while starting SignalR connection:', err);
    });
  }

  joinGame(): void {
    // Call the PlayerJoinGame method on the server
    this.hubConnection.invoke('PlayerJoinGame', this.gameId)
      .catch((err: any) => console.error('Error while joining game:', err));
  }

  makeMove(x: number, y: number, playerChar: string): void {
    // Call the PlayerMove method on the server
    this.hubConnection.invoke('PlayerMove', this.gameId, x, y, playerChar)
      .catch((err: any) => console.error('Error while making move:', err));
  }
}
