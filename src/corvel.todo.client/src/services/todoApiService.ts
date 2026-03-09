import { z } from 'zod'
import type { CreateToDoItemRequest, ToDoItem, UpdateToDoItemRequest } from '../types/todo'

const BASE_PATH = '/api/todo-items'

const apiResponseSchema = z.object({
  success: z.boolean(),
  data: z.unknown().optional(),
  error: z.string().optional(),
  statusCode: z.number(),
})

async function handleResponse<T>(response: Response): Promise<T> {
  const json: unknown = await response.json()
  const body = apiResponseSchema.parse(json)

  if (!body.success) {
    throw new Error(body.error ?? `Request failed with status ${body.statusCode}`)
  }

  if (body.data === undefined || body.data === null) {
    throw new Error('Response data is missing')
  }

  return body.data as T
}

export async function getAllTodos(): Promise<readonly ToDoItem[]> {
  const response = await fetch(BASE_PATH)
  return handleResponse<readonly ToDoItem[]>(response)
}

export async function getTodoById(id: number): Promise<ToDoItem> {
  const response = await fetch(`${BASE_PATH}/${id}`)
  return handleResponse<ToDoItem>(response)
}

export async function createTodo(request: CreateToDoItemRequest): Promise<ToDoItem> {
  const response = await fetch(BASE_PATH, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })
  return handleResponse<ToDoItem>(response)
}

export async function updateTodo(id: number, request: UpdateToDoItemRequest): Promise<ToDoItem> {
  const response = await fetch(`${BASE_PATH}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })
  return handleResponse<ToDoItem>(response)
}

export async function deleteTodo(id: number): Promise<void> {
  const response = await fetch(`${BASE_PATH}/${id}`, {
    method: 'DELETE',
  })

  if (response.status === 204) {
    return
  }

  const json: unknown = await response.json()
  const body = apiResponseSchema.parse(json)

  if (!body.success) {
    throw new Error(body.error ?? `Request failed with status ${body.statusCode}`)
  }
}
