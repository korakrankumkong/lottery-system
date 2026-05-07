import { TicketEntryParserService } from './ticket-entry-parser.service';

describe('TicketEntryParserService', () => {
  let service: TicketEntryParserService;

  beforeEach(() => {
    service = new TicketEntryParserService();
  });

  it('parses multiline standard input', () => {
    const result = service.parse('123=100\n456/50\n89:200');

    expect(result.errors.length).toBe(0);
    expect(result.items).toEqual([
      { number: '123', amount: 100, type: 'three_digit' },
      { number: '456', amount: 50, type: 'three_digit' },
      { number: '89', amount: 200, type: 'two_digit' }
    ]);
  });

  it('detects Thai lottery styles', () => {
    const top = '\u0E1A\u0E19';
    const bottom = '\u0E25\u0E48\u0E32\u0E07';
    const tod = '\u0E42\u0E15\u0E4A\u0E14';
    const runTop = '\u0E27\u0E34\u0E48\u0E07\u0E1A\u0E19';

    const result = service.parse(`123${top}=100\n89${bottom}=200\n456${tod}=50\n7${runTop}=20`);

    expect(result.errors.length).toBe(0);
    expect(result.items[0].type).toBe('three_digit_top');
    expect(result.items[1].type).toBe('two_digit_bottom');
    expect(result.items[2].type).toBe('tod');
    expect(result.items[3].type).toBe('running_top');
  });

  it('returns errors for invalid lines without dropping valid ones', () => {
    const result = service.parse('123=100\nxx=50\n89bottom=-1\nabc');

    expect(result.items.length).toBe(1);
    expect(result.errors.length).toBe(3);
    expect(result.errors[0].line).toBe(2);
    expect(result.errors[1].line).toBe(3);
    expect(result.errors[2].line).toBe(4);
  });

  it('handles large input efficiently', () => {
    const lines = new Array(5000).fill('123=10').join('\n');

    const started = performance.now();
    const result = service.parse(lines);
    const elapsed = performance.now() - started;

    expect(result.items.length).toBe(5000);
    expect(result.errors.length).toBe(0);
    expect(elapsed).toBeLessThan(250);
  });
});
