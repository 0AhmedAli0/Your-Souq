import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { product } from '../../shared/models/product';
import { ProductItemComponent } from './product-item/product-item.component';
import { MatDialog } from '@angular/material/dialog';
import { MatButton } from '@angular/material/button';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [ProductItemComponent, MatButton, MatIcon],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})
export class ShopComponent implements OnInit {
  private shopService = inject(ShopService);
  private dialogService = inject(MatDialog);
  products: product[] = [];

  ngOnInit(): void {
    this.initializeShop();
  }

  initializeShop() {
    this.shopService.getBrand();
    this.shopService.getTypes();

    //recive observable and cast it into products array using map function >> cast بمعني وضع او صب
    this.shopService.getProducts().subscribe({
      next: (response) => (this.products = response.data),
      error: (err) => console.log(err),
    });
  }

  openFilterDialog() {
    const dialogRef = this.dialogService.open(FiltersDialogComponent, {
      minWidth: '500px',
    });
  }
}
