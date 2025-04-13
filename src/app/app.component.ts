import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layout/header/header.component';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [/*RouterOutlet, */ HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  baseUrl = 'https://localhost:7196/api/';
  // constructor(private _HttpClient: HttpClient) {} // old inject
  private http = inject(HttpClient);

  title = 'Client';
}
