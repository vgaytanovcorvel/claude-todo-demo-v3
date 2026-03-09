import { type FormEvent, useEffect, useState } from 'react'
import { z } from 'zod'
import { useAuth } from '../hooks/useAuth'
import * as authApiService from '../services/authApiService'
import { zodErrorsToMap } from '../utils/validation'

const profileSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().min(1, 'Email is required').email('Invalid email address'),
})

interface ProfileErrors {
  readonly firstName?: string
  readonly lastName?: string
  readonly email?: string
}

export function ProfileForm() {
  const { user, refreshProfile } = useAuth()

  const [firstName, setFirstName] = useState(user?.firstName ?? '')
  const [lastName, setLastName] = useState(user?.lastName ?? '')
  const [email, setEmail] = useState(user?.email ?? '')
  const [profileErrors, setProfileErrors] = useState<ProfileErrors>({})
  const [submitting, setSubmitting] = useState(false)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    if (user) {
      setFirstName(user.firstName)
      setLastName(user.lastName)
      setEmail(user.email)
    }
  }, [user])

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setSuccessMessage(null)
    setErrorMessage(null)

    const result = profileSchema.safeParse({
      firstName: firstName.trim(),
      lastName: lastName.trim(),
      email: email.trim(),
    })

    if (!result.success) {
      setProfileErrors(zodErrorsToMap<ProfileErrors>(result.error))
      return
    }

    setProfileErrors({})
    setSubmitting(true)

    try {
      await authApiService.updateProfile({
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
      })
      await refreshProfile()
      setSuccessMessage('Profile updated successfully.')
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to update profile.'
      setErrorMessage(message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="profile-section">
      <h3 className="profile-section-title">Personal Information</h3>

      {successMessage && <div className="profile-success-banner">{successMessage}</div>}
      {errorMessage && <div className="profile-error-banner">{errorMessage}</div>}

      <form onSubmit={handleSubmit} noValidate>
        <div className="auth-field-row">
          <div className="auth-field">
            <label htmlFor="profile-first-name" className="auth-label">First Name</label>
            <input
              id="profile-first-name"
              type="text"
              className={`auth-input ${profileErrors.firstName ? 'input-error' : ''}`}
              value={firstName}
              onChange={e => setFirstName(e.target.value)}
            />
            {profileErrors.firstName && <span className="auth-field-error">{profileErrors.firstName}</span>}
          </div>

          <div className="auth-field">
            <label htmlFor="profile-last-name" className="auth-label">Last Name</label>
            <input
              id="profile-last-name"
              type="text"
              className={`auth-input ${profileErrors.lastName ? 'input-error' : ''}`}
              value={lastName}
              onChange={e => setLastName(e.target.value)}
            />
            {profileErrors.lastName && <span className="auth-field-error">{profileErrors.lastName}</span>}
          </div>
        </div>

        <div className="auth-field">
          <label htmlFor="profile-email" className="auth-label">Email</label>
          <input
            id="profile-email"
            type="email"
            className={`auth-input ${profileErrors.email ? 'input-error' : ''}`}
            value={email}
            onChange={e => setEmail(e.target.value)}
          />
          {profileErrors.email && <span className="auth-field-error">{profileErrors.email}</span>}
        </div>

        <button type="submit" className="btn btn-primary" disabled={submitting}>
          {submitting ? 'Saving...' : 'Save Changes'}
        </button>
      </form>
    </div>
  )
}
