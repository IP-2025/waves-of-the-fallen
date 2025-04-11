import request from 'supertest';
import app from '../src/app'; // Adjust the path to your app file

describe('POST /auth/register', () => {
  it('should return 400 if username or password is missing', async () => {
    const response = await request(app)
      .post('/api/v1/auth/register') // Adjust the route prefix if necessary
      .send({ username: 'testUser' }); // Missing password
    expect(response.status).toBe(400);
    expect(response.body).toEqual({ error: 'Username or Password is required' });
  });
});

describe('POST /auth/login', () => {
  it('should return 201 if user is registered properly', async () => {
    const  response = await request(app)
    .post('/api/v1/auth/register')
    .send({ username: 'testUser', password: 'testPassword' });
    expect(response.status).toBe(201);
    expect(response.body).toEqual({message: 'User testUser registered'});
  });

})
