import type { CreateToDoItemRequest, ToDoItem, UpdateToDoItemRequest } from '../types/todo'
import { apiFetch, apiResponseSchema, handleApiResponse } from './apiClient'

const BASE_PATH = '/api/todo-items'

export async function getAllTodos(): Promise<readonly ToDoItem[]> {
  const response = await apiFetch(BASE_PATH)
  return handleApiResponse<readonly ToDoItem[]>(response)
}

export async function getTodoById(id: number): Promise<ToDoItem> {
  const response = await apiFetch(`${BASE_PATH}/${id}`)
  return handleApiResponse<ToDoItem>(response)
}

export async function createTodo(request: CreateToDoItemRequest): Promise<ToDoItem> {
  const response = await apiFetch(BASE_PATH, {
    method: 'POST',
    body: JSON.stringify(request),
  })
  return handleApiResponse<ToDoItem>(response)
}

export async function updateTodo(id: number, request: UpdateToDoItemRequest): Promise<ToDoItem> {
  const response = await apiFetch(`${BASE_PATH}/${id}`, {
    method: 'PUT',
    body: JSON.stringify(request),
  })
  return handleApiResponse<ToDoItem>(response)
}

export async function deleteTodo(id: number): Promise<void> {
  const response = await apiFetch(`${BASE_PATH}/${id}`, {
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
