import { checkDatabaseHealth } from '../../src/services/healthService';
import {innitAllCharacters,getAllCharacters} from '../../src/services/characterService'

describe('test character Service', () => {
  it('tests', async () => {
    await innitAllCharacters();
    await getAllCharacters();
  });
});
