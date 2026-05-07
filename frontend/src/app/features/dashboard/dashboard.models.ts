export interface DashboardMetric {
  label: string;
  value: number;
  currency?: boolean;
  trend?: string;
}

export interface RiskyNumber {
  number: string;
  frequency: number;
  totalAmount: number;
}

export interface RecentTicket {
  id: string;
  customerId: string;
  lotteryRoundId: string;
  totalAmount: number;
  createdAtUtc: string;
}

export interface RoundStatus {
  id: string;
  code: string;
  drawDate: string;
  isClosed: boolean;
}

export interface SalesPoint {
  date: string;
  sales: number;
  payout: number;
  profit: number;
}

export interface DashboardViewModel {
  totalSalesToday: number;
  totalPayout: number;
  profitSummary: number;
  topRiskyNumbers: RiskyNumber[];
  recentTickets: RecentTicket[];
  rounds: RoundStatus[];
  chart: SalesPoint[];
}
