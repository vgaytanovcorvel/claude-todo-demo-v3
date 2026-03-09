import {
  Priority,
  ToDoItemStatus,
  getPriorityLabel,
  getStatusLabel,
  type ToDoItem,
} from '../types/todo'
import './TodoItem.css'

const PRIORITY_COLORS: Record<Priority, string> = {
  [Priority.Low]: '#4caf50',
  [Priority.Medium]: '#ff9800',
  [Priority.High]: '#f44336',
  [Priority.Critical]: '#9c27b0',
}

const STATUS_CLASS: Record<ToDoItemStatus, string> = {
  [ToDoItemStatus.Pending]: 'status-pending',
  [ToDoItemStatus.InProgress]: 'status-in-progress',
  [ToDoItemStatus.Completed]: 'status-completed',
  [ToDoItemStatus.Cancelled]: 'status-cancelled',
}

interface TodoItemProps {
  readonly todo: ToDoItem
  readonly onEdit: (todo: ToDoItem) => void
  readonly onDelete: (todo: ToDoItem) => void
}

function isOverdue(todo: ToDoItem): boolean {
  if (!todo.dueDate) return false
  if (todo.status === ToDoItemStatus.Completed || todo.status === ToDoItemStatus.Cancelled) return false
  return new Date(todo.dueDate) < new Date()
}

function formatDate(isoDate: string | null): string {
  if (!isoDate) return '-'
  return new Date(isoDate).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

export function TodoItem({ todo, onEdit, onDelete }: TodoItemProps) {
  const overdue = isOverdue(todo)

  return (
    <tr className={`todo-row ${overdue ? 'todo-overdue' : ''}`}>
      <td className="todo-cell todo-title-cell">
        <span className="todo-title-text">{todo.title}</span>
        {todo.description && (
          <span className="todo-description">{todo.description}</span>
        )}
      </td>
      <td className="todo-cell">
        <span
          className="priority-badge"
          style={{ backgroundColor: PRIORITY_COLORS[todo.priority] }}
        >
          {getPriorityLabel(todo.priority)}
        </span>
      </td>
      <td className="todo-cell">
        <span className={`status-badge ${STATUS_CLASS[todo.status]}`}>
          {getStatusLabel(todo.status)}
        </span>
      </td>
      <td className="todo-cell todo-date-cell">
        {overdue && <span className="overdue-indicator">Overdue</span>}
        {formatDate(todo.dueDate)}
      </td>
      <td className="todo-cell todo-date-cell">
        {formatDate(todo.createdAtUtc)}
      </td>
      <td className="todo-cell todo-actions-cell">
        <button
          type="button"
          className="btn btn-small btn-secondary"
          onClick={() => onEdit(todo)}
          aria-label={`Edit ${todo.title}`}
        >
          Edit
        </button>
        <button
          type="button"
          className="btn btn-small btn-danger"
          onClick={() => onDelete(todo)}
          aria-label={`Delete ${todo.title}`}
        >
          Delete
        </button>
      </td>
    </tr>
  )
}
