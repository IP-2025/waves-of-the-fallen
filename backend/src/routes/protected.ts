import express from "express";
import { authenticationStep} from "../middleware/validateMiddleware";
import { fetchSettings, setSettings } from '../controllers/settingsController';
import { getSettingsByPlayerId } from '../repositories/settingsRepository';

const protectedRouter = express.Router();

//protectedRouter.post('/fetchSettings', fetchSettings )
protectedRouter.post('/setSettings', authenticationStep, setSettings);

protectedRouter.get('/', authenticationStep, (req, res) => {
    res.json({ authenticated: true })
})

export default protectedRouter;