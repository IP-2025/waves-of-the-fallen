import { RequestHandler } from 'express';
import { getAllCharactersRepo } from '../repositories/characterRepository';
import { getAllCharacters } from '../services/characterService';

export const getAllCharacterController: RequestHandler = async (req, res) => {
  try {
    const characters = await getAllCharacters();
    res.json(characters).status(200);
  } catch (err) {
    res.status(500).send('Server error');
  }
};