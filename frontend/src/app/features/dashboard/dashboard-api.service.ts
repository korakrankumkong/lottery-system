import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, forkJoin, map, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardViewModel, RecentTicket, RiskyNumber, RoundStatus, SalesPoint } from './dashboard.models';

@Injectable({ providedIn: 'root' })
export class DashboardApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getDashboard() {
    const now = new Date();
    const start = new Date(now);
    start.setHours(0, 0, 0, 0);

    const from30Days = new Date(now);
    from30Days.setDate(from30Days.getDate() - 29);
    from30Days.setHours(0, 0, 0, 0);

    return forkJoin({
      totalsToday: this.http.get<any>(`${this.baseUrl}/tickets/totals`, { params: { fromUtc: start.toISOString(), toUtc: now.toISOString() } }).pipe(catchError(() => of({ data: { totalAmount: 0 } }))),
      risky: this.http.get<any>(`${this.baseUrl}/tickets/risky-numbers`, { params: { fromUtc: start.toISOString(), toUtc: now.toISOString(), top: 10 } }).pipe(catchError(() => of({ data: [] }))),
      history: this.http.get<any>(`${this.baseUrl}/tickets/history`, { params: { page: 1, pageSize: 8 } }).pipe(catchError(() => of({ data: [] }))),
      rounds: this.http.get<RoundStatus[]>(`${this.baseUrl}/lotteryrounds`).pipe(catchError(() => of([] as RoundStatus[]))),
      daily: this.http.get<any>(`${this.baseUrl}/tickets/daily-summary`, { params: { fromUtc: from30Days.toISOString(), toUtc: now.toISOString() } }).pipe(catchError(() => of({ data: [] })))
    }).pipe(
      map(({ totalsToday, risky, history, rounds, daily }): DashboardViewModel => {
        const totalsData = totalsToday?.Data ?? totalsToday?.data ?? totalsToday;
        const riskyData = risky?.Data ?? risky?.data ?? risky ?? [];
        const historyData = history?.Data ?? history?.data ?? history ?? [];
        const dailyData = daily?.Data ?? daily?.data ?? daily ?? [];

        const totalSalesToday = totalsData?.TotalAmount ?? totalsData?.totalAmount ?? 0;
        const totalPayout = totalSalesToday * 0.62;
        const profitSummary = totalSalesToday - totalPayout;

        const topRiskyNumbers: RiskyNumber[] = riskyData;
        const recentTickets: RecentTicket[] = historyData;

        const chart: SalesPoint[] = dailyData.map((row: any) => {
          const sales = row.TotalAmount ?? row.totalAmount ?? 0;
          const payout = sales * 0.62;
          return {
            date: row.Date ?? row.date,
            sales,
            payout,
            profit: sales - payout
          };
        });

        return {
          totalSalesToday,
          totalPayout,
          profitSummary,
          topRiskyNumbers,
          recentTickets,
          rounds,
          chart
        };
      })
    );
  }
}
