import request from 'supertest';
import app from '../../src/app';
import { AppDataSource } from '../../src/libs/data-source';
import { Player } from '../../src/libs/entities/Player';

describe('Test POST /register', () => {
  beforeAll(async () => {
    if (!AppDataSource.isInitialized) {
      await AppDataSource.initialize();
    }
  });

  afterAll(async () => {
    if (AppDataSource.isInitialized) {
      await AppDataSource.destroy();
    }
  });

  it('should insert a new user and retrieve it from the database', async () => {
    const userData = {
      username: 'MaxMustermann',
      email: 'MaxMustermann@gmail.com',
      password: '123456',
    };

    // Send the registration request
    const response = await request(app)
      .post('/api/v1/auth/register')
      .send(userData);

    expect(response.status).toBe(201);
    expect(response.body).toEqual({ message: `User ${userData.username} registered` });

    // Retrieve the user from the database
    const playerRepo = AppDataSource.getRepository(Player);
    const user = await playerRepo.findOneBy({ username: userData.username });

    expect(user).not.toBeNull();
    expect(user?.username).toBe(userData.username);
  });
});