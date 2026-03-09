import './App.css'
import { TodoList } from './components/TodoList'

function App() {
  return (
    <div className="app">
      <header className="app-header">
        <h1 className="app-title">Corvel ToDo</h1>
        <p className="app-subtitle">Manage your tasks efficiently</p>
      </header>
      <main className="app-main">
        <TodoList />
      </main>
    </div>
  )
}

export default App
