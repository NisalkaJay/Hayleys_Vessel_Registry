import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult, SortDirection, Vessel, VesselType } from './vessel.model';

@Injectable({ providedIn: 'root' })
export class VesselService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/api`;

  getTypes(): Observable<VesselType[]> {
    return this.http.get<VesselType[]>(`${this.baseUrl}/vessel-types`);
  }

  getVessels(filters: { search: string; vesselTypeId?: number; isActive?: boolean; page: number; pageSize: number; sortBy?: string; sortDirection?: SortDirection }): Observable<PagedResult> {
    let params = new HttpParams().set('page', filters.page).set('pageSize', filters.pageSize);
    if (filters.search) params = params.set('search', filters.search);
    if (filters.vesselTypeId) params = params.set('vesselTypeId', filters.vesselTypeId);
    if (filters.isActive !== undefined) params = params.set('isActive', filters.isActive);
    if (filters.sortBy) params = params.set('sortBy', filters.sortBy);
    if (filters.sortDirection) params = params.set('sortDirection', filters.sortDirection);
    return this.http.get<PagedResult>(`${this.baseUrl}/vessels`, { params });
  }

  getVessel(id: number): Observable<Vessel> {
    return this.http.get<Vessel>(`${this.baseUrl}/vessels/${id}`);
  }

  createVessel(vessel: Omit<Vessel, 'vesselId' | 'vesselTypeName'>): Observable<Vessel> {
    return this.http.post<Vessel>(`${this.baseUrl}/vessels`, vessel);
  }

  updateVessel(id: number, vessel: Omit<Vessel, 'vesselId' | 'vesselTypeName'>): Observable<Vessel> {
    return this.http.put<Vessel>(`${this.baseUrl}/vessels/${id}`, vessel);
  }

  deactivateVessel(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.baseUrl}/vessels/${id}`);
  }
}
