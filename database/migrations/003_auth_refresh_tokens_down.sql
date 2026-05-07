DROP INDEX IF EXISTS idx_users_refresh_token_expires;
ALTER TABLE users DROP COLUMN IF EXISTS refresh_token_expires_at_utc;
ALTER TABLE users DROP COLUMN IF EXISTS refresh_token_hash;
