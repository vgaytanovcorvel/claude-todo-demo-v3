import { useCallback, useState } from 'react'
import { useTodos } from '../hooks/useTodos'
import type { CreateToDoItemRequest, Priority, ToDoItem, ToDoItemStatus, UpdateToDoItemRequest } from '../types/todo'
import { ConfirmDialog } from './ConfirmDialog'
import { TodoFilters } from './TodoFilters'
import { TodoForm } from './TodoForm'
import { TodoItem } from './TodoItem'
import './TodoList.css'

function applyFilters(
  todos: readonly ToDoItem[],
  statusFilter: ToDoItemStatus | null,
  priorityFilter: Priority | null,
): readonly ToDoItem[] {
  return todos.filter(todo => {
    if (statusFilter !== null && todo.status !== statusFilter) return false
    if (priorityFilter !== null && todo.priority !== priorityFilter) return false
    return true
  })
}

export function TodoList() {
  const { todos, loading, error, fetchTodos, addTodo, editTodo, removeTodo } = useTodos()
  const [showForm, setShowForm] = useState(false)
  const [editingTodo, setEditingTodo] = useState<ToDoItem | null>(null)
  const [deletingTodo, setDeletingTodo] = useState<ToDoItem | null>(null)
  const [statusFilter, setStatusFilter] = useState<ToDoItemStatus | null>(null)
  const [priorityFilter, setPriorityFilter] = useState<Priority | null>(null)
  const [deleteError, setDeleteError] = useState<string | null>(null)

  const filteredTodos = applyFilters(todos, statusFilter, priorityFilter)

  function handleAddClick() {
    setEditingTodo(null)
    setShowForm(true)
  }

  function handleEditClick(todo: ToDoItem) {
    setEditingTodo(todo)
    setShowForm(true)
  }

  function handleDeleteClick(todo: ToDoItem) {
    setDeleteError(null)
    setDeletingTodo(todo)
  }

  const handleCreateSubmit = useCallback(async (request: CreateToDoItemRequest) => {
    await addTodo(request)
    setShowForm(false)
    setEditingTodo(null)
  }, [addTodo])

  const handleUpdateSubmit = useCallback(async (request: UpdateToDoItemRequest) => {
    if (!editingTodo) return
    await editTodo(editingTodo.id, request)
    setShowForm(false)
    setEditingTodo(null)
  }, [editingTodo, editTodo])

  const handleFormSubmit = useCallback(async (request: CreateToDoItemRequest | UpdateToDoItemRequest) => {
    if (editingTodo && 'status' in request) {
      await handleUpdateSubmit(request)
    } else if (!editingTodo) {
      await handleCreateSubmit(request)
    }
  }, [editingTodo, handleUpdateSubmit, handleCreateSubmit])

  function handleFormCancel() {
    setShowForm(false)
    setEditingTodo(null)
  }

  async function handleDeleteConfirm() {
    if (!deletingTodo) return
    try {
      await removeTodo(deletingTodo.id)
      setDeletingTodo(null)
    } catch {
      setDeletingTodo(null)
      setDeleteError('Failed to delete todo. Please try again.')
    }
  }

  function handleDeleteCancel() {
    setDeletingTodo(null)
  }

  if (loading) {
    return <div className="todo-loading">Loading todos...</div>
  }

  if (error) {
    return (
      <div className="todo-error">
        <p>Failed to load todos: {error}</p>
        <button type="button" className="btn btn-primary" onClick={() => void fetchTodos()}>
          Retry
        </button>
      </div>
    )
  }

  return (
    <div className="todo-list-container">
      <div className="todo-list-header">
        <TodoFilters
          statusFilter={statusFilter}
          priorityFilter={priorityFilter}
          onStatusChange={setStatusFilter}
          onPriorityChange={setPriorityFilter}
        />
        <button type="button" className="btn btn-primary" onClick={handleAddClick}>
          Add New Todo
        </button>
      </div>

      {filteredTodos.length === 0 ? (
        <div className="todo-empty">
          {todos.length === 0
            ? 'No todos yet. Click "Add New Todo" to create one.'
            : 'No todos match the current filters.'}
        </div>
      ) : (
        <div className="todo-table-wrapper">
          <table className="todo-table">
            <thead>
              <tr>
                <th className="todo-th">Title</th>
                <th className="todo-th">Priority</th>
                <th className="todo-th">Status</th>
                <th className="todo-th">Due Date</th>
                <th className="todo-th">Created</th>
                <th className="todo-th">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredTodos.map(todo => (
                <TodoItem
                  key={todo.id}
                  todo={todo}
                  onEdit={handleEditClick}
                  onDelete={handleDeleteClick}
                />
              ))}
            </tbody>
          </table>
        </div>
      )}

      {showForm && (
        <TodoForm
          editingTodo={editingTodo}
          onSubmit={handleFormSubmit}
          onCancel={handleFormCancel}
        />
      )}

      {deleteError && (
        <div className="todo-error">
          <p>{deleteError}</p>
          <button type="button" className="btn btn-secondary" onClick={() => setDeleteError(null)}>
            Dismiss
          </button>
        </div>
      )}

      {deletingTodo && (
        <ConfirmDialog
          title="Delete Todo"
          message={`Are you sure you want to delete "${deletingTodo.title}"?`}
          confirmLabel="Delete"
          onConfirm={() => void handleDeleteConfirm()}
          onCancel={handleDeleteCancel}
        />
      )}
    </div>
  )
}
