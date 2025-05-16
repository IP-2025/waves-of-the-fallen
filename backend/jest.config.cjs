module.exports = {
  preset: 'ts-jest/presets/default',
  testEnvironment: 'node',
  setupFilesAfterEnv: ['<rootDir>/jest.setup.ts'],
  moduleDirectories: ['node_modules', 'src'],
  testMatch: ['**/test/**/*.test.ts'],
  transform: {
    '^.+\\.ts$': ['ts-jest', { tsconfig: 'tsconfig.test.json' }]
  },
  transformIgnorePatterns: [
  // '/node_modules/(?!(your-own-esm-packages)/)', // keep transforming ESM you need…
    'src/services/k8sService.ts',
  '/node_modules/@kubernetes/client-node/'      // but skip this one entirely
],
}
