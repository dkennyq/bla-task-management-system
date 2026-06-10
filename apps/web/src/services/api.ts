import { tasksApiClient, usersApiClient } from './apiConfig'
import type { Task, CreateTaskDto, UpdateTaskDto } from '../types/task'
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse, User } from '../types/user'

// ==================== Auth API ====================

export async function login(credentials: LoginRequest): Promise<LoginResponse> {
  const response = await usersApiClient.post<LoginResponse>('/users/login', credentials)
  return response.data
}

export async function register(data: RegisterRequest): Promise<RegisterResponse> {
  const response = await usersApiClient.post<RegisterResponse>('/users/register', data)
  return response.data
}

export async function getCurrentUser(): Promise<User> {
  const response = await usersApiClient.get<User>('/users/me')
  return response.data
}

export async function getAllUsers(): Promise<User[]> {
  const response = await usersApiClient.get<User[]>('/users')
  return response.data
}

// ==================== Tasks API ====================

export async function getTasks(userId: string): Promise<Task[]> {
  const response = await tasksApiClient.get<Task[]>(`/tasks?userId=${userId}`)
  return response.data
}

export async function getTaskById(id: string): Promise<Task> {
  const response = await tasksApiClient.get<Task>(`/tasks/${id}`)
  return response.data
}

export async function createTask(task: CreateTaskDto): Promise<Task> {
  const response = await tasksApiClient.post<Task>('/tasks', task)
  return response.data
}

export async function updateTask(id: string, task: UpdateTaskDto): Promise<Task> {
  const response = await tasksApiClient.put<Task>(`/tasks/${id}`, task)
  return response.data
}

export async function deleteTask(id: string): Promise<void> {
  await tasksApiClient.delete(`/tasks/${id}`)
}
