import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CustomerOption, LotteryRoundOption, TicketEntryPayload } from './ticket-entry.models';

@Injectable({ providedIn: 'root' })
export class TicketEntryApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getCustomers() {
    return this.http.get<CustomerOption[]>(`${this.baseUrl}/customers`);
  }

  getRounds() {
    return this.http.get<LotteryRoundOption[]>(`${this.baseUrl}/lotteryrounds`);
  }

  saveTicket(payload: TicketEntryPayload) {
    return this.http.post(`${this.baseUrl}/tickets`, payload);
  }
}
