import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, VueWrapper } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import TaskCard from '../TaskCard.vue'
import type { Task, TaskStatus, TaskPriority } from '../../../types/task'

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

describe('TaskCard', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.restoreAllMocks()
  })

  function mountCard(props: Record<string, unknown> = {}): VueWrapper {
    return mount(TaskCard, {
      props: {
        task: createMockTask(),
        ...props,
      },
    })
  }

  it('should render task title and description', () => {
    const wrapper = mountCard({ task: createMockTask({ title: 'My Task', description: 'My description' }) })
    expect(wrapper.text()).toContain('My Task')
    expect(wrapper.text()).toContain('My description')
  })

  it('should render priority badge', () => {
    const wrapper = mountCard({ task: createMockTask({ priority: 'High' as TaskPriority }) })
    expect(wrapper.text()).toContain('High')
  })

  it('should render status badge', () => {
    const wrapper = mountCard({ task: createMockTask({ status: 'InProgress' as TaskStatus }) })
    expect(wrapper.text()).toContain('In Progress')
  })

  it('should render due date when provided', () => {
    const wrapper = mountCard({ task: createMockTask({ dueDate: '2026-06-15T12:00:00Z' }) })
    expect(wrapper.text()).toContain('Jun 15, 2026')
  })

  it('should emit edit when card body clicked', async () => {
    const task = createMockTask({ id: '42' })
    const wrapper = mountCard({ task })
    await wrapper.find('[data-testid="task-card"]').trigger('click')
    expect(wrapper.emitted('edit')).toBeTruthy()
    expect(wrapper.emitted('edit')![0]).toEqual([task])
  })

  it('should re-emit delete from TaskActions', async () => {
    const task = createMockTask({ id: '42' })
    const wrapper = mountCard({ task })
    const deleteBtn = wrapper.find('[aria-label="Delete task"]')
    await deleteBtn.trigger('click')
    expect(wrapper.emitted('delete')).toBeTruthy()
    expect(wrapper.emitted('delete')![0]).toEqual([task])
  })

  it('should re-emit statusChange from TaskActions', async () => {
    const task = createMockTask({ id: '42' })
    const wrapper = mountCard({ task })
    const select = wrapper.find('select[aria-label="Change task status"]')
    await select.setValue('InProgress')
    await wrapper.vm.$nextTick()
    expect(wrapper.emitted('statusChange')).toBeTruthy()
    expect(wrapper.emitted('statusChange')![0]).toEqual([task, 'InProgress'])
  })

  it('should render without crashing for all status values', () => {
    const statuses: TaskStatus[] = ['Pending', 'InProgress', 'Completed']
    for (const status of statuses) {
      const wrapper = mountCard({ task: createMockTask({ status }) })
      expect(wrapper.find('[data-testid="task-card"]').exists()).toBe(true)
    }
  })
})
