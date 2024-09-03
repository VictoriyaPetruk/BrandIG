// src/App.js
import React from 'react';
import { CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import ChatBot from './ChatBot';

const theme = createTheme();

const App = () => {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <ChatBot />
    </ThemeProvider>
  );
};

export default App;
