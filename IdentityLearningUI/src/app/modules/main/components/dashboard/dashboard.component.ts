import { Component } from '@angular/core';
import { NavbarServices } from '../../services/navBarServices/navBarService.service';
import { DashboardApiService } from '../../services/apiServices/dashboardApiServices.service';
import { Response } from 'src/app/domain model/response';
import { BookingsByMeetingRoomDTO } from '../../models/dashboard/bookingsByMeetingRoomDTO';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  providers: [DashboardApiService]
})

export class DashboardComponent {
  bookings: BookingsByMeetingRoomDTO[];
  constructor(
    private navBarService: NavbarServices,
    private dashboardApiService: DashboardApiService
  ){}

  ngOnInit(){
    this.navBarService.selectNavBarServiceEmitter.emit('Dashboard');
    this.dashboardApiService.GetAllBookingsByMeetingRoom().subscribe((data: Response<BookingsByMeetingRoomDTO[]>) => {
      console.log(data)
      if(data.success && data.payload ){
        this.bookings = data.payload;
        console.log(data.payload);
        console.log(this.bookings);

      }
    })
  }
}