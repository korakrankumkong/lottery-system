import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { RecentTicket } from '../dashboard.models';

@Component({
  selector: 'app-recent-tickets-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  template: `
    <mat-card class="p-4 rounded-2xl">
      <h3 class="font-semibold text-slate-900 dark:text-slate-100">Recent Tickets</h3>
      <div class="mt-3 space-y-2" *ngIf="tickets.length; else empty">
        <div *ngFor="let ticket of tickets" class="rounded-lg px-3 py-2 bg-slate-50 dark:bg-slate-800">
          <div class="flex items-center justify-between">
            <span class="font-semibold">{{ ticket.totalAmount | number:'1.0-2' }}</span>
            <span class="text-xs text-slate-500">{{ ticket.createdAtUtc | date:'shortTime' }}</span>
          </div>
          <div class="text-xs text-slate-500 mt-1">Ticket #{{ ticket.id.slice(0, 8) }}</div>
        </div>
      </div>
      <ng-template #empty><p class="mt-3 text-sm text-slate-500">No tickets found.</p></ng-template>
    </mat-card>
  `
})
export class RecentTicketsWidgetComponent {
  @Input() tickets: RecentTicket[] = [];
}
