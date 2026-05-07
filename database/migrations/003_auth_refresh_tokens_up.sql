ALTER TABLE users ADD COLUMN IF NOT EXISTS refresh_token_hash TEXT NULL;
ALTER TABLE users ADD COLUMN IF NOT EXISTS refresh_token_expires_at_utc TIMESTAMPTZ NULL;
CREATE INDEX IF NOT EXISTS idx_users_refresh_token_expires ON users(refresh_token_expires_at_utc);
