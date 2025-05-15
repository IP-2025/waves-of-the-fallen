// __mocks__/services/k8sService.ts
export const k8sApi = {
  createNamespacedPod: jest.fn(),
  deleteNamespacedPod: jest.fn(),
};

export const namespace = 'mock-namespace';
export const patchIngress = jest.fn();
