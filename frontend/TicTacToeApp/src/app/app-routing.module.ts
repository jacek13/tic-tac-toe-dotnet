import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GameListComponent } from './game-list/game-list.component';
import { GameComponent } from './game/game.component';
import { SignInComponent } from './auth/signin/signin.component';
import { SignUpComponent } from './auth/signup/signup.component';
import { SignUpConfirmComponent } from './auth/signup/signup-confirm.component';
import { ScoreBoardComponent } from './game-global-score-board/game-score-board.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'games',
    pathMatch: 'full'
  },
  {
    path: '#',
    redirectTo: 'games',
    pathMatch: 'full'
  },
  {
    path: 'auth/sign-in',
    component: SignInComponent,
  },
  {
    path: 'auth/sign-up',
    component: SignUpComponent,
  },
  {
    path: 'auth/sign-up/confirm',
    component: SignUpConfirmComponent,
  },
  { 
    path: 'games', 
    component: GameListComponent 
  },
  { 
    path: 'game/:id', 
    component: GameComponent 
  },
  { 
    path: 'score-board', 
    component: ScoreBoardComponent 
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }