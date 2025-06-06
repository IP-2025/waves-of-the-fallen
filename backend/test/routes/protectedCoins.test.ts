import request from 'supertest';
import app from '../../src/app';
import { AppDataSource } from '../../src/database/dataSource';
import { UnlockedCharacter } from '../../src/database/entities/UnlockedCharacter';

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
    player: { player_id: registeredPlayerId }, // Use the relation, not raw player_id
    character_id: 1,
    level: 1,
  });

});

describe('POST gold/set', () => {
  it('should return set the amount of Gold', async () => {
    const gold = 10;
    const param = {
      player_id: registeredPlayerId,
      gold: gold,
    };
    const coinsResponse = await request(app)
      .post('/api/v1/protected/gold/set')
      .send(param)
      .set('Authorization', `Bearer ${validToken}`);
    expect(coinsResponse.status).toBe(200);
  });


  it('should return the amount of Gold', async () => {

    const gold = 10;
    const param = {
      player_id: registeredPlayerId,
      gold: gold,
    };
    await request(app)
      .post('/api/v1/protected/gold/set')
      .send(param)
      .set('Authorization', `Bearer ${validToken}`);

    const coinsResponse = await request(app)
      .post('/api/v1/protected/gold/get')
      .send(registeredPlayerId)
      .set('Authorization', `Bearer ${validToken}`);
    expect(coinsResponse.status).toBe(200);
    expect(coinsResponse.body).toEqual(10);
  });
});

describe('POST /levelUp', () => {
  it('should return 500 for invalid character_id', async () => {
    const invalidCharacterId = -1;

    const levelUpResponse = await request(app)
      .post('/api/v1/protected/character/levelUp')
      .set('Authorization', `Bearer ${validToken}`)
      .send({ character_id: invalidCharacterId });

    expect(levelUpResponse.status).toBe(500);
    expect(levelUpResponse.body).toHaveProperty('status', 'error');
    expect(levelUpResponse.body).toHaveProperty(
      'message',
      `Character with ID ${invalidCharacterId} not found for player ${registeredPlayerId}`
    );
  });
});
