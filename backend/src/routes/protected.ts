import express from "express";
import { authenticationStep} from "../middleware/validateMiddleware";

const protectedRouter = express.Router();

protectedRouter.get('/', authenticationStep, (req, res) => {
    res.json({ authenticated: true })
})

export default protectedRouter;