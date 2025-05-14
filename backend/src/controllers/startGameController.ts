import crypto from "crypto";
import {Request, Response} from 'express';
import {k8sApi, namespace, patchIngress} from "../services/k8sService";

export async function startGameController(req: Request, res: Response) {

    try {
        // Step 1: Generate a random 4-letter code
        const code = crypto.randomBytes(2).toString('hex').toUpperCase(); // e.g. "A1B2"
        const podName = `pod-${code.toLowerCase()}`;
        const serviceName = `service-${code.toLowerCase()}`

        // Step 2: Define Pod manifest
        const podManifest = {
            metadata: {
                name: podName,
                labels: {
                    app: 'hello-pod',
                    code: code,
                },
            },
            spec: {
                containers: [
                    {
                        name: 'hello-container',
                        image: 'kaprele/k8s-hello-pod',
                        imagePullPolicy: 'Never',
                        ports: [{containerPort: 3000}],
                        env: [{name: "CODE", value: code}],
                    },
                ],
                restartPolicy: 'Never',
            },
        };

        // Step 3: Create Pod
        await k8sApi.createNamespacedPod({namespace: namespace, body: podManifest});

        // Step 4: (Later) Create Service and Ingress here (placeholder for now)
        res.status(201).json({message: `Pod ${podName} created with code ${code}`, code});
        console.log(`✅ Pod ${podName} created.`);

        // Create Service
        const serviceManifest = {
            metadata: {
                name: serviceName,
            },
            spec: {
                selector: {
                    code, // targets pod with label 'code'
                },
                ports: [
                    {
                        port: 80,
                        targetPort: 3000,
                    },
                ],
            },
        };

        await k8sApi.createNamespacedService({namespace: namespace, body: serviceManifest});
        console.log(`✅ Service ${serviceName} created.`);

        await patchIngress(`/game/${code}`, serviceName)

    } catch (error) {
        console.error('Error creating pod:', error);
        res.status(500).json({error: 'Failed to create pod'});
    }


}