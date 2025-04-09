import { checkDatabaseHealth } from '../../src/services/healthService';

describe('test healthService', () => {
  it('test database health function', async () => {
    const result = await checkDatabaseHealth();
    expect(result).toBe(true);
  });
});
