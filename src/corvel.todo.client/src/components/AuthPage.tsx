import { useState } from 'react'
import { LoginForm } from './LoginForm'
import { RegisterForm } from './RegisterForm'

type AuthView = 'login' | 'register'

export function AuthPage() {
  const [view, setView] = useState<AuthView>('login')

  return (
    <div className="auth-page">
      <div className="auth-card">
        <h1 className="auth-app-title">Corvel ToDo</h1>
        {view === 'login' ? (
          <LoginForm onSwitchToRegister={() => setView('register')} />
        ) : (
          <RegisterForm onSwitchToLogin={() => setView('login')} />
        )}
      </div>
    </div>
  )
}
