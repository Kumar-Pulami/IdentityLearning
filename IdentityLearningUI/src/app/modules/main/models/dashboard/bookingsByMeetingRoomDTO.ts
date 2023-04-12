import { MeetingRoomBookingsDTO } from "./meetingRoomBookingsDTO";

export class BookingsByMeetingRoomDTO{
    meetingRoomId: number;
    meetingRoomName: string;
    bookings: MeetingRoomBookingsDTO[];
}