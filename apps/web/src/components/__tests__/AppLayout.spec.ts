import { describe, it, expect } from 'vitest'
import { mountWithPlugins } from '../../utils/testUtils'
import AppLayout from '../AppLayout.vue'

describe('AppLayout', () => {
  it('should render the layout with header and footer', () => {
    const { wrapper } = mountWithPlugins(AppLayout)
    
    expect(wrapper.find('header').exists()).toBe(true)
    expect(wrapper.find('main').exists()).toBe(true)
    expect(wrapper.find('footer').exists()).toBe(true)
  })

  it('should display footer text', () => {
    const { wrapper } = mountWithPlugins(AppLayout)
    
    expect(wrapper.text()).toContain('BLA Task Management System')
  })
})
