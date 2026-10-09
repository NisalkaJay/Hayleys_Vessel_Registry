import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'vessels' },
  { path: 'vessels', loadComponent: () => import('./features/vessels/vessel-list.component').then(m => m.VesselListComponent) },
  { path: 'vessels/new', loadComponent: () => import('./features/vessels/vessel-form.component').then(m => m.VesselFormComponent) },
  { path: 'vessels/:id/edit', loadComponent: () => import('./features/vessels/vessel-form.component').then(m => m.VesselFormComponent) },
  { path: 'vessels/:id', loadComponent: () => import('./features/vessels/vessel-detail.component').then(m => m.VesselDetailComponent) },
  { path: '**', redirectTo: 'vessels' }
];
