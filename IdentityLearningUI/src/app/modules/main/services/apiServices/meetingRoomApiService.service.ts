import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { localHostUrl } from "src/app/constant";
import { MeetingRoom } from "src/app/domain model/MeetingRoom";


@Injectable()


export class MeetingRoomApiService{

    constructor(
        private httpClient: HttpClient
    ){}

    GetAllMeetingRoom(): Observable<MeetingRoom[]> {
        const getAllMeetingRoomApiUrl: string  = localHostUrl + "meetingRoom/getAllMeetingRooms"
        return this.httpClient.get<MeetingRoom[]>(getAllMeetingRoomApiUrl);
    }
}