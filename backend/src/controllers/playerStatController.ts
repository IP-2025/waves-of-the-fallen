import { RequestHandler } from 'express';
import { getAllUnlockedCharacters } from '../services/characterService';


export const getGoldController: RequestHandler = async (req, res) => {
  try {
    const playerId = req.body.player_id;
    const characters = await getAllUnlockedCharacters(playerId);
    res.json(characters).status(200);
  } catch (err) {
    res.status(500).send('Server error');
  }
};

export async function setGoldController(){


}
