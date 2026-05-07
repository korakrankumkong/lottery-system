export interface CustomerOption {
  id: string;
  name: string;
  phoneNumber?: string | null;
}

export interface LotteryRoundOption {
  id: string;
  code: string;
  drawDate: string;
  isClosed: boolean;
}

export type TicketType =
  | 'two_digit'
  | 'three_digit'
  | 'three_digit_top'
  | 'two_digit_bottom'
  | 'running_top'
  | 'running_bottom'
  | 'tod'
  | 'unknown';

export interface TicketItemInput {
  number: string;
  amount: number;
  type: TicketType;
}

export interface TicketEntryPayload {
  customerId: string;
  lotteryRoundId: string;
  items: TicketItemInput[];
}

export interface ParseError {
  line: number;
  reason: string;
  value: string;
}

export interface ParseResult {
  items: TicketItemInput[];
  errors: ParseError[];
}
