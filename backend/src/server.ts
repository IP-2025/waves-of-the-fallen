import app from './app';
import { PORT } from './core/config';

const port = PORT || 3000;

app.listen(port, () => {
  console.log(`🚀 Server screaming on port ${port}`);
});
