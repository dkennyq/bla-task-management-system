import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { mount, VueWrapper } from '@vue/test-utils'
import { createRouter, createWebHistory, Router } from 'vue-router'
import { createPinia, setActivePinia } from 'pinia'
import { useAuthStore } from '../../stores/authStore'
import * as api from '../../services/api'
import RegisterView from '../RegisterView.vue'
import type { RegisterRequest, RegisterResponse } from '../../types/user'
import type { Pinia } from 'pinia'

function createMockResponse(overrides: Partial<RegisterResponse> = {}): RegisterResponse {
  return {
    id: 'new-user-id',
    username: 'johndoe',
    email: 'john.doe@example.com',
    fullName: 'John Doe',
    createdAt: '2026-06-10T12:00:00Z',
    ...overrides,
  }
}

interface MountResult {
  wrapper: VueWrapper
  router: Router
  pinia: Pinia
}

function mountView(): MountResult {
  const pinia = createPinia()
  setActivePinia(pinia)

  const router = createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', name: 'home', component: { template: '<div>Home</div>' } },
      { path: '/login', name: 'login', component: { template: '<div>Login</div>' } },
      { path: '/register', name: 'register', component: { template: '<div>Register</div>' } },
    ],
  })

  const wrapper = mount(RegisterView, {
    global: {
      plugins: [pinia, router],
    },
  })

  return { wrapper, router, pinia }
}

async function flushAll(): Promise<void> {
  await new Promise(resolve => setTimeout(resolve, 0))
}

describe('RegisterView', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.restoreAllMocks()
  })

  afterEach(() => {
    vi.useRealTimers()
  })

  it('should render registration form', () => {
    const { wrapper } = mountView()
    expect(wrapper.text()).toContain('Create Account')
    expect(wrapper.find('#username').exists()).toBe(true)
    expect(wrapper.find('#email').exists()).toBe(true)
    expect(wrapper.find('#password').exists()).toBe(true)
    expect(wrapper.find('#confirmPassword').exists()).toBe(true)
    expect(wrapper.find('#fullName').exists()).toBe(true)
    expect(wrapper.find('input[type="checkbox"]').exists()).toBe(true)
  })

  it('should show validation error for empty username on blur', async () => {
    const { wrapper } = mountView()
    const usernameInput = wrapper.find('#username')
    await usernameInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('Username is required')
  })

  it('should show validation error for short username', async () => {
    const { wrapper } = mountView()
    const usernameInput = wrapper.find('#username')
    await usernameInput.setValue('ab')
    await usernameInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('at least 3 characters')
  })

  it('should show validation error for username with special chars', async () => {
    const { wrapper } = mountView()
    const usernameInput = wrapper.find('#username')
    await usernameInput.setValue('user name!')
    await usernameInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('letters, numbers, underscores, and email characters')
  })

  it('should show validation error for invalid email', async () => {
    const { wrapper } = mountView()
    const emailInput = wrapper.find('#email')
    await emailInput.setValue('notanemail')
    await emailInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('valid email address')
  })

  it('should show validation error for weak password', async () => {
    const { wrapper } = mountView()
    const passwordInput = wrapper.find('#password')
    await passwordInput.setValue('short')
    await passwordInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('at least 8 characters')
  })

  it('should show validation error for password without uppercase', async () => {
    const { wrapper } = mountView()
    const passwordInput = wrapper.find('#password')
    await passwordInput.setValue('lowercase123!')
    await passwordInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('uppercase letter')
  })

  it('should show validation error for password without number', async () => {
    const { wrapper } = mountView()
    const passwordInput = wrapper.find('#password')
    await passwordInput.setValue('UppercaseLower!')
    await passwordInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('at least one number')
  })

  it('should show validation error for password without special char', async () => {
    const { wrapper } = mountView()
    const passwordInput = wrapper.find('#password')
    await passwordInput.setValue('UppercaseLower123')
    await passwordInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('special character')
  })

  it('should show validation error when passwords do not match', async () => {
    const { wrapper } = mountView()
    const passwordInput = wrapper.find('#password')
    await passwordInput.setValue('ValidPass123!')
    const confirmInput = wrapper.find('#confirmPassword')
    await confirmInput.setValue('DifferentPass123!')
    await confirmInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('Passwords do not match')
  })

  it('should show validation error for empty full name', async () => {
    const { wrapper } = mountView()
    const nameInput = wrapper.find('#fullName')
    await nameInput.trigger('blur')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('Full name is required')
  })

  it('should show validation error when terms not accepted on submit', async () => {
    const { wrapper } = mountView()
    const usernameInput = wrapper.find('#username')
    await usernameInput.setValue('validuser')
    const emailInput = wrapper.find('#email')
    await emailInput.setValue('valid@example.com')
    const passwordInput = wrapper.find('#password')
    await passwordInput.setValue('ValidPass123!')
    const confirmInput = wrapper.find('#confirmPassword')
    await confirmInput.setValue('ValidPass123!')
    const nameInput = wrapper.find('#fullName')
    await nameInput.setValue('Valid Name')

    await wrapper.find('form').trigger('submit.prevent')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('accept the terms')
  })

  it('should display password strength indicator', async () => {
    const { wrapper } = mountView()
    const passwordInput = wrapper.find('#password')

    await passwordInput.setValue('weak')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('Weak')

    await passwordInput.setValue('StrongPass123!')
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('Strong')
  })

  it('should toggle password visibility', async () => {
    const { wrapper } = mountView()
    const passwordInput = wrapper.find('#password')
    expect(passwordInput.attributes('type')).toBe('password')

    const toggleBtn = wrapper.findAll('button').find(b => b.attributes('aria-label') === 'Show password')
    await toggleBtn!.trigger('click')
    await wrapper.vm.$nextTick()

    expect(passwordInput.attributes('type')).toBe('text')
  })

  it('should call register API on valid form submission', async () => {
    vi.spyOn(api, 'register').mockResolvedValue(createMockResponse())
    const { wrapper } = mountView()
    await flushAll()

    await wrapper.find('#username').setValue('newuser')
    await wrapper.find('#email').setValue('new@example.com')
    await wrapper.find('#password').setValue('ValidPass123!')
    await wrapper.find('#confirmPassword').setValue('ValidPass123!')
    await wrapper.find('#fullName').setValue('New User')
    const checkbox = wrapper.find('input[type="checkbox"]')
    await checkbox.setValue(true)

    await wrapper.find('form').trigger('submit.prevent')
    await flushAll()

    expect(api.register).toHaveBeenCalledOnce()
    expect(api.register).toHaveBeenCalledWith({
      username: 'newuser',
      email: 'new@example.com',
      password: 'ValidPass123!',
      fullName: 'New User',
    })
  })

  it('should show success message after registration', async () => {
    vi.useFakeTimers()
    vi.spyOn(api, 'register').mockResolvedValue(createMockResponse())
    const { wrapper } = mountView()

    await wrapper.find('#username').setValue('newuser')
    await wrapper.find('#email').setValue('new@example.com')
    await wrapper.find('#password').setValue('ValidPass123!')
    await wrapper.find('#confirmPassword').setValue('ValidPass123!')
    await wrapper.find('#fullName').setValue('New User')
    await wrapper.find('input[type="checkbox"]').setValue(true)

    await wrapper.find('form').trigger('submit.prevent')
    await wrapper.vm.$nextTick()
    await wrapper.vm.$nextTick()
    await wrapper.vm.$nextTick()

    expect(wrapper.text()).toContain('Registration Successful!')
  })

  it('should redirect to login after 2 seconds on success', async () => {
    vi.useFakeTimers()
    vi.spyOn(api, 'register').mockResolvedValue(createMockResponse())
    const { wrapper, router } = mountView()
    const pushSpy = vi.spyOn(router, 'push')

    await wrapper.find('#username').setValue('newuser')
    await wrapper.find('#email').setValue('new@example.com')
    await wrapper.find('#password').setValue('ValidPass123!')
    await wrapper.find('#confirmPassword').setValue('ValidPass123!')
    await wrapper.find('#fullName').setValue('New User')
    await wrapper.find('input[type="checkbox"]').setValue(true)

    await wrapper.find('form').trigger('submit.prevent')
    await wrapper.vm.$nextTick()
    await wrapper.vm.$nextTick()
    await wrapper.vm.$nextTick()

    vi.advanceTimersByTime(2000)
    expect(pushSpy).toHaveBeenCalledWith('/login')
  })

  it('should show API error on registration failure', async () => {
    vi.spyOn(api, 'register').mockRejectedValue({
      message: 'Username already exists',
      statusCode: 409,
    })
    const { wrapper } = mountView()
    await flushAll()

    await wrapper.find('#username').setValue('existinguser')
    await wrapper.find('#email').setValue('existing@example.com')
    await wrapper.find('#password').setValue('ValidPass123!')
    await wrapper.find('#confirmPassword').setValue('ValidPass123!')
    await wrapper.find('#fullName').setValue('Existing User')
    await wrapper.find('input[type="checkbox"]').setValue(true)

    await wrapper.find('form').trigger('submit.prevent')
    await flushAll()

    expect(wrapper.text()).toContain('Username already exists')
  })

  it('should have link to login page', () => {
    const { wrapper } = mountView()
    const loginLink = wrapper.findAll('a').find(a => a.text().includes('Sign in'))
    expect(loginLink).toBeDefined()
    expect(loginLink!.attributes('href')).toBe('/login')
  })

  it('should have autofocus attribute on username field', () => {
    const { wrapper } = mountView()
    const usernameInput = wrapper.find('#username')
    expect(usernameInput.attributes('autofocus')).toBe('')
  })
})
