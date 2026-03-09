import { type FormEvent, useEffect, useRef, useState } from 'react'
import { z } from 'zod'
import { useAuth } from '../hooks/useAuth'
import { zodErrorsToMap } from '../utils/validation'

const loginSchema = z.object({
  email: z.string().min(1, 'Email is required').email('Invalid email address'),
  password: z.string().min(1, 'Password is required'),
})

interface LoginFormProps {
  readonly onSwitchToRegister: () => void
}

interface FormErrors {
  readonly email?: string
  readonly password?: string
}

export function LoginForm({ onSwitchToRegister }: LoginFormProps) {
  const { login } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [errors, setErrors] = useState<FormErrors>({})
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState<string | null>(null)

  const emailInputRef = useRef<HTMLInputElement>(null)

  useEffect(() => {
    emailInputRef.current?.focus()
  }, [])

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setSubmitError(null)

    const result = loginSchema.safeParse({ email: email.trim(), password })

    if (!result.success) {
      setErrors(zodErrorsToMap<FormErrors>(result.error))
      return
    }

    setErrors({})
    setSubmitting(true)

    try {
      await login({ email: email.trim(), password })
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Login failed. Please try again.'
      setSubmitError(message)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="auth-form-container">
      <h2 className="auth-form-title">Sign In</h2>

      {submitError && <div className="auth-error-banner">{submitError}</div>}

      <form onSubmit={handleSubmit} noValidate>
        <div className="auth-field">
          <label htmlFor="login-email" className="auth-label">Email</label>
          <input
            ref={emailInputRef}
            id="login-email"
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
          <label htmlFor="login-password" className="auth-label">Password</label>
          <input
            id="login-password"
            type="password"
            className={`auth-input ${errors.password ? 'input-error' : ''}`}
            value={password}
            onChange={e => setPassword(e.target.value)}
            autoComplete="current-password"
            required
          />
          {errors.password && <span className="auth-field-error">{errors.password}</span>}
        </div>

        <button type="submit" className="btn btn-primary auth-submit-btn" disabled={submitting}>
          {submitting ? 'Signing in...' : 'Sign In'}
        </button>
      </form>

      <p className="auth-switch-text">
        Don&apos;t have an account?{' '}
        <button type="button" className="auth-link-btn" onClick={onSwitchToRegister}>
          Create one
        </button>
      </p>
    </div>
  )
}
