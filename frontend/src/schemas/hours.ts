export const OPENING_MIN = 540;
export const CLOSING_MIN = 1080;

export function timeToMinutes(hhmm: string): number {
  const parts = hhmm.split(':');
  return parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10);
}
