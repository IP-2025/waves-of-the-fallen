import { checkDatabaseHealth } from '../../src/services/healthService';
import {innitAllCharacters} from '../../src/services/characterService'

describe('test character Service', () => {
  it('tests', async () => {
    await innitAllCharacters();
  });
});
