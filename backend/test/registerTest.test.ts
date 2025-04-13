import app from '../src/app'; // Adjust the path to your app file
import { v4 as uuidv4 } from 'uuid';
import { Credential } from '../src/types/databaseEntries';
import { createNewPlayer, NewPlayer } from '../src/repositories/usersRepository';
import { AppDataSource } from '../src/libs/data-source';
import { Player } from '../src/libs/entities/Player';


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
    const newPlayer: NewPlayer = {
      player_id: uuidv4(), // Generate a valid UUID
      username: 'testuser',
    };

    const createdPlayer = await createNewPlayer(newPlayer);

    // Verify that the player was inserted correctly
    const foundPlayer = await AppDataSource.getRepository(Player).findOneBy({ player_id: newPlayer.player_id });

    expect(foundPlayer).not.toBeNull();
    expect(foundPlayer?.player_id).toBe(newPlayer.player_id);
    expect(foundPlayer?.username).toBe(newPlayer.username);
  });
});
