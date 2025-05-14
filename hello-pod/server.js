import express from 'express';
import cors from 'cors';
import process from 'node:process';

const app = express();
const PORT = 3000;
const CODE = process.env.CODE ?? 'default-code';

// Middleware
app.use(cors());
app.use(express.json());

app.get('/', (req, res) => {
    res.send(`Hello from Game ${CODE}`)
})

// Start server
app.listen(PORT, async () => {
    console.log(`🚀 Pod server screaming quietly on http://localhost:${PORT}`);
});