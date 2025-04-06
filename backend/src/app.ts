import express from 'express';
import dotenv from 'dotenv';
import healthRouter from './routes/health';
import authRouter from './routes/auth';

dotenv.config();

const app = express();

app.use(express.json());

app.use('/api/healthz', healthRouter);
app.use('/api/auth', authRouter);

export default app;
