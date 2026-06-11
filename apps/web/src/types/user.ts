// User Entity Types
export type UserRole = 'Operator' | 'Manager'

export interface User {
  id: string
  username: string
  email: string
  fullName: string
  role: UserRole
  createdAt: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  userId: string
  email: string
  fullName: string
  role: UserRole
  token: string
  refreshToken: string
  expiresAt: string
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
  fullName: string
}

export interface RegisterResponse {
  id: string
  username: string
  email: string
  fullName: string
  createdAt: string
}

export interface UserListItem {
  id: string
  email: string
  role: UserRole
  isActive: boolean
  createdAt: string
}

export interface CreateUserByAdminRequest {
  username: string
  email: string
  password: string
  fullName: string
  role: UserRole
}

export interface UpdateUserRoleRequest {
  role: UserRole
}
