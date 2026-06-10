// User Entity Types
export interface User {
  id: string
  username: string
  email: string
  fullName: string
  createdAt: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  token: string
  user: User
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
  fullName: string
}

export interface RegisterResponse {
  user: User
  message: string
}
