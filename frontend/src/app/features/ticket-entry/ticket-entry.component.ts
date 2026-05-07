import { CommonModule } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { TicketEntryApiService } from './ticket-entry-api.service';
import { TicketEntryParserService } from './ticket-entry-parser.service';
import { TicketEntryState } from './ticket-entry.state';

@Component({
  standalone: true,
  selector: 'app-ticket-entry',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  providers: [TicketEntryState],
  templateUrl: './ticket-entry.component.html',
  styleUrl: './ticket-entry.component.scss'
})
export class TicketEntryComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly api = inject(TicketEntryApiService);
  private readonly parser = inject(TicketEntryParserService);
  private readonly snackBar = inject(MatSnackBar);

  readonly state = inject(TicketEntryState);

  @ViewChild('rawInput') rawInput?: ElementRef<HTMLTextAreaElement>;

  readonly form = this.fb.group({
    customerId: ['', Validators.required],
    lotteryRoundId: ['', Validators.required],
    quickInput: ['', Validators.required]
  });

  ngOnInit(): void {
    this.loadLookupData();
  }

  loadLookupData(): void {
    this.state.loading.set(true);
    this.state.loadError.set(null);

    forkJoin({ customers: this.api.getCustomers(), rounds: this.api.getRounds() }).subscribe({
      next: ({ customers, rounds }) => {
        this.state.setLookupData(customers, rounds);
        this.state.loading.set(false);
      },
      error: () => {
        this.state.loading.set(false);
        this.state.loadError.set('Unable to load customers or rounds. Please retry.');
      }
    });
  }

  parseNow(): void {
    const rawText = this.form.controls.quickInput.value?.trim() ?? '';
    const result = this.parser.parse(rawText);
    this.state.items.set(result.items);
    this.state.parseErrors.set(result.errors);
  }

  onKeydown(event: KeyboardEvent): void {
    if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'enter') {
      event.preventDefault();
      this.saveTicket();
      return;
    }

    if (event.key === 'Tab') {
      return;
    }

    queueMicrotask(() => this.parseNow());
  }

  clearAll(): void {
    this.form.controls.quickInput.setValue('');
    this.state.items.set([]);
    this.state.parseErrors.set([]);
    this.rawInput?.nativeElement.focus();
  }

  saveTicket(): void {
    this.form.markAllAsTouched();
    this.parseNow();

    if (this.form.invalid || this.state.items().length === 0 || this.state.parseErrors().length > 0) {
      this.snackBar.open('Please fix validation errors before saving.', 'Close', { duration: 2500 });
      return;
    }

    this.state.saving.set(true);
    this.state.saveError.set(null);
    this.state.saveSuccess.set(null);

    this.api.saveTicket({
      customerId: this.form.controls.customerId.value!,
      lotteryRoundId: this.form.controls.lotteryRoundId.value!,
      items: this.state.items()
    }).subscribe({
      next: () => {
        this.state.saving.set(false);
        this.state.saveSuccess.set('Ticket saved successfully.');
        this.form.controls.quickInput.setValue('');
        this.state.items.set([]);
        this.state.parseErrors.set([]);
        this.rawInput?.nativeElement.focus();
      },
      error: () => {
        this.state.saving.set(false);
        this.state.saveError.set('Save failed. Check your network and try again.');
      }
    });
  }
}
