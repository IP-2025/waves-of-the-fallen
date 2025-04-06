import { Router } from 'express';

const router = Router();

router.post('/login', (req, res) => {
  const { username, password } = req.body;
  // TODO: validate and authenticate user
  res.status(200).json({ message: `Login successful for ${username}` });
});

router.post('/register', (req, res) => {
  const { username, password } = req.body;
  // TODO: create user in database
  res.status(201).json({ message: `User ${username} registered` });
});

export default router;
