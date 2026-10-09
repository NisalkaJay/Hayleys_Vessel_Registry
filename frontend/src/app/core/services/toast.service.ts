import { Injectable, signal } from '@angular/core';

export type Toast = { type: 'success' | 'error'; message: string };

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly current = signal<Toast | null>(null);

  show(type: Toast['type'], message: string): void {
    this.current.set({ type, message });
    window.setTimeout(() => this.current.set(null), 4000);
  }
}
