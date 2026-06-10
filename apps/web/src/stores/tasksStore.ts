import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Task, CreateTaskDto, UpdateTaskDto, TaskStatus } from '../types/task'
import { getTasks, getTaskById, createTask as createTaskApi, updateTask as updateTaskApi, deleteTask as deleteTaskApi } from '../services/api'

export const useTasksStore = defineStore('tasks', () => {
  // State
  const tasks = ref<Task[]>([])
  const currentTask = ref<Task | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const filter = ref<TaskStatus | 'All'>('All')
  const searchQuery = ref('')

  // Getters
  const filteredTasks = computed(() => {
    let result = tasks.value

    // Apply status filter
    if (filter.value !== 'All') {
      result = result.filter(task => task.status === filter.value)
    }

    // Apply search
    if (searchQuery.value) {
      const query = searchQuery.value.toLowerCase()
      result = result.filter(task => 
        task.title.toLowerCase().includes(query) ||
        task.description.toLowerCase().includes(query)
      )
    }

    return result
  })

  const tasksByStatus = computed(() => {
    return {
      pending: tasks.value.filter(t => t.status === 'Pending').length,
      inProgress: tasks.value.filter(t => t.status === 'InProgress').length,
      completed: tasks.value.filter(t => t.status === 'Completed').length,
    }
  })

  // Actions
  async function fetchTasks(userId: string) {
    loading.value = true
    error.value = null
    try {
      const data = await getTasks(userId)
      tasks.value = data
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch tasks'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchTaskById(id: string) {
    loading.value = true
    error.value = null
    try {
      const data = await getTaskById(id)
      currentTask.value = data
      return data
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch task'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createTask(task: CreateTaskDto) {
    loading.value = true
    error.value = null
    try {
      const newTask = await createTaskApi(task)
      tasks.value.push(newTask)
      return newTask
    } catch (err: any) {
      error.value = err.message || 'Failed to create task'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateTask(id: string, task: UpdateTaskDto) {
    loading.value = true
    error.value = null
    try {
      const updatedTask = await updateTaskApi(id, task)
      const index = tasks.value.findIndex(t => t.id === id)
      if (index !== -1) {
        tasks.value[index] = updatedTask
      }
      return updatedTask
    } catch (err: any) {
      error.value = err.message || 'Failed to update task'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteTask(id: string) {
    loading.value = true
    error.value = null
    try {
      await deleteTaskApi(id)
      tasks.value = tasks.value.filter(t => t.id !== id)
    } catch (err: any) {
      error.value = err.message || 'Failed to delete task'
      throw err
    } finally {
      loading.value = false
    }
  }

  function setFilter(newFilter: TaskStatus | 'All') {
    filter.value = newFilter
  }

  function setSearchQuery(query: string) {
    searchQuery.value = query
  }

  function clearError() {
    error.value = null
  }

  return {
    // State
    tasks,
    currentTask,
    loading,
    error,
    filter,
    searchQuery,
    // Getters
    filteredTasks,
    tasksByStatus,
    // Actions
    fetchTasks,
    fetchTaskById,
    createTask,
    updateTask,
    deleteTask,
    setFilter,
    setSearchQuery,
    clearError,
  }
})
