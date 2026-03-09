import { ALL_PRIORITIES, ALL_STATUSES, getPriorityLabel, getStatusLabel, type Priority, type ToDoItemStatus } from '../types/todo'
import './TodoFilters.css'

interface TodoFiltersProps {
  readonly statusFilter: ToDoItemStatus | null
  readonly priorityFilter: Priority | null
  readonly onStatusChange: (status: ToDoItemStatus | null) => void
  readonly onPriorityChange: (priority: Priority | null) => void
}

export function TodoFilters({ statusFilter, priorityFilter, onStatusChange, onPriorityChange }: TodoFiltersProps) {
  return (
    <div className="todo-filters">
      <div className="filter-group">
        <label htmlFor="status-filter" className="filter-label">Status</label>
        <select
          id="status-filter"
          className="filter-select"
          value={statusFilter ?? ''}
          onChange={e => onStatusChange(e.target.value === '' ? null : Number(e.target.value) as ToDoItemStatus)}
        >
          <option value="">All</option>
          {ALL_STATUSES.map(s => (
            <option key={s} value={s}>{getStatusLabel(s)}</option>
          ))}
        </select>
      </div>
      <div className="filter-group">
        <label htmlFor="priority-filter" className="filter-label">Priority</label>
        <select
          id="priority-filter"
          className="filter-select"
          value={priorityFilter ?? ''}
          onChange={e => onPriorityChange(e.target.value === '' ? null : Number(e.target.value) as Priority)}
        >
          <option value="">All</option>
          {ALL_PRIORITIES.map(p => (
            <option key={p} value={p}>{getPriorityLabel(p)}</option>
          ))}
        </select>
      </div>
    </div>
  )
}
