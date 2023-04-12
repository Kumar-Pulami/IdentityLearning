import { Component } from '@angular/core';
import { NavbarServices } from '../../services/navBarServices/navBarService.service';

@Component({
  selector: 'app-meeting-room-bookings',
  templateUrl: './meeting-room-bookings.component.html',
  styleUrls: ['./meeting-room-bookings.component.scss']
})
export class MeetingRoomBookingsComponent {
  constructor(
    private navBarService: NavbarServices,
  ){}


  ngOnInit(){
    this.navBarService.selectNavBarServiceEmitter.emit('Booking');
  }
}
