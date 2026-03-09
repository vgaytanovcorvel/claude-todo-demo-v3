import { type FormEvent, useEffect, useRef, useState } from 'react'
import { z } from 'zod'
import { useAuth } from '../hooks/useAuth'
import { zodErrorsToMap } from '../utils/validation'

const registerSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().min(1, 'Email is required').email('Invalid email address'),
  password: z.string().min(8, 'Password must be at least 8 characters'),
})

interface RegisterFormProps {
  readonly onSwitchToLogin: () => void
}

interface FormErrors {
  readonly firstName?: string
  readonly lastName?: string
  readonly email?: string
  readonly password?: string
}

export function RegisterForm({ onSwitchToLogin }: RegisterFormProps) {
  const { register } = useAuth()
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [errors, setErrors] = useState<FormErrors>({})
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState<string | null>(null)

  const firstNameInputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    firstNameInputRef.current?.focus()
  }, [])

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setSubmitError(null)

    const result = registerSchema.safeParse({
      firstName: firstName.trim(),
      lastName: lastName.trim(),
      email: email.trim(),
      password,
    })

    if (!result.success) {
      setErrors(zodErrorsToMap<FormErrors>(result.error))
      return
    }

    setErrors({})
    setSubmitting(true)

    try {
      await register({
        firstName: firstName.trim(),
        lastName: lastName.trim(),
        email: email.trim(),
        password,
      })
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Registration failed. Please try again.'
      setSubmitError(message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="auth-form-container">
      <h2 className="auth-form-title">Create Account</h2>

      {submitError && <div className="auth-error-banner">{submitError}</div>}

      <form onSubmit={handleSubmit} noValidate>
        <div className="auth-field-row">
          <div className="auth-field">
            <label htmlFor="register-first-name" className="auth-label">First Name</label>
            <input
              ref={firstNameInputRef}
              id="register-first-name"
              type="text"
              className={`auth-input ${errors.firstName ? 'input-error' : ''}`}
              value={firstName}
              onChange={e => setFirstName(e.target.value)}
              autoComplete="given-name"
              required
            />
            {errors.firstName && <span className="auth-field-error">{errors.firstName}</span>}
          </div>

          <div className="auth-field">
            <label htmlFor="register-last-name" className="auth-label">Last Name</label>
            <input
              id="register-last-name"
              type="text"
              className={`auth-input ${errors.lastName ? 'input-error' : ''}`}
              value={lastName}
              onChange={e => setLastName(e.target.value)}
              autoComplete="family-name"
              required
            />
            {errors.lastName && <span className="auth-field-error">{errors.lastName}</span>}
          </div>
        </div>

        <div className="auth-field">
          <label htmlFor="register-email" className="auth-label">Email</label>
          <input
            id="register-email"
            type="email"
            className={`auth-input ${errors.email ? 'input-error' : ''}`}
            value={email}
            onChange={e => setEmail(e.target.value)}
            autoComplete="email"
            required
          />
          {errors.email && <span className="auth-field-error">{errors.email}</span>}
        </div>

        <div className="auth-field">
          <label htmlFor="register-password" className="auth-label">Password</label>
          <input
            id="register-password"
            type="password"
            className={`auth-input ${errors.password ? 'input-error' : ''}`}
            value={password}
            onChange={e => setPassword(e.target.value)}
            autoComplete="new-password"
            required
          />
          {errors.password && <span className="auth-field-error">{errors.password}</span>}
        </div>

        <button type="submit" className="btn btn-primary auth-submit-btn" disabled={submitting}>
          {submitting ? 'Creating account...' : 'Create Account'}
        </button>
      </form>

      <p className="auth-switch-text">
        Already have an account?{' '}
        <button type="button" className="auth-link-btn" onClick={onSwitchToLogin}>
          Sign in
        </button>
      </p>
    </div>
  )
}
