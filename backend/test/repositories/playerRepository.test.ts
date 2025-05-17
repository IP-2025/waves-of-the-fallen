import { v4 as uuidv4 } from 'uuid';
import { createNewPlayer, NewPlayer } from '../../src/repositories/playerRepository';
import { AppDataSource } from '../../src/libs/data-source';
import { Player } from '../../src/libs/entities/Player';

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
