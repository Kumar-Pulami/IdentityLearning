import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthenticationGuard } from '../auth/guards/authentication.guard';
import { BookMeetingRoomComponent } from './components/book-meeting-room/book-meeting-room.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { MeetingRoomComponent } from './components/meeting-room/meeting-room.component';

const routes: Routes = [
  { path: "", children: [
    { path: "", redirectTo:"Dashboard", pathMatch: 'full'},
    { path: "Dashboard" , component: DashboardComponent },
    { path: "MeetingRoom" , component: MeetingRoomComponent },
    { path: "BookMeetingRoom" , component: BookMeetingRoomComponent },
  ], component: HomePageComponent, canActivateChild: [AuthenticationGuard]}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class MainRoutingModule { }