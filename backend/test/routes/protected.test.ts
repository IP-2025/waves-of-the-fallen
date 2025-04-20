import request from 'supertest';
import app from '../../src/app';


const userData = {
  username: 'MaxMustermann',
  email: 'MaxMustermann@gmail.com',
  password: '123456',
};

const userCredentials = {
  email: 'MaxMustermann@gmail.com',
  password: '123456',
};

describe('Settings API Tests', () => {
  let validToken: string;
  const invalidToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyIjoiZmFrZSIsImlhdCI6MH0.invalidsignature';

  beforeAll(async () => {

    // Register a user
    await request(app).post('/api/v1/auth/register').send(userData);

    // Log in and save token
    const loginResponse = await request(app)
      .post('/api/v1/auth/login')
      .send(userCredentials);

    validToken = loginResponse.body.token;
  });
// TODO Error handling Value Should be between 0 and 1
  it('should insert settings', async () => {
    const settingsData = {
      player_id: userData.username,
      musicVolume: 0.5,
      soundVolume: 0.2,
    };

    const response = await request(app)
      .post('/api/v1/protected/setSettings')
      .set('Authorization', `Bearer ${validToken}`)
      .send(settingsData);

    expect(response.status).toBe(200);
    expect(response.body).toEqual(settingsData);
  });


// TODO fix error handling Unauthorized
  it('should fail with missing fields', async () => {
    const incompleteSettings = {
      player_id: userData.username,
      musicVolume: 50,
    };

    const response = await request(app)
      .post('/api/v1/protected/setSettings')
      .set('Authorization', `Bearer ${validToken}`)
      .send(incompleteSettings);

    expect(response.status).toBe(401);
    expect(response.body).toEqual({
      message: 'Unauthorized',
      status: 'error',
    });
  });

  it('should fail with invalid token', async () => {
    const settingsData = {
      player_id: userData.username,
      musicVolume: 50,
      soundVolume: 70,
    };

    const response = await request(app)
      .post('/api/v1/protected/setSettings')
      .set('Authorization', `Bearer ${invalidToken}`)
      .send(settingsData);

    expect(response.status).toBe(401);
    expect(response.body).toEqual({
      message: 'Unauthorized',
      status: 'error',
    });
  });
});
