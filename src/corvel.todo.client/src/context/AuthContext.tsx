import { createContext, type ReactNode, useCallback, useEffect, useMemo, useState } from 'react'
import { clearAuthToken, getAuthToken, setAuthToken, setOnUnauthorized } from '../services/apiClient'
import * as authApiService from '../services/authApiService'
import type { LoginRequest, RegisterRequest, UserProfile } from '../types/auth'

export interface AuthContextValue {
  readonly user: UserProfile | null
  readonly token: string | null
  readonly isAuthenticated: boolean
  readonly isLoading: boolean
  login: (request: LoginRequest) => Promise<void>
  register: (request: RegisterRequest) => Promise<void>
  logout: () => void
  refreshProfile: () => Promise<void>
}

export const AuthContext = createContext<AuthContextValue | null>(null)

interface AuthProviderProps {
  readonly children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<UserProfile | null>(null)
  const [token, setToken] = useState<string | null>(getAuthToken)
  const [isLoading, setIsLoading] = useState(true)

  const logout = useCallback(() => {
    clearAuthToken()
    setToken(null)
    setUser(null)
  }, [])

  useEffect(() => {
    return setOnUnauthorized(logout)
  }, [logout])

  const refreshProfile = useCallback(async () => {
    try {
      const profile = await authApiService.getProfile()
      setUser(profile)
    } catch {
      logout()
    }
  }, [logout])

  useEffect(() => {
    async function validateToken() {
      const existingToken = getAuthToken()
      if (!existingToken) {
        setIsLoading(false)
        return
      }

      try {
        const profile = await authApiService.getProfile()
        setUser(profile)
        setToken(existingToken)
      } catch {
        clearAuthToken()
        setToken(null)
        setUser(null)
      } finally {
        setIsLoading(false)
      }
    }

    void validateToken()
  }, [])

  const login = useCallback(async (request: LoginRequest) => {
    const authToken = await authApiService.login(request)
    setAuthToken(authToken.token, authToken.expiresAtUtc)
    setToken(authToken.token)
    try {
      const profile = await authApiService.getProfile()
      setUser(profile)
    } catch (err) {
      logout()
      throw err
    }
  }, [logout])

  const register = useCallback(async (request: RegisterRequest) => {
    const authToken = await authApiService.register(request)
    setAuthToken(authToken.token, authToken.expiresAtUtc)
    setToken(authToken.token)
    try {
      const profile = await authApiService.getProfile()
      setUser(profile)
    } catch (err) {
      logout()
      throw err
    }
  }, [logout])

  const isAuthenticated = token !== null && user !== null

  const value = useMemo<AuthContextValue>(() => ({
    user,
    token,
    isAuthenticated,
    isLoading,
    login,
    register,
    logout,
    refreshProfile,
  }), [user, token, isAuthenticated, isLoading, login, register, logout, refreshProfile])

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  )
}
