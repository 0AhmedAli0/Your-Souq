import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr'; // Assuming you are using @microsoft/signalr for SignalR connections
import { Order } from '../../shared/models/order';

@Injectable({
  providedIn: 'root',
})
export class SignalrService {
  hubUrl = environment.hubUrl;
  hubConnection?: HubConnection;
  orderSignal = signal<Order | null>(null);

  CreateHubConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        withCredentials: true, // Use withCredentials if your API requires credentials
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch((err) =>
        console.error('Error while starting SignalR connection: ', err)
      );

    // Listen for the 'OrderCompleteNotification' event
    this.hubConnection.on('OrderCompleteNotification', (order: Order) => {
      this.orderSignal.set(order); // Update the signal with the received order
      console.log('Order complete notification received:', order);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection
        .stop()
        .catch((err) =>
          console.error('Error while stopping SignalR connection: ', err)
        );
    }
  }
}
