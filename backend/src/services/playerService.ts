import { setGoldRepository, getGoldRepository } from 'repositories';

export async function getGoldService(id: string): Promise<number> {
  return getGoldRepository(id);
}

export async function setGoldService(id: string, gold: number): Promise<void> {
  await setGoldRepository(id, gold);
}

export async function addGoldService(id: string, gold: number): Promise<void> {
    const currentGold = await getGoldRepository(id);
    const newGold = currentGold + gold;
    await setGoldRepository(id, newGold);
}
