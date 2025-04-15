import request from 'supertest';
import app from '../../src/app';
import { AppDataSource } from '../../src/libs/data-source';
import { Player } from '../../src/libs/entities/Player';

const userData = {
  username: 'MaxMustermann',
  email: 'MaxMustermann@gmail.com',
  password: '123456',
};
const userCredentials = {
  email: 'MaxMustermann@gmail.com',
  password: '123456',
};

describe('Test POST /register', () => {
  it('should insert a new user and retrieve it from the database', async () => {
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

describe('Test POST /login', () => {
  it('should insert a new user and login should be possible with it', async () => {

    // Send the registration request
    const registrateResponse = await request(app)
      .post('/api/v1/auth/register')
      .send(userData);

    const loginResponse = await request(app)
      .post('/api/v1/auth/login')
      .send(userCredentials);

    expect(loginResponse.status).toBe(200);
    expect(loginResponse.body).toEqual({
      message: `Login successful for ${userData.email}`,
      token: expect.any(String),
    });
  });
});


