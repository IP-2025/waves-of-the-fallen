import request from "supertest";
import app from "../../src/app";

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
});

describe('POST /update', () => {
    it('should update Highscore', async () => {
        const updateResponse = await request(app)
            .post('/api/v1/highscore/update')
            .set('Authorization', `Bearer ${validToken}`)
            .send({score:100});
        expect(updateResponse.status).toBe(200);
    });

    it('should get User Highscore', async () => {
        const getHighscoreResponse = await request(app)
            .post('/api/v1/highscore/getUserHighscore')
            .set('Authorization', `Bearer ${validToken}`)
        expect(getHighscoreResponse.status).toBe(200);
        const relevantFields = getHighscoreResponse.body.unlocked_characters.map((char: any) => ({
            highScore: char.character_id,
        }));

        expect(relevantFields).toEqual(
            expect.arrayContaining([
                expect.objectContaining({
                    highScore:100,
                }),
            ])
        );
    });
});


