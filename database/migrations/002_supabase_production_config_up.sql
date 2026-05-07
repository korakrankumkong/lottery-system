-- Supabase/PostgreSQL production hardening script
-- Run after base schema migration in production environments.

CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- Example maintenance-focused indexes for ticket analytics workloads.
CREATE INDEX IF NOT EXISTS idx_tickets_created_at_desc ON tickets(created_at DESC) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS idx_ticket_items_number_ticket ON ticket_items(number, ticket_id) WHERE is_deleted = FALSE;

-- Optional: if using pg_stat_statements in your managed tier, enable it there.
-- CREATE EXTENSION IF NOT EXISTS pg_stat_statements;
