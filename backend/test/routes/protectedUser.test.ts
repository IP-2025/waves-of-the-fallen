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

beforeEach(async () => {
    const testUser = generateTestUser();

    const registerResponse = await request(app)
        .post('/api/v1/auth/register')
        .send(testUser);

    expect(registerResponse.status).toBe(201); // Correct status for resource creation

    const loginResponse = await request(app)
        .post('/api/v1/auth/login')
        .send({
            email: testUser.email,
            password: testUser.password,
        });

    expect(loginResponse.status).toBe(200); // Correct status for login
    validToken = loginResponse.body.token;
});


describe('DELETE user', () => {
    it('should delete the user', async () => {

        const response = await request(app)
            .delete('/api/v1/protected/user')
            .set('Authorization', `Bearer ${validToken}`)

        expect(response.status).toBe(200);
    });
});

