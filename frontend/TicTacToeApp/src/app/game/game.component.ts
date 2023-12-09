import { NgFor, NgIf } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import * as signalR from '@microsoft/signalr';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [NgIf, NgFor, FormsModule],
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent implements OnInit {
  private hubConnection: signalR.HubConnection;
  public board: any[][];
  public gameId: string | null;
  public isMyMove: any;
  public clientFieldType: any | null;
  public userText: string = '';
  public chatContent: string = '';

  constructor(private route: ActivatedRoute, private cdr: ChangeDetectorRef) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hub/game', { withCredentials: false }) // TODO problem with CORS - origin null and move url to const data
      .build();

    this.board = [];
    this.gameId = '';

    this.hubConnection.on('SetMover', (isMyTurn: any) => {
      console.log(isMyTurn);
      if (isMyTurn) {
        this.isMyMove = isMyTurn;
        this.cdr.detectChanges();
      }
    });

    this.hubConnection.on('SetChar', (fieldType: any) => {
      console.log(fieldType);
      if (fieldType) {
        this.clientFieldType = fieldType;
        this.chatContent += `[Client info] You play as: ${fieldType}\n`
        this.cdr.detectChanges();
      }
    });

    this.hubConnection.on('GetGame', (board: any) => {
      console.log(board);
      if (board) {
        this.board = JSON.parse(board);
        this.cdr.detectChanges();
      }
    });

    this.hubConnection.on('UpdateBoard', (board: any) => {
      console.log(board);
      if (board) {
        this.board = JSON.parse(board);
        this.cdr.detectChanges();
      }
    });

    this.hubConnection.on('GameEnded', (result: string) => {
      console.log('Game Ended:', result);
      this.cdr.detectChanges();
    });

    this.hubConnection.on('NewChatMessage', (chatMessage: any) => {
      console.log(chatMessage);
      if(chatMessage){
        this.chatContent += `${chatMessage.author} - ${chatMessage.content}\n`
      }
      this.cdr.detectChanges();
    });

    this.hubConnection.on('Error', (error: string) => {
      console.error('Error:', error);
      this.cdr.detectChanges();
    });
  }

  ngOnInit(): void {
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
    this.hubConnection.invoke('PlayerJoinGame', this.gameId)
      .catch((err: any) => console.error('Error while joining game:', err));
  }

  makeMove(x: number, y: number): void {
    this.hubConnection.invoke('PlayerMove', this.gameId, x, y, this.clientFieldType)
      .catch((err: any) => console.error('Error while making move:', err));
  }

  sendMessage(message: string): void {
    this.hubConnection.invoke('SendChatMessage', this.gameId, message)
      .catch((err: any) => console.error('Error while sending message:', err));
    this.userText = '';
  }
}
