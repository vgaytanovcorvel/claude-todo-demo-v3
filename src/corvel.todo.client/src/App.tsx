import { useState } from 'react'
import './App.css'
import { AuthPage } from './components/AuthPage'
import { ProfilePage } from './components/ProfilePage'
import { TodoList } from './components/TodoList'
import { useAuth } from './hooks/useAuth'

type AppView = 'todos' | 'profile'

function App() {
  const { user, isAuthenticated, isLoading, logout } = useAuth()
  const [view, setView] = useState<AppView>('todos')

  if (isLoading) {
    return (
      <div className="app">
        <div className="app-loading">Loading...</div>
      </div>
    )
  }

  if (!isAuthenticated) {
    return (
      <div className="app">
        <AuthPage />
      </div>
    )
  }

  return (
    <div className="app">
      <header className="app-header">
        <div className="app-header-content">
          <div>
            <h1 className="app-title">Corvel ToDo</h1>
            <p className="app-subtitle">Manage your tasks efficiently</p>
          </div>
          <div className="app-header-actions">
            <span className="app-user-name">{user?.firstName} {user?.lastName}</span>
            <button
              type="button"
              className="btn btn-secondary btn-small"
              onClick={() => setView(view === 'todos' ? 'profile' : 'todos')}
            >
              {view === 'todos' ? 'Profile' : 'Todos'}
            </button>
            <button type="button" className="btn btn-secondary btn-small" onClick={logout}>
              Logout
            </button>
          </div>
        </div>
      </header>
      <main className="app-main">
        {view === 'todos' ? (
          <TodoList />
        ) : (
          <ProfilePage onBack={() => setView('todos')} />
        )}
      </main>
    </div>
  )
}

export default App
