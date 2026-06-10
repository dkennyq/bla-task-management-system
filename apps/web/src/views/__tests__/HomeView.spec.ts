import { describe, it, expect } from 'vitest'
import { mountWithPlugins } from '../../utils/testUtils'
import HomeView from '../HomeView.vue'

describe('HomeView', () => {
  it('should render the home view', () => {
    const { wrapper } = mountWithPlugins(HomeView)
    
    expect(wrapper.text()).toContain('BLA Task Management')
    expect(wrapper.text()).toContain('Welcome to your task management system')
  })

  it('should display feature cards', () => {
    const { wrapper } = mountWithPlugins(HomeView)
    
    expect(wrapper.text()).toContain('Create Tasks')
    expect(wrapper.text()).toContain('Track Progress')
    expect(wrapper.text()).toContain('Stay Organized')
  })
})
