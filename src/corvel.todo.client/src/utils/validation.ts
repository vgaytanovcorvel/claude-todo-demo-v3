import type { ZodError } from 'zod'

/**
 * Converts Zod validation errors into a flat map of field name to first error message.
 * Only the first error per field is included.
 */
export function zodErrorsToMap<T>(error: ZodError): T {
  const errorMap: Record<string, string> = {}
  for (const issue of error.issues) {
    const field = String(issue.path[0])
    if (!errorMap[field]) {
      errorMap[field] = issue.message
    }
  }
  return errorMap as T
}
