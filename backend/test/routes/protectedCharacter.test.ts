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

    //TODO: insert unlocked characters

});


describe('POST /getAllCharacters', () => {
    it('should return all Possible Charcters', async () => {


        const AllCharacters = await request(app)
            .post('/api/v1/protected/setSettings')
            //.set('Authorization', `Bearer ${validToken}`)
        expect(AllCharacters.status).toBe(200);
    });
});

describe('POST /getAllCharacters', () => {
    it('should return all unlocked Characters for a user', async () => {

        const unlockedCharacter = await request(app)
            .post('/api/v1/protected/getAllCharacters')
            //.set('Authorization', `Bearer ${validToken}`)
            .send(registeredPlayerId);
        expect(unlockedCharacter.status).toBe(200);

    });
});

