import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RiskyNumber } from '../dashboard.models';

@Component({
  selector: 'app-risky-numbers-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  template: `
    <mat-card class="p-4 rounded-2xl">
      <h3 class="font-semibold text-slate-900 dark:text-slate-100">Top Risky Numbers</h3>
      <div class="mt-3 space-y-2" *ngIf="items.length; else empty">
        <div *ngFor="let item of items" class="flex items-center justify-between rounded-lg px-3 py-2 bg-slate-50 dark:bg-slate-800">
          <div>
            <div class="font-semibold">{{ item.number }}</div>
            <div class="text-xs text-slate-500 dark:text-slate-400">{{ item.frequency }} bets</div>
          </div>
          <div class="font-bold">{{ item.totalAmount | number:'1.0-2' }}</div>
        </div>
      </div>
      <ng-template #empty><p class="mt-3 text-sm text-slate-500">No risky numbers yet.</p></ng-template>
    </mat-card>
  `
})
export class RiskyNumbersWidgetComponent {
  @Input() items: RiskyNumber[] = [];
}
