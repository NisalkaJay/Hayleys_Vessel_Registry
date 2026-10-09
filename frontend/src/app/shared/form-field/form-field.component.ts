import { Component, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-form-field',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <label [for]="id()">{{ label() }}</label>
    <input [id]="id()" [type]="type()" [formControl]="control()" [placeholder]="placeholder()" />
    @if (serverError()) {
      <span class="field-error">{{ serverError() }}</span>
    } @else if (control().invalid && (control().touched || control().dirty)) {
      <span class="field-error">{{ errorMessage() }}</span>
    }
  `
})
export class FormFieldComponent {
  readonly id = input.required<string>();
  readonly label = input.required<string>();
  readonly control = input.required<FormControl>();
  readonly type = input('text');
  readonly placeholder = input('');
  readonly serverError = input<string | undefined>();

  errorMessage(): string {
    const errors = this.control().errors;
    if (!errors) return '';
    if (errors['required']) return `${this.label()} is required.`;
    if (errors['pattern']) return 'Enter a valid value.';
    if (errors['maxlength']) return `Maximum ${errors['maxlength'].requiredLength} characters.`;
    if (errors['min']) return `Value must be at least ${errors['min'].min}.`;
    return 'Please check this value.';
  }
}
