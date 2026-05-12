export function formatTime(hhmmss: string): string {
  return hhmmss.substring(0, 5);
}

export function generateSlots(fromMin: number, toMin: number, stepMin = 30): string[] {
  const slots: string[] = [];
  for (let m = fromMin; m <= toMin; m += stepMin) {
    const h = Math.floor(m / 60).toString().padStart(2, '0');
    const min = (m % 60).toString().padStart(2, '0');
    slots.push(`${h}:${min}`);
  }
  return slots;
}

export const START_SLOTS = generateSlots(9 * 60, 17 * 60 + 30);
