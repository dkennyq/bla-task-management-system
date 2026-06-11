import { mount, VueWrapper } from '@vue/test-utils'
import { createRouter, createWebHistory, Router } from 'vue-router'
import { createPinia, Pinia, setActivePinia } from 'pinia'
import type { Component } from 'vue'

export interface MountOptions {
  props?: Record<string, unknown>
  global?: {
    plugins?: unknown[]
    stubs?: Record<string, unknown>
    mocks?: Record<string, unknown>
  }
}

export function mountWithPlugins(
  component: Component,
  options: MountOptions = {}
): { wrapper: VueWrapper; router: Router; pinia: Pinia } {
  const pinia = createPinia()
  setActivePinia(pinia)
  
  const router = createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', name: 'home', component: { template: '<div>Home</div>' } },
      { path: '/login', name: 'login', component: { template: '<div>Login</div>' } },
      { path: '/tasks', name: 'tasks', component: { template: '<div>Tasks</div>' } },
    ],
  })

  const wrapper = mount(component, {
    ...options,
    global: {
      plugins: [pinia, router, ...(options.global?.plugins || [])],
      stubs: {
        RouterLink: true,
        RouterView: true,
        ...options.global?.stubs,
      },
      ...options.global,
    },
  })

  return { wrapper, router, pinia }
}

export async function flushPromises() {
  return new Promise(resolve => setTimeout(resolve, 0))
}

export function createMockApiResponse<T>(data: T, success = true) {
  return {
    data,
    success,
    error: success ? undefined : { message: 'Test error', statusCode: 400 },
  }
}