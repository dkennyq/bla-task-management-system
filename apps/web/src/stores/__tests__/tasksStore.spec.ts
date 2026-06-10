import { describe, it, expect, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useTasksStore } from '../tasksStore'
import type { Task, TaskStatus } from '../../types/task'

describe('Tasks Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('should initialize with empty tasks array', () => {
    const store = useTasksStore()
    expect(store.tasks).toEqual([])
    expect(store.currentTask).toBe(null)
    expect(store.loading).toBe(false)
    expect(store.error).toBe(null)
  })

  it('should filter tasks by status', () => {
    const store = useTasksStore()
    const mockTasks: Task[] = [
      { id: '1', userId: 'user1', title: 'Task 1', description: 'Desc 1', status: 'Pending' as TaskStatus, priority: 'Low', createdAt: '2026-01-01', updatedAt: '2026-01-01' },
      { id: '2', userId: 'user1', title: 'Task 2', description: 'Desc 2', status: 'InProgress' as TaskStatus, priority: 'Medium', createdAt: '2026-01-02', updatedAt: '2026-01-02' },
      { id: '3', userId: 'user1', title: 'Task 3', description: 'Desc 3', status: 'Completed' as TaskStatus, priority: 'High', createdAt: '2026-01-03', updatedAt: '2026-01-03' },
    ]
    
    store.tasks = mockTasks
    
    store.setFilter('Pending' as TaskStatus)
    expect(store.filteredTasks).toHaveLength(1)
    expect(store.filteredTasks[0].id).toBe('1')
    
    store.setFilter('InProgress' as TaskStatus)
    expect(store.filteredTasks).toHaveLength(1)
    expect(store.filteredTasks[0].id).toBe('2')
  })

  it('should search tasks by title or description', () => {
    const store = useTasksStore()
    const mockTasks: Task[] = [
      { id: '1', userId: 'user1', title: 'Buy groceries', description: 'Milk and eggs', status: 'Pending' as TaskStatus, priority: 'Low', createdAt: '2026-01-01', updatedAt: '2026-01-01' },
      { id: '2', userId: 'user1', title: 'Meeting', description: 'Team sync', status: 'Pending' as TaskStatus, priority: 'Medium', createdAt: '2026-01-02', updatedAt: '2026-01-02' },
    ]
    
    store.tasks = mockTasks
    
    store.setSearchQuery('groceries')
    expect(store.filteredTasks).toHaveLength(1)
    expect(store.filteredTasks[0].id).toBe('1')
    
    store.setSearchQuery('sync')
    expect(store.filteredTasks).toHaveLength(1)
    expect(store.filteredTasks[0].id).toBe('2')
  })

  it('should calculate tasks by status', () => {
    const store = useTasksStore()
    const mockTasks: Task[] = [
      { id: '1', userId: 'user1', title: 'Task 1', description: 'Desc 1', status: 'Pending' as TaskStatus, priority: 'Low', createdAt: '2026-01-01', updatedAt: '2026-01-01' },
      { id: '2', userId: 'user1', title: 'Task 2', description: 'Desc 2', status: 'Pending' as TaskStatus, priority: 'Medium', createdAt: '2026-01-02', updatedAt: '2026-01-02' },
      { id: '3', userId: 'user1', title: 'Task 3', description: 'Desc 3', status: 'InProgress' as TaskStatus, priority: 'High', createdAt: '2026-01-03', updatedAt: '2026-01-03' },
      { id: '4', userId: 'user1', title: 'Task 4', description: 'Desc 4', status: 'Completed' as TaskStatus, priority: 'Low', createdAt: '2026-01-04', updatedAt: '2026-01-04' },
    ]
    
    store.tasks = mockTasks
    
    expect(store.tasksByStatus.pending).toBe(2)
    expect(store.tasksByStatus.inProgress).toBe(1)
    expect(store.tasksByStatus.completed).toBe(1)
  })
})
