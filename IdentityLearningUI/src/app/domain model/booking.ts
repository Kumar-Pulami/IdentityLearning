import { MeetingRoom } from "./MeetingRoom";
import { User } from "./User";

export class Booking{
    id: number;
    bookedDate: string;
    startTime: Date;
    endTime: Date;
    isDeleted: boolean;
    isDeletable: boolean;
    user: User;
    meetingRoom: MeetingRoom;       
}