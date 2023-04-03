import { Component } from '@angular/core';
import { NavbarServices } from '../../services/navBarServices/navBarService.service';

@Component({
  selector: 'app-meeting-room',
  templateUrl: './meeting-room.component.html',
  styleUrls: ['./meeting-room.component.scss']
})
export class MeetingRoomComponent {
  constructor(
    private navBarService: NavbarServices
  ){}

  ngOnInit(){
    this.navBarService.selectNavBarServiceEmitter.emit('MeetingRoom');
  }
}
