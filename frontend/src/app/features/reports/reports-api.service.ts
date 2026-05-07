import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ReportFilter } from './reports.models';

@Injectable({ providedIn: 'root' })
export class ReportsApiService {
  private readonly base = `${environment.apiUrl}/reports`;
  constructor(private http: HttpClient) {}

  private params(f: ReportFilter) {
    let p = new HttpParams()
      .set('fromUtc', f.fromUtc)
      .set('toUtc', f.toUtc)
      .set('page', f.page)
      .set('pageSize', f.pageSize);
    if (f.keyword) p = p.set('keyword', f.keyword);
    return { params: p };
  }

  dailySales(f: ReportFilter) { return this.http.get<any>(`${this.base}/daily-sales`, this.params(f)); }
  numberSummary(f: ReportFilter) { return this.http.get<any>(`${this.base}/number-summary`, this.params(f)); }
  riskNumbers(f: ReportFilter) { return this.http.get<any>(`${this.base}/risk-numbers`, this.params(f)); }
  customerHistory(f: ReportFilter) { return this.http.get<any>(`${this.base}/customer-history`, this.params(f)); }
  profitLoss(f: ReportFilter) { return this.http.get<any>(`${this.base}/profit-loss`, this.params(f)); }

  exportExcel(f: ReportFilter) { return this.http.get(`${this.base}/daily-sales/export/excel`, { ...this.params(f), responseType: 'blob' }); }
  exportPdf(f: ReportFilter) { return this.http.get(`${this.base}/daily-sales/export/pdf`, { ...this.params(f), responseType: 'blob' }); }
}
