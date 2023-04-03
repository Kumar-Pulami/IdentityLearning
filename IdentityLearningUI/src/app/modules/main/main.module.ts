import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';

import { MainRoutingModule } from './main-routing.module';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { MeetingRoomComponent } from './components/meeting-room/meeting-room.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { NavbarServices } from './services/navBarServices/navBarService.service';
import { BookMeetingRoomComponent } from './components/book-meeting-room/book-meeting-room.component';
import { MeetingRoomApiService } from './services/apiServices/meetingRoomApiService.service';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    DashboardComponent,
    MeetingRoomComponent,
    HomePageComponent,
    BookMeetingRoomComponent,
  ],
  
  imports: [
    CommonModule,
    MainRoutingModule,
    ReactiveFormsModule
  ],

  providers: [
    NavbarServices,
    MeetingRoomApiService,
    DatePipe
  ]
})

export class MainModule { }