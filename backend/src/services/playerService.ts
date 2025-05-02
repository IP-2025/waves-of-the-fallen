import { setGoldRepository, getGoldRepository } from '../repositories/playerRepository';

export async function getGoldService(id: string): Promise<void> {

  getGoldRepository(id);

}

export async function setGoldService(id: string, gold: number): Promise<void> {

  setGoldRepository(id, gold);

}
