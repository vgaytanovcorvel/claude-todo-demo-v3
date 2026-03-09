import { type FormEvent, useState } from 'react'
import { z } from 'zod'
import * as authApiService from '../services/authApiService'
import { zodErrorsToMap } from '../utils/validation'

const passwordSchema = z.object({
  currentPassword: z.string().min(1, 'Current password is required'),
  newPassword: z.string().min(8, 'New password must be at least 8 characters'),
})

interface PasswordErrors {
  readonly currentPassword?: string
  readonly newPassword?: string
}

export function ChangePasswordForm() {
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [passwordErrors, setPasswordErrors] = useState<PasswordErrors>({})
  const [submitting, setSubmitting] = useState(false)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setSuccessMessage(null)
    setErrorMessage(null)

    const result = passwordSchema.safeParse({ currentPassword, newPassword })

    if (!result.success) {
      setPasswordErrors(zodErrorsToMap<PasswordErrors>(result.error))
      return
    }

    setPasswordErrors({})
    setSubmitting(true)

    try {
      await authApiService.changePassword({ currentPassword, newPassword })
      setCurrentPassword('')
      setNewPassword('')
      setSuccessMessage('Password changed successfully.')
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to change password.'
      setErrorMessage(message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="profile-section">
      <h3 className="profile-section-title">Change Password</h3>

      {successMessage && <div className="profile-success-banner">{successMessage}</div>}
      {errorMessage && <div className="profile-error-banner">{errorMessage}</div>}

      <form onSubmit={handleSubmit} noValidate>
        <div className="auth-field">
          <label htmlFor="profile-current-password" className="auth-label">Current Password</label>
          <input
            id="profile-current-password"
            type="password"
            className={`auth-input ${passwordErrors.currentPassword ? 'input-error' : ''}`}
            value={currentPassword}
            onChange={e => setCurrentPassword(e.target.value)}
            autoComplete="current-password"
          />
          {passwordErrors.currentPassword && <span className="auth-field-error">{passwordErrors.currentPassword}</span>}
        </div>

        <div className="auth-field">
          <label htmlFor="profile-new-password" className="auth-label">New Password</label>
          <input
            id="profile-new-password"
            type="password"
            className={`auth-input ${passwordErrors.newPassword ? 'input-error' : ''}`}
            value={newPassword}
            onChange={e => setNewPassword(e.target.value)}
            autoComplete="new-password"
          />
          {passwordErrors.newPassword && <span className="auth-field-error">{passwordErrors.newPassword}</span>}
        </div>

        <button type="submit" className="btn btn-primary" disabled={submitting}>
          {submitting ? 'Changing...' : 'Change Password'}
        </button>
      </form>
    </div>
  )
}
