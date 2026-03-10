import { z } from 'zod'

const AUTH_TOKEN_KEY = 'auth_token'
const AUTH_EXPIRES_KEY = 'auth_expires'

export function getAuthToken(): string | null {
  const token = localStorage.getItem(AUTH_TOKEN_KEY)
  const expires = localStorage.getItem(AUTH_EXPIRES_KEY)

  if (!token) {
    return null
  }

  if (expires && new Date(expires) <= new Date()) {
    clearAuthToken()
    return null
  }

  return token
}

export function setAuthToken(token: string, expiresAtUtc: string): void {
  localStorage.setItem(AUTH_TOKEN_KEY, token)
  localStorage.setItem(AUTH_EXPIRES_KEY, expiresAtUtc)
}

export function clearAuthToken(): void {
  localStorage.removeItem(AUTH_TOKEN_KEY)
  localStorage.removeItem(AUTH_EXPIRES_KEY)
}

let registeredUnauthorizedCallback: (() => void) | null = null

export function setOnUnauthorized(callback: () => void): () => void {
  registeredUnauthorizedCallback = callback
  return () => {
    registeredUnauthorizedCallback = null
  }
}

const METHODS_WITH_BODY = new Set(['POST', 'PUT', 'PATCH'])

export async function apiFetch(url: string, options: RequestInit = {}): Promise<Response> {
  const token = getAuthToken()

  const headers: Record<string, string> = {
    ...((options.headers as Record<string, string>) ?? {}),
  }

  const method = (options.method ?? 'GET').toUpperCase()
  if (METHODS_WITH_BODY.has(method)) {
    headers['Content-Type'] = headers['Content-Type'] ?? 'application/json'
  }

  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  const response = await fetch(url, {
    ...options,
    headers,
  })

  if (response.status === 401) {
    registeredUnauthorizedCallback?.()
    throw new Error('Authentication expired. Please sign in again.')
  }

  return response
}

// Shared API response schema and handler — used by all API service modules
export const apiResponseSchema = z.object({
  success: z.boolean(),
  data: z.unknown().optional(),
  error: z.string().nullable().optional(),
  statusCode: z.number(),
})

/**
 * Parses and validates a standard API envelope response.
 * The caller trusts the server to return the correct shape for `T`
 * (standard practice for typed API clients with a shared backend).
 */
export async function handleApiResponse<T>(response: Response): Promise<T> {
  const text = await response.text()
  if (!text) {
    throw new Error(`Server returned ${response.status} with no response body`)
  }

  let json: unknown
  try {
    json = JSON.parse(text)
  } catch {
    throw new Error(`Server returned ${response.status}: ${text.substring(0, 200)}`)
  }

  const body = apiResponseSchema.parse(json)

  if (!body.success) {
    throw new Error(body.error ?? `Request failed with status ${body.statusCode}`)
  }

  if (body.data === undefined || body.data === null) {
    throw new Error('Response data is missing')
  }

  return body.data as T
}
