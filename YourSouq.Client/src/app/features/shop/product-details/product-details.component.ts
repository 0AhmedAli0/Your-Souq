import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ShopService } from '../../../core/services/shop.service';
import { product } from '../../../shared/models/product';
import { CurrencyPipe, NgIf } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';
import { CartService } from '../../../core/services/cart.service';
import { FormsModule } from '@angular/forms';

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
    FormsModule,
    NgIf,
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit {
  private _shopService = inject(ShopService);
  private _ActivatedRoute = inject(ActivatedRoute);
  private _cartService = inject(CartService);
  product?: product;
  quantity = 1;
  quantityInCart = 0;

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    const id = this._ActivatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this._shopService.getProduct(+id).subscribe({
      next: (response) => {
        this.product = response;
        this.UpdateQuantityInCart();
      },
      error: (err) => console.log('get product error'),
    });
  }

  updateCart() {
    if (!this.product) return;
    if (this.quantity > this.quantityInCart) {
      const itemsToAdd = this.quantity - this.quantityInCart;
      this.quantityInCart += itemsToAdd;
      this._cartService.addItemToCart(this.product, itemsToAdd);
    } else {
      const itemsToRemove = this.quantityInCart - this.quantity;
      this.quantityInCart -= itemsToRemove;
      this._cartService.removeItemFromCart(this.product.id, itemsToRemove);
    }
  }

  UpdateQuantityInCart() {
    this.quantityInCart =
      this._cartService
        .cart()
        ?.items.find((item) => item.productId === this.product?.id)?.quantity ||
      0;
    this.quantity = this.quantityInCart || 1;
  }

  getButtonText() {
    return this.quantityInCart > 0 ? 'Update cart' : 'Add to cart';
  }
}
