import AppLayout from './components/AppLayout';
import ErrorBoundary from './components/ErrorBoundary';
import HomePage from './pages/HomePage';

export default function App() {
  return (
    <ErrorBoundary>
      <AppLayout>
        <HomePage />
      </AppLayout>
    </ErrorBoundary>
  );
}
