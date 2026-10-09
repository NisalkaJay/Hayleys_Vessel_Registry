import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { DataTableComponent, PageChange, TableColumn } from '../../shared/data-table/data-table.component';
import { ToastService } from '../../core/services/toast.service';
import { Vessel, VesselType } from './vessel.model';
import { VesselService } from './vessel.service';

@Component({
  selector: 'app-vessel-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, DataTableComponent],
  templateUrl: './vessel-list.component.html'
})
export class VesselListComponent implements OnInit {
  private readonly service = inject(VesselService);
  private readonly toast = inject(ToastService);
  private readonly destroyRef = inject(DestroyRef);
  readonly search = new FormControl('', { nonNullable: true });
  readonly typeFilter = new FormControl('', { nonNullable: true });
  readonly statusFilter = new FormControl('all', { nonNullable: true });
  readonly vessels = signal<Vessel[]>([]);
  readonly types = signal<VesselType[]>([]);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly totalCount = signal(0);
  readonly hasRows = computed(() => this.vessels().length > 0);
  readonly columns: TableColumn[] = [
    { key: 'vesselName', label: 'Vessel name' }, { key: 'imoNumber', label: 'IMO' },
    { key: 'vesselTypeName', label: 'Type' }, { key: 'flagCountry', label: 'Flag' },
    { key: 'grossTonnage', label: 'GT' }, { key: 'yearBuilt', label: 'Year built' },
    { key: 'isActive', label: 'Status' }, { key: 'actions', label: 'Actions' }
  ];

  ngOnInit(): void {
    this.service.getTypes().pipe(takeUntilDestroyed(this.destroyRef)).subscribe(types => this.types.set(types));
    this.search.valueChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe(() => { this.page.set(1); this.load(); });
    this.typeFilter.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => { this.page.set(1); this.load(); });
    this.statusFilter.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => { this.page.set(1); this.load(); });
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set('');
    const status = this.statusFilter.value;
    this.service.getVessels({
      search: this.search.value,
      vesselTypeId: this.typeFilter.value ? Number(this.typeFilter.value) : undefined,
      isActive: status === 'all' ? undefined : status === 'active',
      page: this.page(),
      pageSize: this.pageSize()
    }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: result => { this.vessels.set(result.items); this.totalCount.set(result.totalCount); this.loading.set(false); },
      error: () => { this.error.set('We could not load the vessels. Please try again.'); this.loading.set(false); }
    });
  }

  changePage(event: PageChange): void {
    this.page.set(event.page);
    this.load();
  }

  deactivate(vessel: Vessel): void {
    if (!confirm(`Deactivate ${vessel.vesselName}?`)) return;
    this.service.deactivateVessel(vessel.vesselId).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => { this.toast.show('success', 'Vessel deactivated.'); this.load(); }
    });
  }

  reactivate(vessel: Vessel): void {
    if (!confirm(`Reactivate ${vessel.vesselName}?`)) return;
    this.service.getVessel(vessel.vesselId).pipe(
      switchMap(current => this.service.updateVessel(current.vesselId, { ...current, isActive: true })),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe({
      next: () => { this.toast.show('success', 'Vessel reactivated.'); this.load(); }
    });
  }
}
