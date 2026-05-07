export interface ReportFilter {
  fromUtc: string;
  toUtc: string;
  keyword?: string;
  page: number;
  pageSize: number;
}

export interface PagedResult<T> {
  Items: T[];
  Page: number;
  PageSize: number;
  TotalCount: number;
}

export interface DailySalesRow { Date: string; TicketCount: number; SalesAmount: number; }
export interface NumberSummaryRow { Number: string; Frequency: number; TotalAmount: number; }
export interface RiskNumberRow { Number: string; Frequency: number; TotalAmount: number; RiskScore: number; }
export interface CustomerHistoryRow { CustomerId: string; CustomerName: string; TicketCount: number; TotalAmount: number; LastTicketAtUtc: string; }
export interface ProfitLossRow { Date: string; SalesAmount: number; PayoutAmount: number; ProfitAmount: number; }
