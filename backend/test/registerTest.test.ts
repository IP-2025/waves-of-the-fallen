import app from '../src/app'; // Adjust the path to your app file
import { insertNewCred } from '../src/repositories/credentialsRepository';
import { getPrismaClient } from '../src/libs';
import { v4 as uuidv4 } from 'uuid';
import { Credential } from '../src/types/databaseEntries';

const prisma = getPrismaClient();

// describe('POST /auth/register', () => {
//   it('should return 400 if username or password is missing', async () => {
//     const response = await request(app)
//       .post('/api/v1/auth/register') // Adjust the route prefix if necessary
//       .send({ username: 'testUser' }); // Missing password
//     expect(response.status).toBe(400);
//     expect(response.body).toEqual({ error: 'Username or Password is required' });
//   });
// });
//
// describe('POST /auth/login', () => {
//   it('should return 201 if user is registered properly', async () => {
//     const response = await request(app)
//       .post('/api/v1/auth/register')
//       .send({ username: 'testUser', password: 'testPassword' });
//     expect(response.status).toBe(201);
//     expect(response.body).toEqual({ message: 'User testUser registered' });
//   });
// });

describe('Check insertNewUser', () => {
  it('should insert a new user', async () => {
    const email = 'maxmustermann@gmail.com';
    const password = 'XYZ123';
    const playerId = uuidv4();

    const newUser = await insertNewCred({ player_id: playerId, email: email, password: password });

    expect(newUser).toBeDefined();
    expect(newUser.email).toBe(email);
    expect(newUser.player_id).toBeDefined();
    expect(newUser.created_at).toBeInstanceOf(Date);

    const savedUsers= await prisma.$queryRawUnsafe<Credential[]>(`SELECT * FROM credentials WHERE email = '${email}'`);
    const savedUser = savedUsers[0];

    expect(savedUser).toBeDefined();
    expect(savedUser?.email).toBe(newUser.email);
    expect(savedUser?.player_id).toBe(newUser.player_id);
    expect(savedUser?.password).toBe(newUser.password);
    expect(savedUser?.created_at).toBeInstanceOf(Date);
    expect(savedUser?.created_at).toBe(newUser.created_at);

  });
});

