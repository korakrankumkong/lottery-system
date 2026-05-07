import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subscription, interval, startWith, switchMap } from 'rxjs';
import { DashboardApiService } from './dashboard-api.service';
import { DashboardState } from './dashboard.state';
import { MetricCardComponent } from './widgets/metric-card.component';
import { RiskyNumbersWidgetComponent } from './widgets/risky-numbers-widget.component';
import { RecentTicketsWidgetComponent } from './widgets/recent-tickets-widget.component';
import { RoundStatusWidgetComponent } from './widgets/round-status-widget.component';
import { SalesChartWidgetComponent } from './widgets/sales-chart-widget.component';

@Component({
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MetricCardComponent,
    RiskyNumbersWidgetComponent,
    RecentTicketsWidgetComponent,
    RoundStatusWidgetComponent,
    SalesChartWidgetComponent
  ],
  providers: [DashboardState],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly api = inject(DashboardApiService);
  readonly state = inject(DashboardState);
  private pollSub?: Subscription;

  ngOnInit(): void {
    this.pollSub = interval(30000).pipe(
      startWith(0),
      switchMap((tick) => {
        if (tick > 0) this.state.refreshing.set(true);
        if (!this.state.hasData()) this.state.loading.set(true);
        this.state.error.set(null);
        return this.api.getDashboard();
      })
    ).subscribe({
      next: (data) => {
        this.state.data.set(data);
        this.state.loading.set(false);
        this.state.refreshing.set(false);
      },
      error: () => {
        this.state.loading.set(false);
        this.state.refreshing.set(false);
        this.state.error.set('Failed to load dashboard data.');
      }
    });
  }

  ngOnDestroy(): void {
    this.pollSub?.unsubscribe();
  }

  reload(): void {
    this.state.loading.set(true);
    this.api.getDashboard().subscribe({
      next: (data) => {
        this.state.data.set(data);
        this.state.loading.set(false);
      },
      error: () => {
        this.state.loading.set(false);
        this.state.error.set('Failed to load dashboard data.');
      }
    });
  }

  openRoundsCount(): number {
    return this.state.data()?.rounds.filter(r => !r.isClosed).length ?? 0;
  }
}
