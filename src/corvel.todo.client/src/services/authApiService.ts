import type {
  AuthToken,
  ChangePasswordRequest,
  LoginRequest,
  RegisterRequest,
  UpdateProfileRequest,
  UserProfile,
} from '../types/auth'
import { apiFetch, apiResponseSchema, handleApiResponse } from './apiClient'

export async function register(request: RegisterRequest): Promise<AuthToken> {
  const response = await apiFetch('/api/auth/register', {
    method: 'POST',
    body: JSON.stringify(request),
  })
  return handleApiResponse<AuthToken>(response)
}

export async function login(request: LoginRequest): Promise<AuthToken> {
  const response = await apiFetch('/api/auth/login', {
    method: 'POST',
    body: JSON.stringify(request),
  })
  return handleApiResponse<AuthToken>(response)
}

export async function getProfile(): Promise<UserProfile> {
  const response = await apiFetch('/api/users/profile')
  return handleApiResponse<UserProfile>(response)
}

export async function updateProfile(request: UpdateProfileRequest): Promise<UserProfile> {
  const response = await apiFetch('/api/users/profile', {
    method: 'PUT',
    body: JSON.stringify(request),
  })
  return handleApiResponse<UserProfile>(response)
}

export async function changePassword(request: ChangePasswordRequest): Promise<void> {
  const response = await apiFetch('/api/users/profile/password', {
    method: 'PUT',
    body: JSON.stringify(request),
  })

  if (response.status === 204) {
    return
  }

  const json: unknown = await response.json()
  const body = apiResponseSchema.parse(json)

  if (!body.success) {
    throw new Error(body.error ?? `Request failed with status ${body.statusCode}`)
  }
}
