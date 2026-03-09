export const Priority = {
  Low: 0,
  Medium: 1,
  High: 2,
  Critical: 3,
} as const

export type Priority = (typeof Priority)[keyof typeof Priority]

export const ToDoItemStatus = {
  Pending: 0,
  InProgress: 1,
  Completed: 2,
  Cancelled: 3,
} as const

export type ToDoItemStatus = (typeof ToDoItemStatus)[keyof typeof ToDoItemStatus]

export interface ToDoItem {
  readonly id: number
  readonly title: string
  readonly description: string | null
  readonly priority: Priority
  readonly status: ToDoItemStatus
  readonly createdAtUtc: string
  readonly updatedAtUtc: string | null
  readonly dueDate: string | null
  readonly completedAtUtc: string | null
}

export interface CreateToDoItemRequest {
  readonly title: string
  readonly description?: string
  readonly priority: Priority
  readonly dueDate?: string
}

export interface UpdateToDoItemRequest {
  readonly title: string
  readonly description?: string
  readonly priority: Priority
  readonly status: ToDoItemStatus
  readonly dueDate?: string
}

export interface ApiResponse<T> {
  readonly success: boolean
  readonly data?: T
  readonly error?: string
  readonly statusCode: number
}

const PRIORITY_LABELS: Record<Priority, string> = {
  [Priority.Low]: 'Low',
  [Priority.Medium]: 'Medium',
  [Priority.High]: 'High',
  [Priority.Critical]: 'Critical',
}

const STATUS_LABELS: Record<ToDoItemStatus, string> = {
  [ToDoItemStatus.Pending]: 'Pending',
  [ToDoItemStatus.InProgress]: 'In Progress',
  [ToDoItemStatus.Completed]: 'Completed',
  [ToDoItemStatus.Cancelled]: 'Cancelled',
}

export const ALL_PRIORITIES = [Priority.Low, Priority.Medium, Priority.High, Priority.Critical] as const
export const ALL_STATUSES = [ToDoItemStatus.Pending, ToDoItemStatus.InProgress, ToDoItemStatus.Completed, ToDoItemStatus.Cancelled] as const

export function getPriorityLabel(priority: Priority): string {
  return PRIORITY_LABELS[priority]
}

export function getStatusLabel(status: ToDoItemStatus): string {
  return STATUS_LABELS[status]
}
