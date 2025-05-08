import express from 'express';
import dotenv from 'dotenv';
import createRoutes from './routers';
import { errorHandler } from './middleware/errorMiddleware';

dotenv.config();

const app = express();

app.use(express.json());

createRoutes(app);

app.use(errorHandler);

export default app;
