import { v4 as uuidv4 } from 'uuid';
import { saveCredential, NewCred } from '../../src/repositories/credentialsRepository';
import { AppDataSource } from '../../src/libs/data-source';
import { Credential } from '../../src/libs/entities/Credential';
import { Player } from '../../src/libs/entities/Player';

describe('Check saveCredential', () => {
  it('should save a new credential', async () => {
    // Create a new player first
    const playerRepo = AppDataSource.getRepository(Player);
    const newPlayer = new Player();
    newPlayer.player_id = uuidv4();
    newPlayer.username = 'testuser';
    await playerRepo.save(newPlayer);

    const newCred: NewCred = {
      player_id: newPlayer.player_id,
      email: 'hashedemail@example.com',
      hashedPassword: 'hashedpassword',
    };

    const savedCredential = await saveCredential(newCred);

    // Verify that the credential was inserted correctly
    const foundCredential = await AppDataSource.getRepository(Credential).findOne({
      where: { email: newCred.email },
      relations: ['player'],
    });

    expect(foundCredential).not.toBeNull();
    expect(foundCredential?.email).toBe(newCred.email);
    expect(foundCredential?.password).toBe(newCred.hashedPassword);
    expect(foundCredential?.player.player_id).toBe(newPlayer.player_id);
  });

  it('should throw an error when saving a credential with an existing email', async () => {
    // Create a new player first
    const playerRepo = AppDataSource.getRepository(Player);
    const newPlayer = new Player();
    newPlayer.player_id = uuidv4();
    newPlayer.username = 'testuser';
    await playerRepo.save(newPlayer);

    const newCred: NewCred = {
      player_id: newPlayer.player_id,
      email: 'hashedemail@example.com',
      hashedPassword: 'hashedpassword',
    };

    await saveCredential(newCred);

    // Try to save another credential with the same email
    const newCredDuplicate: NewCred = {
      player_id: newPlayer.player_id,
      email: 'hashedemail@example.com',
      hashedPassword: 'anotherpassword',
    };

    await expect(saveCredential(newCredDuplicate)).rejects.toThrow('Email already exists');
  });

  it('should throw an error when saving a credential for a non-existent player', async () => {
    const newCred: NewCred = {
      player_id: uuidv4(), // Non-existent player_id
      email: 'nonexistentplayer@example.com',
      hashedPassword: 'hashedpassword',
    };

    await expect(saveCredential(newCred)).rejects.toThrow('Player not found');
  });

  it('should throw an error when saving a credential with missing fields', async () => {
    // Create a new player first
    const playerRepo = AppDataSource.getRepository(Player);
    const newPlayer = new Player();
    newPlayer.player_id = uuidv4();
    newPlayer.username = 'testuser';
    await playerRepo.save(newPlayer);

    const newCred: Partial<NewCred> = {
      player_id: newPlayer.player_id,
      email: 'missingpassword@example.com',
      // Missing hashedPassword
    };

    await expect(saveCredential(newCred as NewCred)).rejects.toThrow('Missing required fields');
  });
});
