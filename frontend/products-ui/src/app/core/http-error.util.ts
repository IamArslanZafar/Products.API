import { HttpErrorResponse } from '@angular/common/http';

/**
 * Extracts a human-readable message from an API error response.
 * Handles ASP.NET Core's ValidationProblemDetails shape ({ errors: { Field: string[] } }),
 * the plain { message } shape used by the login endpoint, and falls back to `.title`.
 */
export function extractErrorMessage(err: HttpErrorResponse, fallback: string): string {
  const body = err.error;

  if (body && typeof body === 'object') {
    if (body.errors && typeof body.errors === 'object') {
      const messages = Object.values(body.errors as Record<string, string[]>).flat();
      if (messages.length > 0) {
        return messages.join(' ');
      }
    }

    if (typeof body.message === 'string' && body.message.trim()) {
      return body.message;
    }

    if (typeof body.title === 'string' && body.title.trim()) {
      return body.title;
    }
  }

  return fallback;
}
