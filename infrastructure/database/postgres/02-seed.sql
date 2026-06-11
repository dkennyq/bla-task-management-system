-- PostgreSQL Seed Data
-- Inserts demo users for testing

-- Note: Password is "Password123!" for all demo users
-- Hashed with BCrypt (this will be replaced by actual API registration)

-- Insert demo admin user
-- Email: admin@taskmanagement.com
-- Password: Password123!
INSERT INTO users (id, username, email, password_hash, full_name, role, is_active, created_at, updated_at)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    'admin',
    'admin@taskmanagement.com',
    '$2a$11$INSWXYefmENyhBk.oz29z.nrreffFwmNhRTIle/JdVsy0lUI.cJwK', -- Password123!
    'System Administrator',
    'Manager',
    TRUE,
    NOW(),
    NOW()
) ON CONFLICT (id) DO UPDATE SET role = 'Manager';

-- Insert demo regular user
-- Email: john.doe@example.com
-- Password: Password123!
INSERT INTO users (id, username, email, password_hash, full_name, role, is_active, created_at, updated_at)
VALUES (
    '00000000-0000-0000-0000-000000000002',
    'johndoe',
    'john.doe@example.com',
    '$2a$11$INSWXYefmENyhBk.oz29z.nrreffFwmNhRTIle/JdVsy0lUI.cJwK', -- Password123!
    'John Doe',
    'Operator',
    TRUE,
    NOW(),
    NOW()
) ON CONFLICT (id) DO NOTHING;

-- Insert demo regular user
-- Email: jane.smith@example.com
-- Password: Password123!
INSERT INTO users (id, username, email, password_hash, full_name, role, is_active, created_at, updated_at)
VALUES (
    '00000000-0000-0000-0000-000000000003',
    'janesmith',
    'jane.smith@example.com',
    '$2a$11$INSWXYefmENyhBk.oz29z.nrreffFwmNhRTIle/JdVsy0lUI.cJwK', -- Password123!
    'Jane Smith',
    'Operator',
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
