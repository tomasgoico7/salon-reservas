import { alpha } from '@mui/material';

const ACCENT_COLORS = ['#7048D4', '#E8562A', '#2A7A4B', '#B8860B', '#1A6FAF', '#9C3587'];

export function roomColor(name: string): string {
  let hash = 0;
  for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash);
  return ACCENT_COLORS[Math.abs(hash) % ACCENT_COLORS.length];
}

export function roomColorAlpha(name: string, opacity: number): string {
  return alpha(roomColor(name), opacity);
}
