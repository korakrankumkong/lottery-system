import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-metric-card',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  template: `
    <mat-card class="p-4 rounded-2xl h-full">
      <div class="text-xs uppercase tracking-wide text-slate-500 dark:text-slate-400">{{ title }}</div>
      <div class="text-2xl md:text-3xl font-bold mt-2 text-slate-900 dark:text-slate-100">
        <ng-container *ngIf="currency; else plain">{{ value | number:'1.0-2' }}</ng-container>
        <ng-template #plain>{{ value }}</ng-template>
      </div>
      <div *ngIf="subtitle" class="mt-1 text-xs text-slate-500 dark:text-slate-400">{{ subtitle }}</div>
    </mat-card>
  `
})
export class MetricCardComponent {
  @Input({ required: true }) title = '';
  @Input() value = 0;
  @Input() subtitle = '';
  @Input() currency = true;
}
