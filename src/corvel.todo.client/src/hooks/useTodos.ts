import { useCallback, useEffect, useState } from 'react'
import * as todoApiService from '../services/todoApiService'
import type { CreateToDoItemRequest, ToDoItem, UpdateToDoItemRequest } from '../types/todo'

interface UseTodosResult {
  readonly todos: readonly ToDoItem[]
  readonly loading: boolean
  readonly error: string | null
  fetchTodos: () => Promise<void>
  addTodo: (request: CreateToDoItemRequest) => Promise<ToDoItem>
  editTodo: (id: number, request: UpdateToDoItemRequest) => Promise<ToDoItem>
  removeTodo: (id: number) => Promise<void>
}

export function useTodos(): UseTodosResult {
  const [todos, setTodos] = useState<readonly ToDoItem[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const fetchTodos = useCallback(async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await todoApiService.getAllTodos()
      setTodos(data)
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to fetch todos'
      setError(message)
    } finally {
      setLoading(false)
    }
  }, [])

  const addTodo = useCallback(async (request: CreateToDoItemRequest): Promise<ToDoItem> => {
    const created = await todoApiService.createTodo(request)
    setTodos(prev => [...prev, created])
    return created
  }, [])

  const editTodo = useCallback(async (id: number, request: UpdateToDoItemRequest): Promise<ToDoItem> => {
    const updated = await todoApiService.updateTodo(id, request)
    setTodos(prev => prev.map(todo => (todo.id === id ? updated : todo)))
    return updated
  }, [])

  const removeTodo = useCallback(async (id: number): Promise<void> => {
    await todoApiService.deleteTodo(id)
    setTodos(prev => prev.filter(todo => todo.id !== id))
  }, [])

  useEffect(() => {
    void fetchTodos()
  }, [fetchTodos])

  return { todos, loading, error, fetchTodos, addTodo, editTodo, removeTodo }
}
