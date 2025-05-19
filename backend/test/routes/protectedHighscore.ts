import request from "supertest";
import app from "../../src/app";

const randomString = (length = 6) =>
    Math.random().toString(36).substring(2, 2 + length);

const generateTestUser = () => {
    const suffix = randomString();
    return {
        username: `TestUser_${suffix}`,
        email: `testuser_${suffix}@example.com`,
        password: '123456',
    };
};

let validToken1: string;
let validToken2: string;


beforeEach(async () => {
    const [user1, user2] = [generateTestUser(), generateTestUser()];

    //User 1
    const registerResponse = await request(app)
        .post('/api/v1/auth/register')
        .send(user1);

    expect(registerResponse.status).toBe(201); // Correct status for resource creation

    const loginResponse = await request(app)
        .post('/api/v1/auth/login')
        .send({
            email: user1.email,
            password: user1.password,
        });

    expect(loginResponse.status).toBe(200); // Correct status for login
    validToken1 = loginResponse.body.token;

    //User 2

    const registerResponse1 = await request(app)
        .post('/api/v1/auth/register')
        .send(user2);

    expect(registerResponse1.status).toBe(201);

    const loginResponse1 = await request(app)
        .post('/api/v1/auth/login')
        .send({
            email: user2.email,
            password: user2.password,
        });

    expect(loginResponse1.status).toBe(200);
    validToken2 = loginResponse.body.token;
});



describe('POST /update', () => {
    it('should update Highscore', async () => {
        const updateResponse = await request(app)
            .post('/api/v1/highscore/update')
            .set('Authorization', `Bearer ${validToken1}`)
            .send({score:100});
        expect(updateResponse.status).toBe(200);
    });

    it('should get User Highscore', async () => {
        const getHighscoreResponse = await request(app)
            .post('/api/v1/highscore/getUserHighscore')
            .set('Authorization', `Bearer ${validToken1}`)
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


describe('POST /top', () => {
    it('should update Highscore', async () => {
        const updateResponse = await request(app)
            .post('/api/v1/highscore/update')
            .set('Authorization', `Bearer ${validToken1}`)
            .send({score:200});
        expect(updateResponse.status).toBe(200);
    });

    it('should update Highscore', async () => {
        const updateResponse = await request(app)
            .post('/api/v1/highscore/update')
            .set('Authorization', `Bearer ${validToken2}`)
            .send({score:100});
        expect(updateResponse.status).toBe(200);
    });

    it('should return the top highscores for the two seeded players', async () => {
        const res = await request(app)
            .post('/api/v1/highscore/top')
            .set('Authorization', `Bearer ${validToken1}`);
        expect(res.status).toBe(200);
        expect(Array.isArray(res.body)).toBe(true);
        expect(res.body).toHaveLength(2);

        res.body.forEach((entry: any) => {
            expect(entry).toEqual(
                expect.objectContaining({
                    id: expect.any(Number),
                    player: expect.objectContaining({
                        player_id: expect.any(String),
                    }),
                    highScore: expect.any(Number),
                    timeStamp: expect.any(String),
                })
            );
        });

        expect(res.body).toEqual(
            expect.arrayContaining([
                expect.objectContaining({
                    highScore: 100,
                }),
                expect.objectContaining({
                    highScore: 80,
                }),
            ])
        );
    });

});


