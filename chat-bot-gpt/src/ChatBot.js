// src/ChatBot.js
import React, { useState } from 'react';
import { Box, Button, Container, Grid, Paper, TextField, Typography } from '@mui/material';
import SendIcon from '@mui/icons-material/Send';

const ChatBot = () => {
  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState('');
  const getChatGptResponse = async (input) => {
    try {
      const response = await fetch('/api/chat/send', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Message: input })
      });

      if (response.ok) {
        const data = await response.json();
        return data.choices[0].message.content;
      } else {
        const errorText = await response.text();
        console.error('Error:', errorText);
        return 'Sorry, something went wrong.';
      }
    } catch (error) {
      console.error('Error:', error);
      return 'Sorry, something went wrong.';
    }
  };
  const handleSend = async () => {
    if (input.trim()) {
        const userMessage = { text: input, sender: 'user' };
        setMessages([...messages, userMessage]);
  
        const botResponse = await getChatGptResponse(input);
        const botMessage = { text: botResponse, sender: 'bot' };
        setMessages([...messages, userMessage, botMessage]);
  
        setInput('');
    }
  };

  return (
    <Container maxWidth="sm">
      <Typography variant="h4" align="center" gutterBottom>
        Chat Bot
      </Typography>
      <Paper elevation={3} style={{ height: '70vh', overflow: 'auto', padding: '10px' }}>
        <Box display="flex" flexDirection="column" justifyContent="flex-end" height="100%">
          {messages.map((message, index) => (
            <Box
              key={index}
              display="flex"
              justifyContent={message.sender === 'user' ? 'flex-end' : 'flex-start'}
              mb={1}
            >
              <Paper style={{ padding: '10px', backgroundColor: message.sender === 'user' ? '#e1f5fe' : '#fff9c4' }}>
                <Typography>{message.text}</Typography>
              </Paper>
            </Box>
          ))}
        </Box>
      </Paper>
      <Grid container spacing={2} alignItems="center" style={{ marginTop: '10px' }}>
        <Grid item xs={10}>
          <TextField
            fullWidth
            variant="outlined"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            placeholder="Type your message..."
          />
        </Grid>
        <Grid item xs={2}>
          <Button variant="contained" color="primary" endIcon={<SendIcon />} onClick={handleSend}>
            Send
          </Button>
        </Grid>
      </Grid>
    </Container>
  );
};

export default ChatBot;
