export class ShopParams {
  pageNumber: number = 1;
  pageSize: number = 10;
  sort: string = 'name';
  search: string = '';
  brands: string[] = [];
  types: string[] = [];

  // constructor(init?: Partial<ShopParams>) {
  //     Object.assign(this, init);
  // }
}
