import { config } from 'dotenv';

config();

export const AppConfig = {
  PORT: process.env.PORT || '3000',
  JWT_SECRET: process.env.JWT_SECRET || 'secretkey',
};
