import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, VueWrapper } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import * as api from '../../../services/api'
import TaskFormModal from '../TaskFormModal.vue'
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

describe('TaskFormModal', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.restoreAllMocks()
  })

  function mountModal(props: Record<string, unknown> = {}): VueWrapper {
    return mount(TaskFormModal, {
      props: {
        isOpen: true,
        mode: 'create',
        ...props,
      },
      attachTo: document.body,
    })
  }

  it('should render create modal with empty form', () => {
    const wrapper = mountModal()
    expect(wrapper.text()).toContain('Create New Task')
    const titleInput = wrapper.find('#task-title').element as HTMLInputElement
    expect(titleInput.value).toBe('')
  })

  it('should render edit modal with pre-filled form', async () => {
    const task = createMockTask({
      title: 'Existing Task',
      description: 'Existing description',
      priority: 'High' as TaskPriority,
      status: 'InProgress' as TaskStatus,
      dueDate: '2026-07-01T00:00:00Z',
    })
    const wrapper = mountModal({ mode: 'edit', taskData: task })
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('Edit Task')
    const titleInput = wrapper.find('#task-title').element as HTMLInputElement
    expect(titleInput.value).toBe('Existing Task')
    const prioritySelect = wrapper.find('#task-priority').element as HTMLSelectElement
    expect(prioritySelect.value).toBe('High')
    expect(wrapper.find('#task-status').exists()).toBe(true)
  })

  it('should show validation error for empty title', async () => {
    const wrapper = mountModal()
    await wrapper.find('form').trigger('submit.prevent')
    expect(wrapper.text()).toContain('Title is required')
  })

  it('should show validation error for title over 200 chars', async () => {
    const wrapper = mountModal()
    const titleInput = wrapper.find('#task-title')
    await titleInput.setValue('x'.repeat(201))
    await wrapper.find('form').trigger('submit.prevent')
    expect(wrapper.text()).toContain('Title must be 200 characters or less')
  })

  it('should show validation error for description over 1000 chars', async () => {
    const wrapper = mountModal()
    const titleInput = wrapper.find('#task-title')
    await titleInput.setValue('Valid Title')
    const descInput = wrapper.find('#task-description')
    await descInput.setValue('x'.repeat(1001))
    await wrapper.find('form').trigger('submit.prevent')
    expect(wrapper.text()).toContain('Description must be 1000 characters or less')
  })

  it('should show validation error for past due date', async () => {
    const wrapper = mountModal()
    const titleInput = wrapper.find('#task-title')
    await titleInput.setValue('Valid Title')
    const dateInput = wrapper.find('#task-duedate')
    await dateInput.setValue('2020-01-01')
    await wrapper.find('form').trigger('submit.prevent')
    expect(wrapper.text()).toContain('Due date must be today or later')
  })

  it('should submit create form successfully', async () => {
    vi.spyOn(api, 'createTask').mockResolvedValue(createMockTask())
    const wrapper = mountModal()
    const titleInput = wrapper.find('#task-title')
    await titleInput.setValue('New Task Title')
    await wrapper.find('form').trigger('submit.prevent')
    await new Promise(resolve => setTimeout(resolve, 0))
    expect(api.createTask).toHaveBeenCalledOnce()
    expect(wrapper.emitted('save')).toBeTruthy()
  })

  it('should submit edit form successfully', async () => {
    vi.spyOn(api, 'updateTask').mockResolvedValue(createMockTask())
    const task = createMockTask({ id: '5' })
    const wrapper = mountModal({ mode: 'edit', taskData: task })
    const titleInput = wrapper.find('#task-title')
    await titleInput.setValue('Updated Title')
    await wrapper.find('form').trigger('submit.prevent')
    await new Promise(resolve => setTimeout(resolve, 0))
    expect(api.updateTask).toHaveBeenCalledOnce()
    expect(wrapper.emitted('save')).toBeTruthy()
  })

  it('should show error message on submit failure', async () => {
    vi.spyOn(api, 'createTask').mockRejectedValue({ message: 'API Error' })
    const wrapper = mountModal()
    const titleInput = wrapper.find('#task-title')
    await titleInput.setValue('New Task')
    await wrapper.find('form').trigger('submit.prevent')
    await new Promise(resolve => setTimeout(resolve, 0))
    expect(wrapper.text()).toContain('API Error')
  })

  it('should close modal on cancel button', async () => {
    const wrapper = mountModal()
    const cancelBtn = wrapper.findAll('button').find(b => b.text() === 'Cancel')
    expect(cancelBtn).toBeDefined()
    await cancelBtn!.trigger('click')
    expect(wrapper.emitted('close')).toBeTruthy()
  })

  it('should close modal on X button', async () => {
    const wrapper = mountModal()
    const closeBtn = wrapper.find('[aria-label="Close modal"]')
    await closeBtn.trigger('click')
    expect(wrapper.emitted('close')).toBeTruthy()
  })

  it('should close modal on backdrop click', async () => {
    const wrapper = mountModal()
    const backdrop = wrapper.find('[data-testid="modal-backdrop"]')
    await backdrop.trigger('click')
    expect(wrapper.emitted('close')).toBeTruthy()
  })

  it('should close modal on ESC key', async () => {
    const wrapper = mountModal()
    document.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape' }))
    await wrapper.vm.$nextTick()
    expect(wrapper.emitted('close')).toBeTruthy()
  })

  it('should show loading state on save button while submitting', async () => {
    let resolvePromise!: (task: Task) => void
    const promise = new Promise<Task>(resolve => { resolvePromise = resolve })
    vi.spyOn(api, 'createTask').mockReturnValue(promise as unknown as Promise<Task>)

    const wrapper = mountModal()
    const titleInput = wrapper.find('#task-title')
    await titleInput.setValue('New Task')
    await wrapper.find('form').trigger('submit.prevent')

    expect(wrapper.find('.animate-spin').exists()).toBe(true)
    expect(wrapper.find('button[type="submit"]').attributes('disabled')).toBeDefined()

    resolvePromise(createMockTask())
    await new Promise(resolve => setTimeout(resolve, 0))
  })

  it('should not close modal while submitting', async () => {
    let resolvePromise!: (task: Task) => void
    const promise = new Promise<Task>(resolve => { resolvePromise = resolve })
    vi.spyOn(api, 'createTask').mockReturnValue(promise as unknown as Promise<Task>)

    const wrapper = mountModal()
    const titleInput = wrapper.find('#task-title')
    await titleInput.setValue('New Task')
    await wrapper.find('form').trigger('submit.prevent')

    const cancelBtn = wrapper.findAll('button').find(b => b.text() === 'Cancel')
    await cancelBtn!.trigger('click')

    expect(wrapper.emitted('close')).toBeFalsy()

    resolvePromise(createMockTask())
    await new Promise(resolve => setTimeout(resolve, 0))
  })

  it('should reset form when modal opens', async () => {
    const wrapper = mountModal()
    const titleInput = wrapper.find('#task-title')
    await titleInput.setValue('Some value')
    expect((titleInput.element as HTMLInputElement).value).toBe('Some value')

    await wrapper.setProps({ isOpen: false })
    await wrapper.setProps({ isOpen: true })
    await wrapper.vm.$nextTick()

    const titleAfterReset = wrapper.find('#task-title').element as HTMLInputElement
    expect(titleAfterReset.value).toBe('')
  })

  it('should not render when isOpen is false', () => {
    const wrapper = mount(TaskFormModal, {
      props: { isOpen: false, mode: 'create' },
    })
    expect(wrapper.find('[role="dialog"]').exists()).toBe(false)
  })

  it('should show status field only in edit mode', () => {
    const createWrapper = mountModal()
    expect(createWrapper.find('#task-status').exists()).toBe(false)

    const task = createMockTask()
    const editWrapper = mountModal({ mode: 'edit', taskData: task })
    expect(editWrapper.find('#task-status').exists()).toBe(true)
  })
})
