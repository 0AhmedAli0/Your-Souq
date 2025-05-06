import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class SnackbarService {
  private _snackBar = inject(MatSnackBar);

  error(message: string) {
    this._snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['snackbar-error'],
    });
  }
  success(message: string) {
    this._snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['snackbar-success'],
    });
  }
}
