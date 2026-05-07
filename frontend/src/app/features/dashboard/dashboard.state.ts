import { Injectable, computed, signal } from '@angular/core';
import { DashboardViewModel } from './dashboard.models';

@Injectable()
export class DashboardState {
  readonly loading = signal(true);
  readonly refreshing = signal(false);
  readonly error = signal<string | null>(null);
  readonly data = signal<DashboardViewModel | null>(null);

  readonly hasData = computed(() => !!this.data());
}
