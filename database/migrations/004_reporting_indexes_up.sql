CREATE INDEX IF NOT EXISTS idx_tickets_created_customer ON tickets(created_at, customer_id) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS idx_ticket_items_number_amount ON ticket_items(number, amount) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS idx_tickets_round_total ON tickets(lottery_round_id, total_amount) WHERE is_deleted = FALSE;
