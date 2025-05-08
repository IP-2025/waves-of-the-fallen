import request from 'supertest';
import app from '../../src/app';
import { AppDataSource } from '../../src/database/dataSource';
import { Player } from '../../src/database/entities/Player';

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
    expect(response.body).toMatchObject({ message: `User ${userData.username} registered` });
    expect(response.body).toHaveProperty('player_id');
    expect(typeof response.body.player_id).toBe('string');

    const playerRepo = AppDataSource.getRepository(Player);
    const user = await playerRepo.findOneBy({ username: userData.username });

    expect(user).not.toBeNull();
    expect(user?.username).toBe(userData.username);
  });
});

describe('Test POST /login', () => {
  it('should login a registered user and fail for invalid credentials', async () => {
    // Register a new user
    const registrateResponse = await request(app)
      .post('/api/v1/auth/register')
      .send(userData);

    expect(registrateResponse.status).toBe(201);
    expect(registrateResponse.body).toHaveProperty('player_id');

    // Login with valid credentials
    const loginResponse = await request(app)
      .post('/api/v1/auth/login')
      .send(userCredentials);

    expect(loginResponse.status).toBe(200);
    expect(loginResponse.body).toMatchObject({
      message: `Login successful for ${userData.email}`,
    });
    expect(loginResponse.body).toHaveProperty('token');
    expect(typeof loginResponse.body.token).toBe('string');

    // Login with invalid credentials
    const errorLoginResponse = await request(app)
      .post('/api/v1/auth/login')
      .send(wrongCredentials);

    expect(errorLoginResponse.status).toBe(401);
    expect(errorLoginResponse.body).toMatchObject({
      message: 'Invalid password.',
      status: 'error',
    });
  });
});



describe('Test protected routes with token', () => {
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
