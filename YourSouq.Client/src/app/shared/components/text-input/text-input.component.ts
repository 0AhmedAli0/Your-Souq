import { Component, Input, Self } from '@angular/core';
import {
  ControlValueAccessor,
  FormControl,
  NgControl,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';

@Component({
  selector: 'app-text-input',
  standalone: true,
  imports: [ReactiveFormsModule, MatFormField, MatInput, MatError, MatLabel],
  templateUrl: './text-input.component.html',
  styleUrl: './text-input.component.scss',
})
export class TextInputComponent implements ControlValueAccessor {
  //Angular forms تمكنك من جعل مكونك الخاص يتصرف مثل عناصر الإدخال الأصلية في
  //Create re-usable text input
  @Input() label = '';
  @Input() type = 'text';

  constructor(@Self() public controlDir: NgControl) {
    //@Self()>>يتم استخدامها عندما تريد التأكد من أن الخدمة مُقدمة في هذا المكون فقط ولم يتم استخدامها في اي مكون اخر
    //يعيد استخدام الخدمات مره اخر  Dependency Injection لانه من الطبيعي في
    //NgControl>> FormControl object to a DOM element تقوم بالربط بين
    //NgControl>> تسمح بالوصول إلى القيمة، حالة الصحة ، وغيرها من خصائص عنصر الإدخال

    this.controlDir.valueAccessor = this; //Assigning the control value accessor
  }
  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}

  get control() {
    return this.controlDir.control as FormControl;
  }
}
