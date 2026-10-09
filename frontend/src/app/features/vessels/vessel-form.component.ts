import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { catchError, of, switchMap } from 'rxjs';
import { ToastService } from '../../core/services/toast.service';
import { FormFieldComponent } from '../../shared/form-field/form-field.component';
import { FormSelectComponent, SelectOption } from '../../shared/form-select/form-select.component';
import { VesselType } from './vessel.model';
import { VesselService } from './vessel.service';

type VesselForm = FormGroup<{
  vesselName: FormControl<string>; imoNumber: FormControl<string>; vesselTypeId: FormControl<number>;
  flagCountry: FormControl<string>; grossTonnage: FormControl<number>; yearBuilt: FormControl<number>; isActive: FormControl<boolean>;
}>;

@Component({
  selector: 'app-vessel-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FormFieldComponent, FormSelectComponent],
  templateUrl: './vessel-form.component.html'
})
export class VesselFormComponent implements OnInit {
  private readonly service = inject(VesselService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);
  private readonly destroyRef = inject(DestroyRef);
  readonly types = signal<VesselType[]>([]);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly notFound = signal(false);
  readonly serverErrors = signal<Record<string, string[]>>({});
  readonly vesselId = signal<number | null>(null);
  readonly form: VesselForm = new FormGroup({
    vesselName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    imoNumber: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.pattern(/^\d{7}$/)] }),
    vesselTypeId: new FormControl(0, { nonNullable: true, validators: [Validators.required, Validators.min(1)] }),
    flagCountry: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(60)] }),
    grossTonnage: new FormControl(0, { nonNullable: true, validators: [Validators.required, Validators.min(0.01)] }),
    yearBuilt: new FormControl(new Date().getFullYear(), { nonNullable: true, validators: [Validators.required, Validators.min(1950), Validators.max(new Date().getFullYear())] }),
    isActive: new FormControl(true, { nonNullable: true })
  });

  ngOnInit(): void {
    this.service.getTypes().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(types => this.types.set(types));
    this.route.paramMap.pipe(
      switchMap(params => {
        const id = params.get('id');
        if (!id) return of(null);
        this.vesselId.set(Number(id));
        this.loading.set(true);
        return this.service.getVessel(Number(id)).pipe(catchError(() => { this.notFound.set(true); return of(null); }));
      }),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(vessel => {
      if (vessel) this.form.patchValue(vessel);
      this.loading.set(false);
    });
  }

  get typeOptions(): SelectOption[] {
    return this.types().map(type => ({ value: type.vesselTypeId, label: type.name }));
  }

  save(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving.set(true);
    this.serverErrors.set({});
    const value = this.form.getRawValue();
    const request = this.vesselId() ? this.service.updateVessel(this.vesselId()!, value) : this.service.createVessel(value);
    request.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => { this.toast.show('success', this.vesselId() ? 'Vessel updated.' : 'Vessel created.'); this.router.navigate(['/vessels']); },
      error: error => {
        const data = error.error?.data;
        if (error.status === 409) this.serverErrors.set({ imoNumber: ['IMO number already exists for this company.'] });
        else if (data && typeof data === 'object') this.serverErrors.set(data);
        this.saving.set(false);
      }
    });
  }
}
