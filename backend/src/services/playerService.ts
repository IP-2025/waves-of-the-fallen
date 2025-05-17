import { setGoldRepository, getGoldRepository } from '../repositories/playerRepository';

export async function getGoldService(id: string): Promise<number> {
  return getGoldRepository(id);
}

export async function setGoldService(id: string, gold: number): Promise<void> {
  await setGoldRepository(id, gold);
}
