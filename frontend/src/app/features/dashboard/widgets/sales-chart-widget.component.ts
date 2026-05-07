import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, Input, OnChanges, SimpleChanges, ViewChild } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { Chart, ChartConfiguration, registerables } from 'chart.js';
import { SalesPoint } from '../dashboard.models';

Chart.register(...registerables);

@Component({
  selector: 'app-sales-chart-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  template: `
    <mat-card class="p-4 rounded-2xl">
      <h3 class="font-semibold text-slate-900 dark:text-slate-100">Sales vs Payout vs Profit</h3>
      <div class="mt-3 h-72">
        <canvas #canvas></canvas>
      </div>
    </mat-card>
  `
})
export class SalesChartWidgetComponent implements AfterViewInit, OnChanges {
  @Input() points: SalesPoint[] = [];
  @ViewChild('canvas') canvas?: ElementRef<HTMLCanvasElement>;

  private chart?: Chart;

  ngAfterViewInit(): void {
    this.renderChart();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['points'] && this.canvas) {
      this.renderChart();
    }
  }

  private renderChart(): void {
    if (!this.canvas) return;

    if (this.chart) {
      this.chart.destroy();
    }

    const labels = this.points.map(x => x.date);
    const sales = this.points.map(x => x.sales);
    const payout = this.points.map(x => x.payout);
    const profit = this.points.map(x => x.profit);

    const config: ChartConfiguration = {
      type: 'line',
      data: {
        labels,
        datasets: [
          { label: 'Sales', data: sales, borderColor: '#0284c7', backgroundColor: 'rgba(2,132,199,.2)', tension: 0.25 },
          { label: 'Payout', data: payout, borderColor: '#ef4444', backgroundColor: 'rgba(239,68,68,.2)', tension: 0.25 },
          { label: 'Profit', data: profit, borderColor: '#16a34a', backgroundColor: 'rgba(22,163,74,.2)', tension: 0.25 }
        ]
      },
      options: { responsive: true, maintainAspectRatio: false }
    };

    this.chart = new Chart(this.canvas.nativeElement, config);
  }
}
