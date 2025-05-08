import { AppDataSource } from '../database/dataSource';
import { Settings } from '../database/entities/Settings';

const settingsRepo = AppDataSource.getRepository(Settings);

export async function getSettingsByPlayerId(playerId: string) {
  return await settingsRepo.findOneBy({ player_id: playerId });
}

export async function insertSettings(playerId: string, musicVolume: number, soundVolume: number) {
  const existing = await getSettingsByPlayerId(playerId);

  if (existing) {
    existing.musicVolume = musicVolume;
    existing.soundVolume = soundVolume;
    return await settingsRepo.save(existing);
  } else {
    const newSettings = settingsRepo.create({
      player_id: playerId,
      musicVolume,
      soundVolume,
    });
    return await settingsRepo.save(newSettings);
  }
}
