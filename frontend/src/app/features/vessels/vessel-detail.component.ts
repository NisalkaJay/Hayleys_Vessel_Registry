import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, of, switchMap } from 'rxjs';
import { Vessel } from './vessel.model';
import { VesselService } from './vessel.service';

@Component({
  selector: 'app-vessel-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    @if (loading()) {
      <div class="state card">Loading vessel...</div>
    } @else if (notFound()) {
      <div class="state card error-state">
        <h1>Vessel not found</h1>
        <p>This vessel does not exist or does not belong to your company.</p>
        <a class="button" routerLink="/vessels">Back to vessels</a>
      </div>
    } @else if (vessel(); as item) {
      <section class="page-heading">
        <div><p class="eyebrow">Vessel details</p><h1>{{ item.vesselName }}</h1><p class="muted">{{ item.imoNumber }}</p></div>
        <div class="actions"><a class="button" routerLink="/vessels">Back</a><a class="button primary" [routerLink]="['/vessels', item.vesselId, 'edit']">Edit</a></div>
      </section>
      <section class="card detail-grid">
        <div><span>Vessel name</span><strong>{{ item.vesselName }}</strong></div>
        <div><span>IMO number</span><strong class="mono">{{ item.imoNumber }}</strong></div>
        <div><span>Type</span><strong>{{ item.vesselTypeName }}</strong></div>
        <div><span>Flag country</span><strong>{{ item.flagCountry }}</strong></div>
        <div><span>Gross tonnage</span><strong>{{ item.grossTonnage | number }}</strong></div>
        <div><span>Year built</span><strong>{{ item.yearBuilt }}</strong></div>
        <div><span>Status</span><strong>{{ item.isActive ? 'Active' : 'Inactive' }}</strong></div>
      </section>
    }
  `
})
export class VesselDetailComponent implements OnInit {
  private readonly service = inject(VesselService);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  readonly vessel = signal<Vessel | null>(null);
  readonly loading = signal(true);
  readonly notFound = signal(false);

  ngOnInit(): void {
    this.route.paramMap.pipe(
      switchMap(params => this.service.getVessel(Number(params.get('id'))).pipe(
        catchError(() => { this.notFound.set(true); return of(null); })
      )),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(item => {
      this.vessel.set(item);
      this.loading.set(false);
    });
  }
}
