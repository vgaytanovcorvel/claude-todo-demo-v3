import { type FormEvent, useEffect, useRef, useState } from 'react'
import { DESCRIPTION_MAX_LENGTH, TITLE_MAX_LENGTH } from '../constants/validation'
import {
  ALL_PRIORITIES,
  ALL_STATUSES,
  Priority,
  ToDoItemStatus,
  getPriorityLabel,
  getStatusLabel,
  type CreateToDoItemRequest,
  type ToDoItem,
  type UpdateToDoItemRequest,
} from '../types/todo'
import './TodoForm.css'

interface TodoFormProps {
  readonly editingTodo: ToDoItem | null
  readonly onSubmit: (request: CreateToDoItemRequest | UpdateToDoItemRequest) => Promise<void>
  readonly onCancel: () => void
}

interface FormErrors {
  readonly title?: string
  readonly description?: string
  readonly dueDate?: string
}

function formatDateForInput(isoDate: string | null): string {
  if (!isoDate) return ''
  return isoDate.substring(0, 16)
}

function validateForm(
  title: string,
  description: string,
  dueDate: string,
  isEditing: boolean,
): FormErrors {
  let titleError: string | undefined
  let descriptionError: string | undefined
  let dueDateError: string | undefined

  const trimmedTitle = title.trim()
  if (!trimmedTitle) {
    titleError = 'Title is required'
  } else if (trimmedTitle.length > TITLE_MAX_LENGTH) {
    titleError = `Title must be ${TITLE_MAX_LENGTH} characters or fewer`
  }

  if (description.trim().length > DESCRIPTION_MAX_LENGTH) {
    descriptionError = `Description must be ${DESCRIPTION_MAX_LENGTH} characters or fewer`
  }

  if (dueDate && !isEditing) {
    const dueDateValue = new Date(dueDate)
    if (dueDateValue <= new Date()) {
      dueDateError = 'Due date must be in the future'
    }
  }

  return {
    ...(titleError ? { title: titleError } : {}),
    ...(descriptionError ? { description: descriptionError } : {}),
    ...(dueDateError ? { dueDate: dueDateError } : {}),
  }
}

export function TodoForm({ editingTodo, onSubmit, onCancel }: TodoFormProps) {
  const [title, setTitle] = useState(editingTodo?.title ?? '')
  const [description, setDescription] = useState(editingTodo?.description ?? '')
  const [priority, setPriority] = useState<Priority>(editingTodo?.priority ?? Priority.Medium)
  const [status, setStatus] = useState<ToDoItemStatus>(editingTodo?.status ?? ToDoItemStatus.Pending)
  const [dueDate, setDueDate] = useState(formatDateForInput(editingTodo?.dueDate ?? null))
  const [errors, setErrors] = useState<FormErrors>({})
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState<string | null>(null)

  const titleInputRef = useRef<HTMLInputElement>(null)
  const isEditing = editingTodo !== null

  useEffect(() => {
    titleInputRef.current?.focus()
  }, [])

  useEffect(() => {
    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === 'Escape') {
        onCancel()
      }
    }
    document.addEventListener('keydown', handleKeyDown)
    return () => document.removeEventListener('keydown', handleKeyDown)
  }, [onCancel])

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()

    const validationErrors = validateForm(title, description, dueDate, isEditing)
    setErrors(validationErrors)

    if (Object.keys(validationErrors).length > 0) {
      return
    }

    setSubmitting(true)
    setSubmitError(null)

    try {
      const trimmedTitle = title.trim()
      const trimmedDescription = description.trim() || undefined
      const dueDateValue = dueDate ? new Date(dueDate).toISOString() : undefined

      if (isEditing) {
        const request: UpdateToDoItemRequest = {
          title: trimmedTitle,
          description: trimmedDescription,
          priority,
          status,
          dueDate: dueDateValue,
        }
        await onSubmit(request)
      } else {
        const request: CreateToDoItemRequest = {
          title: trimmedTitle,
          description: trimmedDescription,
          priority,
          dueDate: dueDateValue,
        }
        await onSubmit(request)
      }
    } catch (err) {
      setSubmitError('Failed to save todo. Please try again.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="form-overlay" onClick={onCancel} role="dialog" aria-modal="true" aria-label={isEditing ? 'Edit Todo' : 'Add Todo'}>
      <div className="form-dialog" onClick={e => e.stopPropagation()}>
        <h2 className="form-title">{isEditing ? 'Edit Todo' : 'Add New Todo'}</h2>

        {submitError && <div className="form-error-banner">{submitError}</div>}

        <form onSubmit={handleSubmit} noValidate>
          <div className="form-field">
            <label htmlFor="todo-title" className="form-label">
              Title <span className="required">*</span>
            </label>
            <input
              ref={titleInputRef}
              id="todo-title"
              type="text"
              className={`form-input ${errors.title ? 'input-error' : ''}`}
              value={title}
              onChange={e => setTitle(e.target.value)}
              maxLength={TITLE_MAX_LENGTH}
              required
            />
            {errors.title && <span className="field-error">{errors.title}</span>}
            <span className="char-count">{title.length}/{TITLE_MAX_LENGTH}</span>
          </div>

          <div className="form-field">
            <label htmlFor="todo-description" className="form-label">Description</label>
            <textarea
              id="todo-description"
              className={`form-input form-textarea ${errors.description ? 'input-error' : ''}`}
              value={description}
              onChange={e => setDescription(e.target.value)}
              maxLength={DESCRIPTION_MAX_LENGTH}
              rows={3}
            />
            {errors.description && <span className="field-error">{errors.description}</span>}
            <span className="char-count">{description.length}/{DESCRIPTION_MAX_LENGTH}</span>
          </div>

          <div className="form-row">
            <div className="form-field">
              <label htmlFor="todo-priority" className="form-label">Priority</label>
              <select
                id="todo-priority"
                className="form-input"
                value={priority}
                onChange={e => setPriority(Number(e.target.value) as Priority)}
              >
                {ALL_PRIORITIES.map(p => (
                  <option key={p} value={p}>{getPriorityLabel(p)}</option>
                ))}
              </select>
            </div>

            {isEditing && (
              <div className="form-field">
                <label htmlFor="todo-status" className="form-label">Status</label>
                <select
                  id="todo-status"
                  className="form-input"
                  value={status}
                  onChange={e => setStatus(Number(e.target.value) as ToDoItemStatus)}
                >
                  {ALL_STATUSES.map(s => (
                    <option key={s} value={s}>{getStatusLabel(s)}</option>
                  ))}
                </select>
              </div>
            )}
          </div>

          <div className="form-field">
            <label htmlFor="todo-due-date" className="form-label">Due Date</label>
            <input
              id="todo-due-date"
              type="datetime-local"
              className={`form-input ${errors.dueDate ? 'input-error' : ''}`}
              value={dueDate}
              onChange={e => setDueDate(e.target.value)}
            />
            {errors.dueDate && <span className="field-error">{errors.dueDate}</span>}
          </div>

          <div className="form-actions">
            <button type="button" className="btn btn-secondary" onClick={onCancel} disabled={submitting}>
              Cancel
            </button>
            <button type="submit" className="btn btn-primary" disabled={submitting}>
              {submitting ? 'Saving...' : isEditing ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
