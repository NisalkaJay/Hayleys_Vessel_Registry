import { CommonModule } from '@angular/common';
import { Component, input, output, TemplateRef } from '@angular/core';

export type TableColumn = { key: string; label: string; sortable?: boolean };
export type PageChange = { page: number; pageSize: number };
export type SortChange = { sortBy: string; sortDirection: 'asc' | 'desc' };

@Component({
  selector: 'app-data-table',
  standalone: true,
  template: `
    <div class="table-wrap">
      <table>
        <thead><tr>@for (column of columns(); track column.key) {
          <th>
            @if (column.sortable !== false) {
              <button type="button" class="sort-button" (click)="sort(column.key)">
                {{ column.label }} @if (sortBy() === column.key) { <span>{{ sortDirection() === 'asc' ? '▲' : '▼' }}</span> }
              </button>
            } @else { {{ column.label }} }
          </th>
        }</tr></thead>
        <tbody>
          @for (row of rows(); track trackRow(row)) {
            <ng-container [ngTemplateOutlet]="rowTemplate()" [ngTemplateOutletContext]="{ $implicit: row }" />
          } @empty {
            <tr><td [attr.colspan]="columns().length" class="empty-cell">No vessels match your filters.</td></tr>
          }
        </tbody>
      </table>
    </div>
    <div class="pagination">
      <span>{{ totalCount() }} vessels</span>
      <button type="button" [disabled]="page() <= 1" (click)="changePage(page() - 1)">Previous</button>
      <strong>Page {{ page() }}</strong>
      <button type="button" [disabled]="page() * pageSize() >= totalCount()" (click)="changePage(page() + 1)">Next</button>
    </div>
  `,
  imports: [CommonModule]
})
export class DataTableComponent {
  readonly columns = input.required<TableColumn[]>();
  readonly rows = input.required<unknown[]>();
  readonly page = input.required<number>();
  readonly pageSize = input.required<number>();
  readonly totalCount = input.required<number>();
  readonly pageChange = output<PageChange>();
  readonly rowTemplate = input.required<TemplateRef<unknown>>();
  readonly sortBy = input('');
  readonly sortDirection = input<'asc' | 'desc'>('asc');
  readonly sortChange = output<SortChange>();

  trackRow(row: unknown): unknown {
    return (row as { vesselId?: number }).vesselId ?? row;
  }

  changePage(page: number): void {
    this.pageChange.emit({ page, pageSize: this.pageSize() });
  }

  sort(column: string): void {
    const direction = this.sortBy() === column && this.sortDirection() === 'asc' ? 'desc' : 'asc';
    this.sortChange.emit({ sortBy: column, sortDirection: direction });
  }
}
