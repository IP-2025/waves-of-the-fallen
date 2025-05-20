import request from 'supertest';
import app from '../../src/app';
import { UnlockedCharacter } from '../../src/database/entities';
import { AppDataSource } from '../../src/database/dataSource';

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

  const unlockedCharacter = AppDataSource.getRepository(UnlockedCharacter).create({
    player: { player_id: registeredPlayerId },
    character_id: 1,
    level: 1,
  });
  await AppDataSource.getRepository(UnlockedCharacter).save(unlockedCharacter);

});

describe('POST /getAllUnlockedCharacters', () => {
  it('should return all Unlocked Characters', async () => {
    const AllCharacters = await request(app)
      .post('/api/v1/protected/getAllUnlockedCharacters')
      .set('Authorization', `Bearer ${validToken}`);
    expect(AllCharacters.status).toBe(200);

    const relevantFields = AllCharacters.body.unlocked_characters.map((char: any) => ({
      character_id: char.character_id,
      level: char.level,
    }));

    expect(relevantFields).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          character_id: 1,
          level: 1,
        }),
      ]),
    );
  });
});

describe('POST /levelUp', () => {
  it('should levelUp specific Character for Player', async () => {

    const unlockResponse = await request(app)
      .post('/api/v1/protected/character/unlock')
      .set('Authorization', `Bearer ${validToken}`)
      .send({ character_id: 3 });
    expect(unlockResponse.status).toBe(200);

    let levelUpResponse = await request(app)
      .post('/api/v1/protected/character/levelUp')
      .set('Authorization', `Bearer ${validToken}`)
      .send({ character_id: 3 });
    expect(levelUpResponse.status).toBe(200);

    levelUpResponse = await request(app)
      .post('/api/v1/protected/character/levelUp')
      .set('Authorization', `Bearer ${validToken}`)
      .send({ character_id: 1 });
    expect(levelUpResponse.status).toBe(200);

    const AllCharacters = await request(app)
      .post('/api/v1/protected/getAllUnlockedCharacters')
      .set('Authorization', `Bearer ${validToken}`);
    expect(AllCharacters.status).toBe(200);

    const relevantFields = AllCharacters.body.unlocked_characters.map((char: any) => ({
      character_id: char.character_id,
      level: char.level,
    }));

    expect(relevantFields).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          character_id: 1,
          level: 2,
        }),
        expect.objectContaining({
          character_id: 3,
          level: 2,
        }),
      ]),
    );
  });
});


describe('POST /unlock', () => {
  it('should unlock specific Character for Player', async () => {

    const character_id: number = 3;

    const unlockResponse = await request(app)
      .post('/api/v1/protected/character/unlock')
      .set('Authorization', `Bearer ${validToken}`)
      .send({ character_id });
    expect(unlockResponse.status).toBe(200);

    const AllCharacters = await request(app)
      .post('/api/v1/protected/getAllUnlockedCharacters')
      .set('Authorization', `Bearer ${validToken}`);
    expect(AllCharacters.status).toBe(200);

    const relevantFields = AllCharacters.body.unlocked_characters.map((char: any) => ({
      character_id: char.character_id,
      level: char.level,
    }));
    expect(relevantFields).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          character_id: 1,
          level: 1,
        }),
        expect.objectContaining({
          character_id: 3,
          level: 1,
        }),
      ]),
    );
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