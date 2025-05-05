import request from 'supertest';
import app from '../../src/app';
import { AppDataSource } from '../../src/libs/data-source';
import { UnlockedCharacter } from '../../src/libs/entities/UnlockedCharacter';

const generateTestUser = () => {
  return {
    username: `TestUser`,
    email: `testuser@example.com`,
    password: '123456',
  };
};

let validToken: string;
let registeredPlayerId: string;
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

  await AppDataSource.getRepository(UnlockedCharacter).insert({
    player_id: registeredPlayerId,
    character_id: '1',
    level: 1,
  });

});

describe('POST /setGold', () => {
  it('should return set the amount of Gold', async () => {
    const gold = 10;
    const param = {
      player_id: registeredPlayerId,
      gold: gold,
    };
    const coinsResponse = await request(app)
      .post('/api/v1/protected/setGold')
      .send(param)
      .set('Authorization', `Bearer ${validToken}`);


    expect(coinsResponse.status).toBe(200);
    expect(coinsResponse.body).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          gold: 0,
        }),
      ]),
    );

  });
});

describe('POST /getGold', () => {
  it('should return the amount of Gold', async () => {
    const coinsResponse = await request(app)
      .post('/api/v1/protected/getGold')
      .send(registeredPlayerId)
      .set('Authorization', `Bearer ${validToken}`);
    expect(coinsResponse.status).toBe(200);
    expect(coinsResponse.body).toEqual(0);
  });
});

