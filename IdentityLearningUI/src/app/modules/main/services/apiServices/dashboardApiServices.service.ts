import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { localHostUrl } from "src/app/constant";
import { Response } from "src/app/domain model/response";
import { BookingsByMeetingRoomDTO } from "../../models/dashboard/bookingsByMeetingRoomDTO";

@Injectable()

export class DashboardApiService{

    constructor(
        private httpClient: HttpClient
    ){}

    private readonly getAllBookingsByMeetingRoomApiUrl: string = localHostUrl + "dashboard/getBookingsByMeetingRoom";

    GetAllBookingsByMeetingRoom(): Observable<Response<BookingsByMeetingRoomDTO[]>>{
        return this.httpClient.get<Response<BookingsByMeetingRoomDTO[]>>(this.getAllBookingsByMeetingRoomApiUrl);
    }
}