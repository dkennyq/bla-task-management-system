// API Response Types
export interface ApiResponse<T> {
  data?: T
  error?: ApiError
  success: boolean
}

export interface ApiError {
  message: string
  statusCode: number
  errors?: Record<string, string[]>
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
}
