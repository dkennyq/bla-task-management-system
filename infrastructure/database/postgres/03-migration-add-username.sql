-- Migration: Add username column to users table
-- Run this for existing databases that don't have the username column yet

ALTER TABLE users ADD COLUMN IF NOT EXISTS username VARCHAR(50) UNIQUE;

-- Backfill usernames for existing users based on email prefix
UPDATE users SET username = SPLIT_PART(email, '@', 1) WHERE username IS NULL;

-- Make username NOT NULL after backfill
ALTER TABLE users ALTER COLUMN username SET NOT NULL;

-- Create index on username
CREATE INDEX IF NOT EXISTS idx_users_username ON users(username);

-- Success message
DO $$
BEGIN
    RAISE NOTICE '✅ Username column added and backfilled successfully';
END $$;
