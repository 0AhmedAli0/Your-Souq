import { Component, effect, inject, OnInit, signal } from '@angular/core';
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
import { ShopParams } from '../../shared/models/shopParams';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Pagination } from '../../shared/models/pagination';
import { FormsModule } from '@angular/forms';
import { EmptyStateComponent } from '../../shared/components/empty-state/empty-state.component';

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
    MatPaginator,
    FormsModule,
    EmptyStateComponent,
  ],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss',
})
export class ShopComponent implements OnInit {
  private shopService = inject(ShopService);
  private dialogService = inject(MatDialog);
  products?: Pagination<product>; //this will be used to display the products
  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low-High', value: 'priceAsc' },
    { name: 'Price: High-Low', value: 'priceDesc' },
  ];

  shopParams = new ShopParams(); //this will be used to filter the products
  pageSizeOptions = [5, 10, 15, 20]; //this will be used to display the number of items per page

  constructor() {
    effect(() => {
      this.shopParams.search = this.shopService.headerSearchValue(); //get the search value from the header service
      this.onSearchChange();
    });
  }

  ngOnInit(): void {
    if (this.shopService.headerSearchValue() !== '') {
      this.shopParams.search = this.shopService.headerSearchValue(); //set the search value from the header service if it exists
      this.onSearchChange();
    } else {
      this.initializeShop();
    }
  }

  initializeShop() {
    this.shopService.getBrands();
    this.shopService.getTypes();
    this.getProducts();
  }

  resetFilters() {
    this.shopParams = new ShopParams(); //reset the shopParams to default values
    this.getProducts(); //call the getProducts function to get the products based on the default values
  }

  getProducts() {
    //recive observable and cast it into products array using map function >> cast بمعني وضع او صب
    this.shopService.getProducts(this.shopParams).subscribe({
      next: (response) => (this.products = response),
      error: (err) => console.log(err),
    });
  }

  onSearchChange() {
    this.shopParams.pageNumber = 1; //reset page number to 1 when the user change the search option
    this.getProducts(); //call the getProducts function to get the products based on the search value
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];
    if (selectedOption) {
      this.shopParams.sort = selectedOption.value;
      this.shopParams.pageNumber = 1; //reset page number to 1 when the user change the sort option
      this.getProducts();
    }
  }

  handlePageEvent(e: PageEvent) {
    //this will be used to handle the page event when the user change the page size or page number
    this.shopParams.pageSize = e.pageSize;
    this.shopParams.pageNumber = e.pageIndex + 1; //pageIndex is 0 based, so we need to add 1 to it
    this.getProducts();
  }

  openFilterDialog() {
    const dialogRef = this.dialogService.open(FiltersDialogComponent, {
      minWidth: '500px',
      data: {
        //علشان يبعت البيانات الي الديالوج علشان اعرضها فيه
        selectedBrands: this.shopParams.brands,
        selectedTypes: this.shopParams.types,
      },
    });

    dialogRef.afterClosed().subscribe({
      next: (result) => {
        //لو انا بعت حاجه في الديالوج احفظها في المتغيرات دي
        if (result) {
          this.shopParams.brands = result.selectedBrands;
          this.shopParams.types = result.selectedTypes;
          this.shopParams.pageNumber = 1; //reset page number to 1 when the user change the filter option

          //apply filter
          this.getProducts();
        }
      },
    });
  }
}
