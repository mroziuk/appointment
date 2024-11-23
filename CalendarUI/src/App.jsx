import { ColorModeContext, useMode } from "./theme";
import { CssBaseline, ThemeProvider } from "@mui/material";
import Routes from "./routes";
import AuthProvider from "./provider/authProvider.jsx";
import "./App.css";
function App() {
  const [theme, colorMode] = useMode();

  return (
    <ColorModeContext.Provider value={colorMode}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <main className="content">
          <AuthProvider>
            <Routes />
          </AuthProvider>
        </main>
      </ThemeProvider>
    </ColorModeContext.Provider>
  );
}

export default App;
