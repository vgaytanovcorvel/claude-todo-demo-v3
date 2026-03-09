export interface UserProfile {
  readonly id: number
  readonly email: string
  readonly firstName: string
  readonly lastName: string
  readonly createdAtUtc: string
  readonly updatedAtUtc: string | null
}

export interface AuthToken {
  readonly token: string
  readonly expiresAtUtc: string
}

export interface LoginRequest {
  readonly email: string
  readonly password: string
}

export interface RegisterRequest {
  readonly email: string
  readonly password: string
  readonly firstName: string
  readonly lastName: string
}

export interface UpdateProfileRequest {
  readonly firstName: string
  readonly lastName: string
  readonly email: string
}

export interface ChangePasswordRequest {
  readonly currentPassword: string
  readonly newPassword: string
}
