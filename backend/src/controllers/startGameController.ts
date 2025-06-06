import crypto from 'crypto';
import { Request, Response } from 'express';
import { getPodManifest, getServiceManifest, k8sApi, namespace } from 'services/k8sService';

export async function startGameController(req: Request, res: Response) {
  try {
    // Step 1: Generate a random 4-letter code
    const code = crypto.randomBytes(2).toString('hex').toLowerCase(); // e.g. "A1B2"
    const podName = `pod-${code}`;
    const serviceName = `service-${code}`;

    // Step 2: Create Pod Manifest
    const podManifest = getPodManifest(code);

    // Step 3: Create Pod
    await k8sApi.createNamespacedPod({ namespace: namespace, body: podManifest });

    // Step 4: (Later) Create Service and Ingress here (placeholder for now)
    console.log(`✅ Pod ${podName} created.`);

    // Create Service
    const { serviceManifest, udpPort, rpcPort } = getServiceManifest(code, `svc-${code}`);
    await k8sApi.createNamespacedService({ namespace: namespace, body: serviceManifest });
    console.log(`✅ Service ${serviceName} created. Pod reachable on ${udpPort} (UDP) and ${rpcPort} (RPC)`);

    // await patchIngress(`/game/${code}`, serviceName)
    res.status(201).json({
      message: `Pod ${podName} created with code ${code} on port ${udpPort} (UDP) and ${rpcPort} (RPC)`,
      code: code,
      udpPort: udpPort,
      rpcPort: rpcPort,
    });
  } catch (error) {
    console.error('Error creating pod:', error);
    res.status(500).json({ error: 'Failed to create pod' });
  }
}
