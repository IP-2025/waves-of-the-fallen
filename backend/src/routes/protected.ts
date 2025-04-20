import express from "express";
import { authenticationStep} from "../middleware/validateMiddleware";
import { getSettings, setSettings } from '../controllers/settingsController';

const protectedRouter = express.Router();

protectedRouter.post('/getSettings', authenticationStep, getSettings )
protectedRouter.post('/setSettings', authenticationStep, setSettings);

protectedRouter.get('/', authenticationStep, (req, res) => {
    res.json({ authenticated: true })
})

export default protectedRouter;