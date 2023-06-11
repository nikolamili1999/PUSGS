import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
// Sluzi za prenos dogadjaja na globalnom nivou u okviru svih komponenti u okviru modula
export class EventService {

  public eventObservable = new BehaviorSubject<Object>(null);

  constructor() { }

  refresh(){
    this.eventObservable.next(new Object());
  }
}
