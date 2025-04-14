import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layout/header/header.component';
import { HttpClient } from '@angular/common/http';
import { product } from './shared/models/product';
import { Pagination } from './shared/models/pagination';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [/*RouterOutlet, */ HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  baseUrl = 'https://localhost:7196/api/';
  // constructor(private _HttpClient: HttpClient) {} // old inject
  private http = inject(HttpClient);
  products: product[] = [];

  title = 'Client';
  ngOnInit(): void {
    //recive observable and cast it into products array using map function >> cast بمعني وضع او صب
    this.http.get<Pagination<product>>(this.baseUrl + 'Products').subscribe({
      next: (response) => (this.products = response.data),
      error: (err) => console.log(err),
      complete() {
        console.log('Complete');
      },
    });
  }
}
