import { Router, Request, Response } from 'express';
import {k8sApi, namespace} from "../services/k8sService";

export async function listGameController(req: Request, res: Response) {
    try {
        // Get pods with label app=hello-pod
        const podList = await k8sApi.listNamespacedPod(
            {namespace: namespace}
        );

        const pods = (podList.items || []).map(pod => {
            const code = pod.metadata?.labels?.code || 'unknown';
            const name = pod.metadata?.name || 'unnamed';
            const phase = pod.status?.phase || 'Unknown';

            return {
                name,
                code,
                status: phase,
                route: `/game/${code}`
            };
        });

        res.json({ pods });
    } catch (error) {
        console.error('❌ Failed to fetch pods:', error);
        res.status(500).json({ error: 'Failed to list pods' });
    }
}