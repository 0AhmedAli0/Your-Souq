import {
  Directive,
  effect,
  inject,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { AccountService } from '../../core/services/account.service';

@Directive({
  selector: '[appIsAdmin]',
  standalone: true,
})
export class IsAdminDirective {
  private accountService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef); // to access the view container
  private templateRef = inject(TemplateRef); // to access the element

  constructor() {
    effect(() => {
      if (this.accountService.isAdmin()) {
        // if user is admin, render the element
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else {
        // if user is not admin, clear the element
        this.viewContainerRef.clear();
      }
    });
  }
}
