// Task Entity Types
export interface Task {
  id: string
  userId: string
  title: string
  description: string
  status: TaskStatus
  priority: TaskPriority
  dueDate?: string
  createdAt: string
  updatedAt: string
}

export enum TaskStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed'
}

export enum TaskPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High'
}

export interface CreateTaskDto {
  title: string
  description: string
  status: TaskStatus
  priority: TaskPriority
  dueDate?: string
}

export interface UpdateTaskDto {
  title?: string
  description?: string
  status?: TaskStatus
  priority?: TaskPriority
  dueDate?: string
}
