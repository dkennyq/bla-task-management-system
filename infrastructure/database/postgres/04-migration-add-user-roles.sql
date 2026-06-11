-- Migration: Add role column to users table for RBAC
-- Adds Manager and Operator roles

ALTER TABLE users
ADD COLUMN IF NOT EXISTS role VARCHAR(50) NOT NULL DEFAULT 'Operator';

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint
        WHERE conname = 'chk_user_role'
        AND conrelid = 'users'::regclass
    ) THEN
        ALTER TABLE users
        ADD CONSTRAINT chk_user_role
        CHECK (role IN ('Manager', 'Operator'));
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_users_role ON users(role);

UPDATE users
SET role = 'Manager'
WHERE email = 'admin@taskmanagement.com';

UPDATE users
SET role = 'Operator'
WHERE email != 'admin@taskmanagement.com' AND role IS NULL OR role = '';

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM users WHERE role = 'Manager') THEN
        RAISE EXCEPTION 'MIGRATION FAILED: No Manager user found. At least one Manager must exist.';
    END IF;
END $$;

DO $$
BEGIN
    RAISE NOTICE '✅ Role column added to users table';
END $$;
