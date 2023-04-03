import { EventEmitter } from "@angular/core";

export class NavbarServices{
    selectNavBarServiceEmitter: EventEmitter<string> = new EventEmitter<string>();
}