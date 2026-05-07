CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TABLE users (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  username VARCHAR(100) NOT NULL UNIQUE,
  password_hash TEXT NOT NULL,
  full_name VARCHAR(200) NOT NULL,
  role INT NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NULL,
  deleted_at TIMESTAMPTZ NULL,
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE customers (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  name VARCHAR(200) NOT NULL,
  phone_number VARCHAR(30),
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NULL,
  deleted_at TIMESTAMPTZ NULL,
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE lottery_rounds (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  code VARCHAR(100) NOT NULL UNIQUE,
  draw_date DATE NOT NULL,
  is_closed BOOLEAN NOT NULL DEFAULT FALSE,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NULL,
  deleted_at TIMESTAMPTZ NULL,
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE tickets (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  customer_id UUID NOT NULL REFERENCES customers(id),
  lottery_round_id UUID NOT NULL REFERENCES lottery_rounds(id),
  total_amount NUMERIC(12,2) NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NULL,
  deleted_at TIMESTAMPTZ NULL,
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE ticket_items (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  ticket_id UUID NOT NULL REFERENCES tickets(id) ON DELETE CASCADE,
  number VARCHAR(10) NOT NULL,
  amount NUMERIC(12,2) NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NULL,
  deleted_at TIMESTAMPTZ NULL,
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE lottery_results (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  lottery_round_id UUID NOT NULL REFERENCES lottery_rounds(id),
  prize_type VARCHAR(100) NOT NULL,
  winning_number VARCHAR(10) NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NULL,
  deleted_at TIMESTAMPTZ NULL,
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE payment_transactions (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  ticket_id UUID NOT NULL REFERENCES tickets(id),
  amount NUMERIC(12,2) NOT NULL,
  method VARCHAR(50) NOT NULL,
  status VARCHAR(30) NOT NULL,
  reference_no VARCHAR(120),
  paid_at TIMESTAMPTZ NOT NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NULL,
  deleted_at TIMESTAMPTZ NULL,
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE audit_logs (
  id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id UUID NULL REFERENCES users(id),
  action VARCHAR(120) NOT NULL,
  entity_name VARCHAR(120) NOT NULL,
  entity_id VARCHAR(120) NOT NULL,
  changes_json JSONB NULL,
  ip_address VARCHAR(64) NULL,
  created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
  updated_at TIMESTAMPTZ NULL,
  deleted_at TIMESTAMPTZ NULL,
  is_deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE INDEX idx_customers_phone_active ON customers(phone_number) WHERE is_deleted = FALSE;
CREATE INDEX idx_rounds_drawdate_active ON lottery_rounds(draw_date) WHERE is_deleted = FALSE;
CREATE INDEX idx_tickets_customer_created_active ON tickets(customer_id, created_at DESC) WHERE is_deleted = FALSE;
CREATE INDEX idx_tickets_round_created_active ON tickets(lottery_round_id, created_at DESC) WHERE is_deleted = FALSE;
CREATE INDEX idx_ticket_items_number_active ON ticket_items(number) WHERE is_deleted = FALSE;
CREATE INDEX idx_ticket_items_ticket_active ON ticket_items(ticket_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_results_round_number_active ON lottery_results(lottery_round_id, winning_number) WHERE is_deleted = FALSE;
CREATE INDEX idx_payments_status_paidat_active ON payment_transactions(status, paid_at DESC) WHERE is_deleted = FALSE;
CREATE INDEX idx_payments_ticket_active ON payment_transactions(ticket_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_audit_entity_created_active ON audit_logs(entity_name, created_at DESC) WHERE is_deleted = FALSE;
CREATE INDEX idx_audit_user_created_active ON audit_logs(user_id, created_at DESC) WHERE is_deleted = FALSE;
