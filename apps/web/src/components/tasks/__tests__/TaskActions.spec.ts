import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, VueWrapper } from '@vue/test-utils'
import TaskActions from '../TaskActions.vue'
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

describe('TaskActions', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
  })

  function mountActions(props: Record<string, unknown> = {}): VueWrapper {
    return mount(TaskActions, {
      props: {
        task: createMockTask(),
        ...props,
      },
    })
  }

  it('should render status dropdown with current value', () => {
    const wrapper = mountActions({ task: createMockTask({ status: 'InProgress' as TaskStatus }) })
    const select = wrapper.find('select[aria-label="Change task status"]').element as HTMLSelectElement
    expect(select.value).toBe('InProgress')
  })

  it('should render edit and delete buttons', () => {
    const wrapper = mountActions()
    expect(wrapper.find('[aria-label="Edit task"]').exists()).toBe(true)
    expect(wrapper.find('[aria-label="Delete task"]').exists()).toBe(true)
  })

  it('should emit edit when edit button clicked', async () => {
    const task = createMockTask({ id: '5' })
    const wrapper = mountActions({ task })
    await wrapper.find('[aria-label="Edit task"]').trigger('click')
    expect(wrapper.emitted('edit')).toBeTruthy()
    expect(wrapper.emitted('edit')![0]).toEqual([task])
  })

  it('should emit delete when delete button clicked', async () => {
    const task = createMockTask({ id: '5' })
    const wrapper = mountActions({ task })
    await wrapper.find('[aria-label="Delete task"]').trigger('click')
    expect(wrapper.emitted('delete')).toBeTruthy()
    expect(wrapper.emitted('delete')![0]).toEqual([task])
  })

  it('should emit statusChange when dropdown changes', async () => {
    const task = createMockTask()
    const wrapper = mountActions({ task })
    const select = wrapper.find('select[aria-label="Change task status"]')
    await select.setValue('Completed')
    await wrapper.vm.$nextTick()
    expect(wrapper.emitted('statusChange')).toBeTruthy()
    expect(wrapper.emitted('statusChange')![0]).toEqual([task, 'Completed'])
  })

  it('should disable buttons during loading prop', () => {
    const wrapper = mountActions({ loading: true })
    expect(wrapper.find('[aria-label="Edit task"]').attributes('disabled')).toBeDefined()
    expect(wrapper.find('[aria-label="Delete task"]').attributes('disabled')).toBeDefined()
    expect(wrapper.find('select[aria-label="Change task status"]').attributes('disabled')).toBeDefined()
  })

  it('should not emit when loading is true', async () => {
    const wrapper = mountActions({ loading: true })
    await wrapper.find('[aria-label="Edit task"]').trigger('click')
    expect(wrapper.emitted('edit')).toBeFalsy()
  })

  it('should handle all status dropdown values', () => {
    const statuses: TaskStatus[] = ['Pending', 'InProgress', 'Completed']
    for (const status of statuses) {
      const wrapper = mountActions({ task: createMockTask({ status }) })
      const select = wrapper.find('select[aria-label="Change task status"]').element as HTMLSelectElement
      expect(select.value).toBe(status)
    }
  })
})
