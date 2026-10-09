import { Component, input } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

export type SelectOption = { value: number | string; label: string };

@Component({
  selector: 'app-form-select',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <label [for]="id()">{{ label() }}</label>
    <select [id]="id()" [formControl]="control()">
      @for (option of options(); track option.value) {
        <option [value]="option.value">{{ option.label }}</option>
      }
    </select>
    @if (serverError()) {
      <span class="field-error">{{ serverError() }}</span>
    } @else if (control().invalid && (control().touched || control().dirty)) {
      <span class="field-error">{{ errorMessage() }}</span>
    }
  `
})
export class FormSelectComponent {
  readonly id = input.required<string>();
  readonly label = input.required<string>();
  readonly control = input.required<FormControl>();
  readonly options = input.required<SelectOption[]>();
  readonly serverError = input<string | undefined>();

  errorMessage(): string {
    return this.control().errors?.['required'] ? `${this.label()} is required.` : 'Please select a valid option.';
  }
}
