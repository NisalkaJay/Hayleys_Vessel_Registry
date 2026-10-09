import { CommonModule } from '@angular/common';
import { Component, input, output, TemplateRef } from '@angular/core';

export type TableColumn = { key: string; label: string };
export type PageChange = { page: number; pageSize: number };

@Component({
  selector: 'app-data-table',
  standalone: true,
  template: `
    <div class="table-wrap">
      <table>
        <thead><tr>@for (column of columns(); track column.key) { <th>{{ column.label }}</th> }</tr></thead>
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

  trackRow(row: unknown): unknown {
    return (row as { vesselId?: number }).vesselId ?? row;
  }

  changePage(page: number): void {
    this.pageChange.emit({ page, pageSize: this.pageSize() });
  }
}
