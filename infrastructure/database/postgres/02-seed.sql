-- PostgreSQL Seed Data
-- Inserts demo users for testing

-- Note: Password is "Password123!" for all demo users
-- Hashed with BCrypt (this will be replaced by actual API registration)

-- Insert demo admin user
-- Email: admin@taskmanagement.com
-- Password: Password123!
INSERT INTO users (id, email, password_hash, full_name, is_active, created_at, updated_at)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    'admin@taskmanagement.com',
    '$2a$11$xQJ7wZRJW5vKZJxKhC2wGOYz7X2xJmVZ8aQqP5mRKlZxVZ7Y8X5Ja', -- Password123!
    'Admin User',
    TRUE,
    NOW(),
    NOW()
) ON CONFLICT (id) DO NOTHING;

-- Insert demo regular user
-- Email: john.doe@example.com
-- Password: Password123!
INSERT INTO users (id, email, password_hash, full_name, is_active, created_at, updated_at)
VALUES (
    '00000000-0000-0000-0000-000000000002',
    'john.doe@example.com',
    '$2a$11$xQJ7wZRJW5vKZJxKhC2wGOYz7X2xJmVZ8aQqP5mRKlZxVZ7Y8X5Ja', -- Password123!
    'John Doe',
    TRUE,
    NOW(),
    NOW()
) ON CONFLICT (id) DO NOTHING;

-- Insert demo regular user
-- Email: jane.smith@example.com
-- Password: Password123!
INSERT INTO users (id, email, password_hash, full_name, is_active, created_at, updated_at)
VALUES (
    '00000000-0000-0000-0000-000000000003',
    'jane.smith@example.com',
    '$2a$11$xQJ7wZRJW5vKZJxKhC2wGOYz7X2xJmVZ8aQqP5mRKlZxVZ7Y8X5Ja', -- Password123!
    'Jane Smith',
    TRUE,
    NOW(),
    NOW()
) ON CONFLICT (id) DO NOTHING;

-- Success message
DO $$
BEGIN
    RAISE NOTICE '✅ Seed data inserted successfully';
    RAISE NOTICE '📝 Demo credentials:';
    RAISE NOTICE '   - admin@taskmanagement.com / Password123!';
    RAISE NOTICE '   - john.doe@example.com / Password123!';
    RAISE NOTICE '   - jane.smith@example.com / Password123!';
END $$;
