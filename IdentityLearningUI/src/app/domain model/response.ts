import { AnimationQueryMetadata } from "@angular/animations";

export class Response<T>{
    success: boolean;
    payload: T | null | undefined;
    error: string[];
}