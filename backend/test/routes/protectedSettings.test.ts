import request from 'supertest';
import app from '../../src/app';

const generateTestUser = () => {
  const timestamp = Date.now();
  return {
    username: `TestUser_${timestamp}`,
    email: `testuser_${timestamp}@example.com`,
    password: '123456',
  };
};

let validToken: string;
let registeredPlayerId: string;

beforeEach(async () => {
  const testUser = generateTestUser();

  // Register new user
  const registerResponse = await request(app)
    .post('/api/v1/auth/register')
    .send(testUser);

  expect(registerResponse.status).toBe(201); // Correct status for resource creation
  registeredPlayerId = registerResponse.body.player_id;

  // Login with new user
  const loginResponse = await request(app)
    .post('/api/v1/auth/login')
    .send({
      email: testUser.email,
      password: testUser.password,
    });

  expect(loginResponse.status).toBe(200); // Correct status for login
  validToken = loginResponse.body.token;
});

describe('User Settings API', () => {
  const invalidToken =
    'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyIjoiZmFrZSIsImlhdCI6MH0.invalidsignature';

  describe('POST /setSettings', () => {
    it('should insert settings successfully', async () => {
      const settingsData = {
        player_id: registeredPlayerId,
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

    it('should fail with invalid token', async () => {
      const settingsData = {
        player_id: registeredPlayerId,
        musicVolume: 0.5,
        soundVolume: 0.2,
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

  describe('POST /getSettings', () => {
    it('should retrieve user settings successfully', async () => {
      const settingsData = {
        player_id: registeredPlayerId,
        musicVolume: 0.5,
        soundVolume: 0.2,
      };

      // First, set the settings
      const responseSet = await request(app)
        .post('/api/v1/protected/setSettings')
        .set('Authorization', `Bearer ${validToken}`)
        .send(settingsData);

      expect(responseSet.status).toBe(200);

      // Then, retrieve the settings
      const responseGet = await request(app)
        .post('/api/v1/protected/getSettings')
        .set('Authorization', `Bearer ${validToken}`)
        .send({ player_id: registeredPlayerId });

      expect(responseGet.status).toBe(200);
      expect(responseGet.body).toEqual(settingsData);
    });
  });
});
