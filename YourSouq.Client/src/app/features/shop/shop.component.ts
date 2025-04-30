import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { product } from '../../shared/models/product';
import { ProductItemComponent } from './product-item/product-item.component';
import { MatDialog } from '@angular/material/dialog';
import { MatButton } from '@angular/material/button';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import {
  MatListOption,
  MatSelectionList,
  MatSelectionListChange,
} from '@angular/material/list';
@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [
    ProductItemComponent,
    MatButton,
    MatIcon,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger,
  ],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})
export class ShopComponent implements OnInit {
  private shopService = inject(ShopService);
  private dialogService = inject(MatDialog);
  products: product[] = [];
  selectedBrands: string[] = [];
  selectedTypes: string[] = [];
  selectedSort: string = 'name';
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low-High', value: 'PriceAsc' },
    { name: 'Price: High-Low', value: 'PriceDesc' },
  ];

  ngOnInit(): void {
    this.initializeShop();
  }

  initializeShop() {
    this.shopService.getBrands();
    this.shopService.getTypes();

    //recive observable and cast it into products array using map function >> cast بمعني وضع او صب
    this.shopService.getProducts().subscribe({
      next: (response) => (this.products = response.data),
      error: (err) => console.log(err),
    });
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.selectedSort = selectedOption.value;
      console.log(selectedOption);
    }
  }

  openFilterDialog() {
    const dialogRef = this.dialogService.open(FiltersDialogComponent, {
      minWidth: '500px',
      data: {
        //علشان يبعت البيانات الي الديالوج علشان اعرضها فيه
        selectedBrands: this.selectedBrands,
        selectedTypes: this.selectedTypes,
      },
    });

    dialogRef.afterClosed().subscribe({
      next: (result) => {
        //لو انا بعت حاجه في الديالوج احفظها في المتغيرات دي
        if (result) {
          this.selectedBrands = result.selectedBrands;
          this.selectedTypes = result.selectedTypes;

          //apply filter
          this.shopService
            .getProducts(this.selectedBrands, this.selectedTypes)
            .subscribe({
              next: (response) => (this.products = response.data),
              error(error) {
                console.log(error);
              },
            });
        }
      },
    });
  }
}
