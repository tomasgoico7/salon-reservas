import { useEffect, useState } from 'react';

const BASE = import.meta.env.VITE_API_BASE_URL || '/api';
// /health lives one level above /api (e.g. https://host/health)
const HEALTH_URL = BASE.replace(/\/api\/?$/, '') + '/health';

type Status = 'checking' | 'online' | 'offline';

export function useHealthCheck(intervalMs = 30_000): Status {
  const [status, setStatus] = useState<Status>('checking');

  useEffect(() => {
    let cancelled = false;

    const check = () => {
      const controller = new AbortController();
      const timer = setTimeout(() => controller.abort(), 5000);
      fetch(HEALTH_URL, { signal: controller.signal })
        .then((res) => { if (!cancelled) setStatus(res.ok ? 'online' : 'offline'); })
        .catch(() => { if (!cancelled) setStatus('offline'); })
        .finally(() => clearTimeout(timer));
    };

    check();
    const id = setInterval(check, intervalMs);
    return () => {
      cancelled = true;
      clearInterval(id);
    };
  }, [intervalMs]);

  return status;
}
