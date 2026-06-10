import { describe, it, expect, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '../authStore'

describe('Auth Store', () => {
  beforeEach(() => {
    // Create a new pinia instance for each test
    setActivePinia(createPinia())
    // Clear localStorage
    localStorage.clear()
  })

  it('should initialize with null user and no token', () => {
    const store = useAuthStore()
    expect(store.user).toBe(null)
    expect(store.isAuthenticated).toBe(false)
    expect(store.loading).toBe(false)
    expect(store.error).toBe(null)
  })

  it('should initialize auth from localStorage', () => {
    const mockUser = { id: '1', username: 'test', email: 'test@example.com', fullName: 'Test User', createdAt: '2026-01-01' }
    localStorage.setItem('token', 'test-token')
    localStorage.setItem('user', JSON.stringify(mockUser))
    
    const store = useAuthStore()
    store.initializeAuth()
    
    expect(store.token).toBe('test-token')
    expect(store.user).toEqual(mockUser)
    expect(store.isAuthenticated).toBe(true)
  })

  it('should logout and clear storage', () => {
    const store = useAuthStore()
    localStorage.setItem('token', 'test-token')
    localStorage.setItem('user', JSON.stringify({ id: '1', username: 'test' }))
    
    store.logout()
    
    expect(store.user).toBe(null)
    expect(store.token).toBe(null)
    expect(localStorage.getItem('token')).toBe(null)
    expect(localStorage.getItem('user')).toBe(null)
  })
})
