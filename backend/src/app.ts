import express from 'express';
import dotenv from 'dotenv';
import healthRouter from './routes/health';
import authRouter from './routes/auth';
import { PREFIX_ROUTE } from './core/url';
import { errorHandler } from './middleware/errorHandler';

dotenv.config();

const app = express();

app.use(express.json());

app.use(`${PREFIX_ROUTE}/healthz`, healthRouter);
app.use(`${PREFIX_ROUTE}/auth`, authRouter);

app.use(errorHandler);

export default app;
