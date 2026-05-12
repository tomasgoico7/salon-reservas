import { describe, it, expect } from 'vitest';
import { isApiError } from '../types/api';

describe('isApiError', () => {
  it('returns true for a valid ApiError shape', () => {
    expect(isApiError({ status: 400, title: 'Bad Request' })).toBe(true);
  });

  it('returns true with optional fields present', () => {
    expect(
      isApiError({
        status: 422,
        title: 'Validation failed',
        detail: 'Some detail',
        errors: { date: ['La fecha es inválida'] },
      })
    ).toBe(true);
  });

  it('returns false for null', () => {
    expect(isApiError(null)).toBe(false);
  });

  it('returns false for a string', () => {
    expect(isApiError('error')).toBe(false);
  });

  it('returns false for an object without title', () => {
    expect(isApiError({ status: 500 })).toBe(false);
  });

  it('returns false when title is not a string', () => {
    expect(isApiError({ status: 400, title: 42 })).toBe(false);
  });

  it('returns false for undefined', () => {
    expect(isApiError(undefined)).toBe(false);
  });
});
