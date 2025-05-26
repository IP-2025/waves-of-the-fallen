import request from 'supertest';
import app from '../../src/app';

const generateTestUser = () => {
  return {
    username: `TestUser`,
    email: `testuser@example.com`,
    password: '123456',
  };
};

let validToken: string;
let registeredPlayerId: string;
const invalidToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyIjoiZmFrZSIsImlhdCI6MH0.invalidsignature';

beforeEach(async () => {
  const testUser = generateTestUser();

  const registerResponse = await request(app)
    .post('/api/v1/auth/register')
    .send(testUser);

  expect(registerResponse.status).toBe(201); // Correct status for resource creation
  registeredPlayerId = registerResponse.body.player_id;


  const loginResponse = await request(app)
    .post('/api/v1/auth/login')
    .send({
      email: testUser.email,
      password: testUser.password,
    });

  expect(loginResponse.status).toBe(200); // Correct status for login
  validToken = loginResponse.body.token;
});


describe('POST settings/set', () => {
  it('should insert settings successfully', async () => {
    const settingsData = {
      player_id: registeredPlayerId,
      musicVolume: 0.5,
      soundVolume: 0.2,
    };

    const response = await request(app)
      .post('/api/v1/protected/settings/set')
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
      .post('/api/v1/protected/settings/set')
      .set('Authorization', `Bearer ${invalidToken}`)
      .send(settingsData);

    expect(response.status).toBe(401);
    expect(response.body).toEqual({
      message: 'Unauthorized',
      status: 'error',
    });
  });
});

describe('POST /settings/get', () => {
  it('should retrieve user settings successfully', async () => {
    const settingsData = {
      player_id: registeredPlayerId,
      musicVolume: 0.5,
      soundVolume: 0.2,
    };

    const responseSet = await request(app)
      .post('/api/v1/protected/settings/set')
      .set('Authorization', `Bearer ${validToken}`)
      .send(settingsData);

    expect(responseSet.status).toBe(200);

    const responseGet = await request(app)
      .post('/api/v1/protected/settings/get')
      .set('Authorization', `Bearer ${validToken}`)
      .send({ player_id: registeredPlayerId });

    expect(responseGet.status).toBe(200);
    expect(responseGet.body).toEqual(settingsData);
  });
});

