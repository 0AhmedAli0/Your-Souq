import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';
import { PaymentSummary } from '../models/order';

@Pipe({
  name: 'payment',
  standalone: true,
})
export class PaymentPipe implements PipeTransform {
  transform(
    value?: ConfirmationToken['payment_method_preview'] | PaymentSummary,
    ...args: unknown[]
  ): unknown {
    if (value && 'card' in value) {
      const { brand, exp_year, exp_month, last4 } = (
        value as ConfirmationToken['payment_method_preview']
      ).card!; //destructuring for better readability
      return `${brand.toUpperCase()} **** **** **** ${last4}, Expires: ${exp_month}/${exp_year}`;
    } else if (value && 'last4' in value) {
      const { brand, expYear, expMonth, last4 } = value as PaymentSummary;
      return `${brand.toUpperCase()} **** **** **** ${last4}, Expires: ${expMonth}/${expYear}`;
    } else {
      return 'unknown address';
    }
  }
}
