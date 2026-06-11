import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mountWithPlugins } from '../../utils/testUtils'
import { useAuthStore } from '../../stores/authStore'
import LoginView from '../LoginView.vue'
import { setActivePinia, createPinia } from 'pinia'

describe('LoginView', () => {
  beforeEach(() => {
    localStorage.clear()
    setActivePinia(createPinia())
  })

  it('should render login form with all fields', () => {
    const { wrapper } = mountWithPlugins(LoginView)
    expect(wrapper.find('#email').exists()).toBe(true)
    expect(wrapper.find('#password').exists()).toBe(true)
    expect(wrapper.find('button[type="submit"]').exists()).toBe(true)
  })

  it('should render heading and description', () => {
    const { wrapper } = mountWithPlugins(LoginView)
    expect(wrapper.text()).toContain('Welcome Back')
    expect(wrapper.text()).toContain('Sign in to manage your tasks')
  })

  it('should show validation errors when submitting empty form', async () => {
    const { wrapper } = mountWithPlugins(LoginView)
    await wrapper.find('form').trigger('submit.prevent')

    expect(wrapper.text()).toContain('Email is required')
    expect(wrapper.text()).toContain('Password is required')
  })

  it('should validate email format', async () => {
    const { wrapper } = mountWithPlugins(LoginView)
    const emailInput = wrapper.find('#email')
    await emailInput.setValue('not-an-email')
    await emailInput.trigger('blur')

    expect(wrapper.text()).toContain('Please enter a valid email address')
  })

  it('should validate email on blur', async () => {
    const { wrapper } = mountWithPlugins(LoginView)
    const emailInput = wrapper.find('#email')
    await emailInput.setValue('')
    await emailInput.trigger('blur')

    expect(wrapper.text()).toContain('Email is required')
  })

  it('should validate password length on blur', async () => {
    const { wrapper } = mountWithPlugins(LoginView)
    const passwordInput = wrapper.find('#password')
    await passwordInput.setValue('Ab1!')
    await passwordInput.trigger('blur')

    expect(wrapper.text()).toContain('Password must be at least 8 characters')
  })

  it('should show loading state during login', async () => {
    const { wrapper } = mountWithPlugins(LoginView)
    const store = useAuthStore()

    vi.spyOn(store, 'login').mockImplementation(() => {
      store.$patch({ loading: true })
      return new Promise(() => {})
    })

    await wrapper.find('#email').setValue('test@example.com')
    await wrapper.find('#password').setValue('Password123!')
    await wrapper.find('form').trigger('submit.prevent')

    const button = wrapper.find('button[type="submit"]')
    expect(button.attributes('disabled')).toBeDefined()
    expect(button.text()).toContain('Signing in...')
  })

  it('should display error from auth store', async () => {
    const { wrapper } = mountWithPlugins(LoginView)
    const store = useAuthStore()
    store.$patch({ error: 'Invalid email or password' })
    await wrapper.vm.$nextTick()

    expect(wrapper.text()).toContain('Invalid email or password')
  })

  it('should clear field error on input', async () => {
    const { wrapper } = mountWithPlugins(LoginView)
    const emailInput = wrapper.find('#email')

    await wrapper.find('form').trigger('submit.prevent')
    expect(wrapper.text()).toContain('Email is required')

    await emailInput.setValue('a')
    expect(wrapper.text()).not.toContain('Email is required')
  })

  it('should have remember me checkbox', () => {
    const { wrapper } = mountWithPlugins(LoginView)
    const checkbox = wrapper.find('input[type="checkbox"]')
    expect(checkbox.exists()).toBe(true)
    expect(wrapper.text()).toContain('Remember me')
  })

  it('should load remembered email from localStorage', () => {
    localStorage.setItem('rememberedEmail', 'saved@example.com')
    const { wrapper } = mountWithPlugins(LoginView)
    const emailInput = wrapper.find('#email').element as HTMLInputElement
    expect(emailInput.value).toBe('saved@example.com')
  })

  it('should have link to registration page', () => {
    const { wrapper } = mountWithPlugins(LoginView)
    expect(wrapper.text()).toContain("Don't have an account?")
    const link = wrapper.findComponent({ name: 'RouterLink' })
    expect(link.exists()).toBe(true)
    expect(link.props('to')).toBe('/register')
  })

  it('should redirect to /tasks on successful login', async () => {
    const { wrapper, router } = mountWithPlugins(LoginView)
    const store = useAuthStore()

    const mockUser = { id: '1', username: 'test', email: 'test@example.com', fullName: 'Test', createdAt: '2026-01-01' }
    vi.spyOn(store, 'login').mockResolvedValue({ token: 'jwt', user: mockUser })

    await wrapper.find('#email').setValue('test@example.com')
    await wrapper.find('#password').setValue('Password123!')
    await wrapper.find('form').trigger('submit.prevent')
    await new Promise(resolve => setTimeout(resolve, 10))

    expect(router.currentRoute.value.name).toBe('tasks')
  })

  it('should redirect to custom redirect query on success', async () => {
    const { wrapper, router } = mountWithPlugins(LoginView)
    const store = useAuthStore()

    await router.push('/login?redirect=/tasks/some-task')
    await wrapper.vm.$nextTick()

    const mockUser = { id: '1', username: 'test', email: 'test@example.com', fullName: 'Test', createdAt: '2026-01-01' }
    vi.spyOn(store, 'login').mockResolvedValue({ token: 'jwt', user: mockUser })

    await wrapper.find('#email').setValue('test@example.com')
    await wrapper.find('#password').setValue('Password123!')
    await wrapper.find('form').trigger('submit.prevent')
    await new Promise(resolve => setTimeout(resolve, 10))

    expect(router.currentRoute.value.fullPath).toBe('/tasks/some-task')
  })
})
