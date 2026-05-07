import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RoundStatus } from '../dashboard.models';

@Component({
  selector: 'app-round-status-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  template: `
    <mat-card class="p-4 rounded-2xl">
      <h3 class="font-semibold text-slate-900 dark:text-slate-100">Lottery Round Status</h3>
      <div class="mt-3 space-y-2" *ngIf="rounds.length; else empty">
        <div *ngFor="let round of rounds" class="flex items-center justify-between rounded-lg px-3 py-2 bg-slate-50 dark:bg-slate-800">
          <div>
            <div class="font-semibold">{{ round.code }}</div>
            <div class="text-xs text-slate-500">{{ round.drawDate }}</div>
          </div>
          <span class="text-xs px-2 py-1 rounded-full" [class]="round.isClosed ? 'bg-slate-300 text-slate-700 dark:bg-slate-700 dark:text-slate-100' : 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900 dark:text-emerald-200'">
            {{ round.isClosed ? 'Closed' : 'Open' }}
          </span>
        </div>
      </div>
      <ng-template #empty><p class="mt-3 text-sm text-slate-500">No rounds available.</p></ng-template>
    </mat-card>
  `
})
export class RoundStatusWidgetComponent {
  @Input() rounds: RoundStatus[] = [];
}
