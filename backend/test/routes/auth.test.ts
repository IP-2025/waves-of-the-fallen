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
const wrongCredentials = {
  email: 'MaxMustermann@gmail.com',
  password: 'XYZ123456',
};

describe('Test POST /register', () => {
  it('should insert a new user and retrieve it from the database', async () => {
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

    const errorLoginResponse = await request(app)
      .post('/api/v1/auth/login')
      .send(wrongCredentials);

    expect(errorLoginResponse.status).toBe(404);
    expect(errorLoginResponse.body).toEqual({
        message: `Invalid password.`,
        status: 'error'
      }
    );
    expect(loginResponse.status).toBe(200);
    expect(loginResponse.body).toEqual({
      message: `Login successful for ${userData.email}`,
      token: expect.any(String),
    });
  });
});


describe('Test protected routes with token', () => {
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

  it('should access a protected route with a valid token', async () => {
    // Register a new user
    await request(app)
      .post('/api/v1/auth/register')
      .send(userData);

    // Log in to get the token
    const loginResponse = await request(app)
      .post('/api/v1/auth/login')
      .send(userCredentials);

    const token = loginResponse.body.token;

    // Access the protected route
    const protectedResponse = await request(app)
      .get('/api/v1/protected')
      .set('Authorization', `Bearer ${token}`);

    expect(protectedResponse.status).toBe(200);
    expect(protectedResponse.body).toEqual({ authenticated: true });
  });

  it('should fail to access a protected route with an invalid token', async () => {
    const invalidToken = 'invalid.token.here';

    const protectedResponse = await request(app)
      .get('/api/v1/protected')
      .set('Authorization', `Bearer ${invalidToken}`);

    expect(protectedResponse.status).toBe(401);
    expect(protectedResponse.body).toEqual({
      message: 'Unauthorized',
      status: 'error',
    });
  });
});


