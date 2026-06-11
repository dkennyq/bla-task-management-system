import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, VueWrapper } from '@vue/test-utils'
import { createRouter, createWebHistory, Router } from 'vue-router'
import { createPinia, setActivePinia } from 'pinia'
import { useTasksStore } from '../../stores/tasksStore'
import { useAuthStore } from '../../stores/authStore'
import * as api from '../../services/api'
import TasksView from '../TasksView.vue'
import type { Task, TaskStatus, TaskPriority } from '../../types/task'
import type { Pinia } from 'pinia'

function createMockTask(overrides: Partial<Task> = {}): Task {
  return {
    id: '1',
    userId: 'user1',
    title: 'Test Task',
    description: 'Test description',
    status: 'Pending' as TaskStatus,
    priority: 'Medium' as TaskPriority,
    createdAt: '2026-01-15T10:00:00Z',
    updatedAt: '2026-01-15T10:00:00Z',
    ...overrides,
  }
}

interface MountResult {
  wrapper: VueWrapper
  router: Router
  pinia: Pinia
}

function mountAuthView(): MountResult {
  const pinia = createPinia()
  setActivePinia(pinia)

  const auth = useAuthStore()
  auth.$patch({
    user: { id: 'user1', username: 'testuser', email: 'test@example.com', fullName: 'Test User', createdAt: '2026-01-01' },
    token: 'fake-jwt',
  })

  const router = createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', name: 'home', component: { template: '<div>Home</div>' } },
      { path: '/login', name: 'login', component: { template: '<div>Login</div>' } },
      { path: '/tasks', name: 'tasks', component: { template: '<div>Tasks</div>' } },
    ],
  })

  vi.spyOn(api, 'getTasks').mockResolvedValue([])

  const wrapper = mount(TasksView, {
    global: {
      plugins: [pinia, router],
      stubs: { RouterView: true },
    },
  })

  return { wrapper, router, pinia }
}

function mountUnauthenticatedView(): MountResult {
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

  vi.spyOn(api, 'getTasks').mockResolvedValue([])

  const wrapper = mount(TasksView, {
    global: {
      plugins: [pinia, router],
      stubs: { RouterView: true },
    },
  })

  return { wrapper, router, pinia }
}

async function flushAll(): Promise<void> {
  await new Promise(resolve => setTimeout(resolve, 0))
}

describe('TasksView', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.restoreAllMocks()
  })

  it('should render header and create button link', async () => {
    const { wrapper } = mountAuthView()
    await flushAll()
    expect(wrapper.text()).toContain('Tasks')
    const links = wrapper.findAllComponents({ name: 'RouterLink' })
    const createLink = links.find(l => l.props('to') === '/tasks/new')
    expect(createLink).toBeDefined()
  })

  it('should render task cards for each task', async () => {
    const { wrapper } = mountAuthView()
    await flushAll()
    const store = useTasksStore()
    store.tasks = [
      createMockTask({ id: '1', title: 'Task One' }),
      createMockTask({ id: '2', title: 'Task Two', status: 'InProgress' as TaskStatus }),
      createMockTask({ id: '3', title: 'Task Three', status: 'Completed' as TaskStatus }),
    ]
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('Task One')
    expect(wrapper.text()).toContain('Task Two')
    expect(wrapper.text()).toContain('Task Three')
  })

  it('should show loading spinner when loading', async () => {
    const { wrapper } = mountAuthView()
    const store = useTasksStore()
    store.loading = true
    store.tasks = [createMockTask()]
    await wrapper.vm.$nextTick()
    expect(wrapper.find('.animate-spin').exists()).toBe(true)
  })

  it('should show error state with retry button', async () => {
    const { wrapper } = mountAuthView()
    await flushAll()
    const store = useTasksStore()
    store.error = 'Failed to fetch tasks'
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('Failed to fetch tasks')
    const retryBtn = wrapper.find('[role="alert"] button')
    expect(retryBtn.exists()).toBe(true)
    expect(retryBtn.text()).toContain('Try Again')
  })

  it('should show empty state when no tasks exist', async () => {
    const { wrapper } = mountAuthView()
    await flushAll()
    expect(wrapper.text()).toContain('There are no records.')
  })

  it('should show filter empty message when filters have no results', async () => {
    const { wrapper } = mountAuthView()
    await flushAll()
    const store = useTasksStore()
    store.tasks = [createMockTask()]
    store.filter = 'Completed'
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('No tasks match your current filters.')
  })

  it('should filter tasks by status when pill is clicked', async () => {
    const { wrapper } = mountAuthView()
    await flushAll()
    const store = useTasksStore()
    store.tasks = [
      createMockTask({ id: '1', title: 'Pending Task' }),
      createMockTask({ id: '2', title: 'Completed Task', status: 'Completed' as TaskStatus }),
    ]
    await wrapper.vm.$nextTick()

    const completedPill = wrapper.findAll('button').find(b => b.text().includes('Completed'))
    expect(completedPill).toBeDefined()
    await completedPill!.trigger('click')
    await wrapper.vm.$nextTick()

    expect(store.filter).toBe('Completed')
    expect(wrapper.text()).not.toContain('Pending Task')
  })

  it('should update search query on input', async () => {
    const { wrapper } = mountAuthView()
    await flushAll()
    const store = useTasksStore()
    store.tasks = [createMockTask(), createMockTask({ id: '2', title: 'Other', description: 'Something else' })]
    await wrapper.vm.$nextTick()

    const searchInput = wrapper.find('input[type="search"]')
    await searchInput.setValue('Other')
    await wrapper.vm.$nextTick()

    expect(store.searchQuery).toBe('Other')
    expect(store.filteredTasks).toHaveLength(1)
  })

  it('should have sort dropdown with options', async () => {
    const { wrapper } = mountAuthView()
    await flushAll()
    const sortSelect = wrapper.find('select[aria-label="Sort by"]')
    expect(sortSelect.exists()).toBe(true)
    const options = sortSelect.findAll('option')
    expect(options.length).toBeGreaterThanOrEqual(4)
    expect(options[0].text()).toBe('Created Date')
  })

  it('should toggle sort order when button clicked', async () => {
    const { wrapper } = mountAuthView()
    await flushAll()
    const store = useTasksStore()

    const sortBtn = wrapper.find('button[aria-label*="Sort"]')
    expect(sortBtn.exists()).toBe(true)

    expect(store.sortOrder).toBe('desc')
    await sortBtn.trigger('click')
    await wrapper.vm.$nextTick()

    expect(store.sortOrder).toBe('asc')
  })

  it('should fetch tasks on mount when user is authenticated', async () => {
    vi.restoreAllMocks()
    vi.spyOn(api, 'getTasks').mockResolvedValue([createMockTask()])

    mountAuthView()
    await flushAll()

    expect(api.getTasks).toHaveBeenCalledOnce()
  })

  it('should fetch tasks on mount regardless of auth state', async () => {
    mountUnauthenticatedView()
    await flushAll()

    expect(api.getTasks).toHaveBeenCalledOnce()
  })
})
