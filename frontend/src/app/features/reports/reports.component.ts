import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { Chart, registerables } from 'chart.js';
import { ReportsApiService } from './reports-api.service';
import { DailySalesRow, ReportFilter } from './reports.models';

Chart.register(...registerables);

@Component({
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, MatProgressSpinnerModule, MatTableModule, MatTabsModule, MatPaginatorModule],
  templateUrl: './reports.component.html'
})
export class ReportsComponent implements OnInit {
  loading = false;
  active = 0;
  totalCount = 0;
  data: any[] = [];
  columns: string[] = ['col1', 'col2', 'col3', 'col4'];
  filterForm: FormGroup;

  constructor(private fb: FormBuilder, private api: ReportsApiService) {
    this.filterForm = this.fb.group({
      fromUtc: [this.isoDay(-7)],
      toUtc: [this.isoDay(0)],
      keyword: [''],
      page: [1],
      pageSize: [10]
    });
  }

  ngOnInit(): void { this.load(); }

  private isoDay(delta: number) {
    const d = new Date();
    d.setDate(d.getDate() + delta);
    d.setHours(0, 0, 0, 0);
    return d.toISOString();
  }

  private filter(): ReportFilter {
    return {
      fromUtc: this.filterForm.value.fromUtc,
      toUtc: this.filterForm.value.toUtc,
      keyword: this.filterForm.value.keyword || undefined,
      page: this.filterForm.value.page,
      pageSize: this.filterForm.value.pageSize
    };
  }

  load() {
    this.loading = true;
    const f = this.filter();
    const req = [this.api.dailySales(f), this.api.numberSummary(f), this.api.riskNumbers(f), this.api.customerHistory(f), this.api.profitLoss(f)][this.active];
    req.subscribe({
      next: (res: any) => {
        const data = res?.Data ?? res?.data;
        this.data = data?.Items ?? [];
        this.totalCount = data?.TotalCount ?? 0;
        this.loading = false;
        if (this.active === 0) this.renderSalesChart(this.data as DailySalesRow[]);
      },
      error: () => this.loading = false
    });
  }

  tabChanged(index: number) {
    this.active = index;
    this.filterForm.patchValue({ page: 1 });
    this.load();
  }

  onPage(e: PageEvent) {
    this.filterForm.patchValue({ page: e.pageIndex + 1, pageSize: e.pageSize });
    this.load();
  }

  exportExcel() { this.api.exportExcel(this.filter()).subscribe(b => this.download(b, 'daily-sales.xlsx')); }
  exportPdf() { this.api.exportPdf(this.filter()).subscribe(b => this.download(b, 'daily-sales.pdf')); }

  private download(blob: Blob, filename: string) {
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url; a.download = filename; a.click();
    URL.revokeObjectURL(url);
  }

  private renderSalesChart(rows: DailySalesRow[]) {
    const canvas = document.getElementById('salesChart') as HTMLCanvasElement | null;
    if (!canvas) return;
    const existing = Chart.getChart(canvas);
    if (existing) existing.destroy();

    new Chart(canvas, {
      type: 'bar',
      data: {
        labels: rows.map(x => x.Date),
        datasets: [{ label: 'Sales', data: rows.map(x => x.SalesAmount), backgroundColor: '#0284c7' }]
      },
      options: { responsive: true, maintainAspectRatio: false }
    });
  }
}
