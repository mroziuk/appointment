import { Outlet } from "react-router-dom";
import Sidebar from "./Sidebar";
import Topbar from "./Topbar";
import { ErrorBoundary } from "react-error-boundary";
export default function RootLayout() {
  return (
    <div className="root-layout app">
      <header>
        <Sidebar />
      </header>
      <main className="content">
        <Topbar />
        <ErrorBoundary
          FallbackComponent={({ error, resetErrorBoundary }) => (
            <div role="alert">
              <p>Something went wrong:</p>
              <pre>{error.message}</pre>
              <button onClick={resetErrorBoundary}>Try again</button>
            </div>
          )}
        >
          <Outlet />
        </ErrorBoundary>
      </main>
    </div>
  );
}
