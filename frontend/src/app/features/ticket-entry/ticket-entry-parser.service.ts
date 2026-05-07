import { Injectable } from '@angular/core';
import { ParseError, ParseResult, TicketItemInput, TicketType } from './ticket-entry.models';

@Injectable({ providedIn: 'root' })
export class TicketEntryParserService {
  private static readonly SEP_RE = /^(.+?)(?:=|\/|:)(.+)$/;
  private static readonly SPACE_RE = /\s+/g;
  private static readonly AMOUNT_RE = /^\d+(?:\.\d{1,2})?$/;
  private static readonly DIGITS_RE = /^\d{1,6}$/;

  private static readonly THAI_TOP = '\u0E1A\u0E19';
  private static readonly THAI_BOTTOM = '\u0E25\u0E48\u0E32\u0E07';
  private static readonly THAI_TOD = '\u0E42\u0E15\u0E4A\u0E14';
  private static readonly THAI_RUN_TOP = '\u0E27\u0E34\u0E48\u0E07\u0E1A\u0E19';
  private static readonly THAI_RUN_BOTTOM = '\u0E27\u0E34\u0E48\u0E07\u0E25\u0E48\u0E32\u0E07';

  private static readonly THAI_LABEL_RE = new RegExp(
    `(${TicketEntryParserService.THAI_TOP}|${TicketEntryParserService.THAI_BOTTOM}|${TicketEntryParserService.THAI_TOD}|${TicketEntryParserService.THAI_RUN_TOP}|${TicketEntryParserService.THAI_RUN_BOTTOM}|runtop|runbottom|top|bottom|tod)`,
    'gi'
  );

  private static readonly TYPE_PATTERNS: Array<{ re: RegExp; type: TicketType }> = [
    { re: new RegExp(`(${TicketEntryParserService.THAI_RUN_TOP}|runtop)`, 'i'), type: 'running_top' },
    { re: new RegExp(`(${TicketEntryParserService.THAI_RUN_BOTTOM}|runbottom)`, 'i'), type: 'running_bottom' },
    { re: new RegExp(`(${TicketEntryParserService.THAI_TOD}|tod)`, 'i'), type: 'tod' },
    { re: new RegExp(`(${TicketEntryParserService.THAI_BOTTOM}|bottom)`, 'i'), type: 'two_digit_bottom' },
    { re: new RegExp(`(${TicketEntryParserService.THAI_TOP}|top)`, 'i'), type: 'three_digit_top' }
  ];

  parse(rawText: string): ParseResult {
    if (!rawText.trim()) {
      return { items: [], errors: [] };
    }

    const lines = rawText.split(/\r?\n/);
    const items: TicketItemInput[] = [];
    const errors: ParseError[] = [];

    for (let i = 0; i < lines.length; i += 1) {
      const lineNo = i + 1;
      const line = lines[i].trim();
      if (!line) {
        continue;
      }

      const match = TicketEntryParserService.SEP_RE.exec(line);
      if (!match) {
        errors.push({ line: lineNo, reason: 'Missing separator (=, /, :)', value: line });
        continue;
      }

      const left = match[1].replace(TicketEntryParserService.SPACE_RE, '');
      const right = match[2].replace(TicketEntryParserService.SPACE_RE, '');

      if (!TicketEntryParserService.AMOUNT_RE.test(right)) {
        errors.push({ line: lineNo, reason: 'Invalid amount format', value: line });
        continue;
      }

      const amount = Number(right);
      if (!Number.isFinite(amount) || amount <= 0) {
        errors.push({ line: lineNo, reason: 'Amount must be greater than 0', value: line });
        continue;
      }

      const type = this.detectType(left);
      const number = this.extractDigits(left);

      if (!number || !TicketEntryParserService.DIGITS_RE.test(number)) {
        errors.push({ line: lineNo, reason: 'Number must be 1-6 digits (Thai labels allowed)', value: line });
        continue;
      }

      if (!this.validateNumberByType(number, type)) {
        errors.push({ line: lineNo, reason: `Invalid number length for type ${type}`, value: line });
        continue;
      }

      items.push({ number, amount, type });
    }

    return { items, errors };
  }

  private detectType(input: string): TicketType {
    for (const pattern of TicketEntryParserService.TYPE_PATTERNS) {
      if (pattern.re.test(input)) {
        return pattern.type;
      }
    }

    const digits = this.extractDigits(input);
    if (!digits) {
      return 'unknown';
    }

    if (digits.length === 2) {
      return 'two_digit';
    }

    if (digits.length === 3) {
      return 'three_digit';
    }

    return 'unknown';
  }

  private extractDigits(input: string): string {
    return input.replace(TicketEntryParserService.THAI_LABEL_RE, '').replace(/[^\d]/g, '');
  }

  private validateNumberByType(number: string, type: TicketType): boolean {
    switch (type) {
      case 'running_top':
      case 'running_bottom':
        return number.length === 1;
      case 'two_digit_bottom':
      case 'two_digit':
        return number.length === 2;
      case 'three_digit_top':
      case 'three_digit':
      case 'tod':
        return number.length === 3;
      case 'unknown':
      default:
        return number.length >= 1 && number.length <= 6;
    }
  }
}
