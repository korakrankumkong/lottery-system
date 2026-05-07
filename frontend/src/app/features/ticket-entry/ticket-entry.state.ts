import { Injectable, computed, signal } from '@angular/core';
import { CustomerOption, LotteryRoundOption, ParseError, TicketItemInput } from './ticket-entry.models';

@Injectable()
export class TicketEntryState {
  readonly customers = signal<CustomerOption[]>([]);
  readonly rounds = signal<LotteryRoundOption[]>([]);
  readonly items = signal<TicketItemInput[]>([]);
  readonly parseErrors = signal<ParseError[]>([]);

  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly loadError = signal<string | null>(null);
  readonly saveError = signal<string | null>(null);
  readonly saveSuccess = signal<string | null>(null);

  readonly total = computed(() => this.items().reduce((sum, item) => sum + item.amount, 0));
  readonly itemCount = computed(() => this.items().length);

  setLookupData(customers: CustomerOption[], rounds: LotteryRoundOption[]) {
    this.customers.set(customers);
    this.rounds.set(rounds.filter(x => !x.isClosed));
  }
}
