import {Request, Response} from "express";
import {k8sApi, namespace, removeFromIngress} from "services/k8sService";

export async function stopGameController(req: Request, res: Response){

    try {
        // Step 1: Generate a random 4-letter code
        const code = req.body.code;
        const podName = `pod-${code.toLowerCase()}`;
        const serviceName = `service-${code.toLowerCase()}`


        // Step 3: Create Pod
        await k8sApi.deleteNamespacedPod({namespace: namespace, name: podName});
        await k8sApi.deleteNamespacedService({namespace: namespace, name: serviceName});

        // Step 4: (Later) Create Service and Ingress here (placeholder for now)
        res.status(201).json({ message: `Pod ${podName} deleted`});
        console.log(`✅ Pod ${podName} deleted.`);

        await removeFromIngress(`game/${code}`, serviceName)

    } catch (error) {
        console.error('Error stopping pod:', error);
        res.status(500).json({ error: 'Failed to stop pod' });
    }


}