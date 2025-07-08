import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';

@Pipe({
  name: 'payment',
  standalone: true,
})
export class PaymentPipe implements PipeTransform {
  transform(
    value?: ConfirmationToken['payment_method_preview'],
    ...args: unknown[]
  ): unknown {
    if (value?.card) {
      const { brand, exp_year, exp_month, last4 } = value.card; //destructuring for better readability
      return `${brand.toUpperCase()} **** **** **** ${last4}, Expires: ${exp_month}/${exp_year}`;
    } else {
      return 'unknown address';
    }
  }
}
