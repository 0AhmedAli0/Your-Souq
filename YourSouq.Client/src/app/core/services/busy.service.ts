import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  //in this service we will track if something is loading or not
  busyRequestCount = 0; //this will track the number of requests that are in progress
  loading = false;

  busy() {
    this.busyRequestCount++; //increment the request count when a new request is made
    this.loading = true; //set loading to true when a request is made
  }
  idle() {
    this.busyRequestCount--; //decrement the request count when a request is completed
    if (this.busyRequestCount <= 0) {
      //if there are no more requests in progress
      this.loading = false;
      this.busyRequestCount = 0;
    }
  }
}
