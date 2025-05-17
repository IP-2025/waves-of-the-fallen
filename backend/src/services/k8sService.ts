import * as k8s from '@kubernetes/client-node';

const kc = new k8s.KubeConfig();
kc.loadFromDefault(); // This automatically loads from ~/.kube/config or in-cluster config

// API clients
export const k8sApi = kc.makeApiClient(k8s.CoreV1Api);
export const k8sAppsApi = kc.makeApiClient(k8s.AppsV1Api);
export const k8sNetworkingApi = kc.makeApiClient(k8s.NetworkingV1Api);

export const namespace = 'k8s-demo';
export const ingressName = 'dynamic-game-ingress';

export async function patchIngress(newPathName: string, serviceName: string) {

    try {
        // 1. Get current Ingress
        const ingress = await k8sNetworkingApi.readNamespacedIngress({name: ingressName, namespace: namespace});

        // 2. Add new path to the existing list
        const newPath = {
            path: newPathName,
            pathType: 'Prefix',
            backend: {
                service: {
                    name: serviceName,
                    port: {number: 80},
                },
            },
        };

        const currentPaths = ingress.spec?.rules?.[0]?.http?.paths ?? [];
        const updatedPaths = [...currentPaths, newPath];

        ingress.spec!.rules![0].http!.paths = updatedPaths;

        // 3. Patch the Ingress
        await k8sNetworkingApi.replaceNamespacedIngress({name: ingressName, namespace: namespace, body: ingress});
        console.log(`✅ Patched Ingress to route ${newPathName} → ${serviceName}`);
    } catch (err) {
        console.error('❌ Failed to patch ingress:', err);
    }

}

export async function removeFromIngress(pathName: string, serviceName: string){

    try {
        // 1. Get current Ingress
        const ingress = await k8sNetworkingApi.readNamespacedIngress({name: ingressName, namespace: namespace});

        const currentPaths = ingress.spec?.rules?.[0]?.http?.paths ?? [];
        const updatedPaths = currentPaths.filter(path => path.path !== pathName)

        ingress.spec!.rules![0].http!.paths = updatedPaths;

        // 3. Patch the Ingress
        await k8sNetworkingApi.replaceNamespacedIngress({name: ingressName, namespace: namespace, body: ingress});
        console.log(`✅ Deleted Route ${pathName} and Service ${serviceName} from ingress`);
    } catch (err) {
        console.error('❌ Failed to patch ingress:', err);
    }
}