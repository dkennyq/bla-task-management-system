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
  userId: string
  email: string
  fullName: string
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
