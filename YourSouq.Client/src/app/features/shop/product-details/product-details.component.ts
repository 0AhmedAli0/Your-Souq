import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ShopService } from '../../../core/services/shop.service';
import { product } from '../../../shared/models/product';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [
    CurrencyPipe,
    MatButton,
    MatIcon,
    MatFormField,
    MatInput,
    MatLabel,
    MatDivider,
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit {
  private _shopService = inject(ShopService);
  private _ActivatedRoute = inject(ActivatedRoute);
  product?: product;

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    const id = this._ActivatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this._shopService.getProduct(+id).subscribe({
      next: (response) => (this.product = response),
      error: (err) => console.log('get product error'),
    });
  }
}
