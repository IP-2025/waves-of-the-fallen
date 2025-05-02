import { RequestHandler } from 'express';
import  {getGoldService,setGoldService} from '../services/playerService'

export const getGoldController: RequestHandler = async (req, res) => {
  try {
    const playerId = req.body.player_id;
    const gold = await getGoldService(playerId);
    res.json(gold).status(200);
  } catch (err) {
    res.status(500).send('Server error');
  }
};

export async function setGoldController(){


}
