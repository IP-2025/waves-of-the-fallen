import express from 'express';
import dotenv from 'dotenv';
import registerRoutes from './routes';
import { errorHandler } from './middleware/errorHandler';

dotenv.config();

const app = express();

app.use(express.json());

registerRoutes(app);

app.use(errorHandler);

export default app;