import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, VueWrapper } from '@vue/test-utils'

import DeleteConfirmDialog from '../DeleteConfirmDialog.vue'

describe('DeleteConfirmDialog', () => {
  beforeEach(() => {
    vi.restoreAllMocks()
  })

  function mountDialog(props: Record<string, unknown> = {}): VueWrapper {
    return mount(DeleteConfirmDialog, {
      props: {
        isOpen: true,
        taskTitle: 'Test Task',
        ...props,
      },
      attachTo: document.body,
    })
  }

  it('should render when isOpen is true', () => {
    const wrapper = mountDialog()
    expect(wrapper.find('[role="dialog"]').exists()).toBe(true)
    expect(wrapper.text()).toContain('Delete Task')
  })

  it('should not render when isOpen is false', () => {
    const wrapper = mountDialog({ isOpen: false })
    expect(wrapper.find('[role="dialog"]').exists()).toBe(false)
  })

  it('should display task title in message', () => {
    const wrapper = mountDialog({ taskTitle: 'My Important Task' })
    expect(wrapper.text()).toContain('My Important Task')
  })

  it('should display warning about irreversible action', () => {
    const wrapper = mountDialog()
    expect(wrapper.text()).toContain('cannot be undone')
  })

  it('should emit confirm when Delete button clicked', async () => {
    const wrapper = mountDialog()
    const deleteBtn = wrapper.findAll('button').find(b => b.text() === 'Delete')
    expect(deleteBtn).toBeDefined()
    await deleteBtn!.trigger('click')
    expect(wrapper.emitted('confirm')).toBeTruthy()
  })

  it('should emit cancel when Cancel button clicked', async () => {
    const wrapper = mountDialog()
    const cancelBtn = wrapper.findAll('button').find(b => b.text() === 'Cancel')
    expect(cancelBtn).toBeDefined()
    await cancelBtn!.trigger('click')
    expect(wrapper.emitted('cancel')).toBeTruthy()
  })

  it('should emit cancel on backdrop click', async () => {
    const wrapper = mountDialog()
    const backdrop = wrapper.find('[data-testid="modal-backdrop"]')
    await backdrop.trigger('click')
    expect(wrapper.emitted('cancel')).toBeTruthy()
  })

  it('should emit cancel on X button click', async () => {
    const wrapper = mountDialog()
    const closeBtn = wrapper.find('[aria-label="Close modal"]')
    await closeBtn.trigger('click')
    expect(wrapper.emitted('cancel')).toBeTruthy()
  })

  it('should emit cancel on ESC key', async () => {
    const wrapper = mountDialog()
    document.dispatchEvent(new KeyboardEvent('keydown', { key: 'Escape' }))
    await wrapper.vm.$nextTick()
    expect(wrapper.emitted('cancel')).toBeTruthy()
  })

  it('should show loading spinner when loading is true', () => {
    const wrapper = mountDialog({ loading: true })
    expect(wrapper.find('.animate-spin').exists()).toBe(true)
  })

  it('should disable buttons during loading', () => {
    const wrapper = mountDialog({ loading: true })
    const cancelBtn = wrapper.findAll('button').find(b => b.text() === 'Cancel')
    expect(cancelBtn!.attributes('disabled')).toBeDefined()
    const deleteBtn = wrapper.findAll('button').find(b => b.text() === 'Delete')
    expect(deleteBtn!.attributes('disabled')).toBeDefined()
  })

  it('should not emit confirm when loading is true', async () => {
    const wrapper = mountDialog({ loading: true })
    const deleteBtn = wrapper.findAll('button').find(b => b.text() === 'Delete')
    await deleteBtn!.trigger('click')
    expect(wrapper.emitted('confirm')).toBeFalsy()
  })

  it('should not emit cancel when loading is true', async () => {
    const wrapper = mountDialog({ loading: true })
    const cancelBtn = wrapper.findAll('button').find(b => b.text() === 'Cancel')
    await cancelBtn!.trigger('click')
    expect(wrapper.emitted('cancel')).toBeFalsy()
  })
})
