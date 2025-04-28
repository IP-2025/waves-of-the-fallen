import { RequestHandler } from 'express';
import { getAllUnlockedCharactersRepo } from '../repositories/characterRepository';
import { getAllCharacters, getAllUnlockedCharacters } from '../services/characterService';

export const getAllCharacterController: RequestHandler = async (req, res) => {
  try {
    const characters = await getAllCharacters();
    res.json(characters).status(200);
  } catch (err) {
    res.status(500).send('Server error');
  }
};
export const getAllUnlockedCharacterController: RequestHandler = async (req, res) => {
  try {
    const playerId = req.body.player_id;
    const characters = await getAllUnlockedCharacters(playerId);
    res.json(characters).status(200);
  } catch (err) {
    res.status(500).send('Server error');
  }
};