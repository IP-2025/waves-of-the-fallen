import * as k8s from '@kubernetes/client-node';

const kc = new k8s.KubeConfig();
kc.loadFromDefault(); // This automatically loads from ~/.kube/config or in-cluster config

// API clients
export const k8sApi = kc.makeApiClient(k8s.CoreV1Api);
export const k8sAppsApi = kc.makeApiClient(k8s.AppsV1Api);
export const k8sNetworkingApi = kc.makeApiClient(k8s.NetworkingV1Api);

export const namespace = 'waves-of-the-fallen';
export const ingressName = 'dynamic-game-ingress';

export async function patchIngress(newPathName: string, serviceName: string) {
  try {
    // 1. Get current Ingress
    const ingress = await k8sNetworkingApi.readNamespacedIngress({ name: ingressName, namespace: namespace });

    // 2. Add new path to the existing list
    const newPath = {
      path: newPathName,
      pathType: 'Prefix',
      backend: {
        service: {
          name: serviceName,
          port: { number: 80 },
        },
      },
    };

    if (!ingress.spec?.rules?.[0]?.http) {
      ingress.spec!.rules![0].http = {
        paths: [],
      };
    }

    const currentPaths = ingress.spec?.rules?.[0]?.http?.paths ?? [];
    const updatedPaths = [...currentPaths, newPath];

    ingress.spec!.rules![0].http!.paths = updatedPaths;

    // 3. Patch the Ingress
    await k8sNetworkingApi.replaceNamespacedIngress({ name: ingressName, namespace: namespace, body: ingress });
    console.log(`✅ Patched Ingress to route ${newPathName} → ${serviceName}`);
  } catch (err) {
    console.error('❌ Failed to patch ingress:', err);
  }
}

export async function removeFromIngress(pathName: string, serviceName: string) {
  try {
    // 1. Get current Ingress
    const ingress = await k8sNetworkingApi.readNamespacedIngress({ name: ingressName, namespace: namespace });

    const currentPaths = ingress.spec?.rules?.[0]?.http?.paths ?? [];
    const updatedPaths = currentPaths.filter((path) => path.path !== pathName);

    ingress.spec!.rules![0].http!.paths = updatedPaths;

    // 3. Patch the Ingress
    await k8sNetworkingApi.replaceNamespacedIngress({ name: ingressName, namespace: namespace, body: ingress });
    console.log(`✅ Deleted Route ${pathName} and Service ${serviceName} from ingress`);
  } catch (err) {
    console.error('❌ Failed to patch ingress:', err);
  }
}

// In-memory set of nodePorts that are already taken
const usedNodePorts = new Set<number>();
const games: {
  [lobbyCode: string]: {
    udpPort: number;
    rpcPort: number;
  };
} ={};

/**
 * Allocate the next free nodePort in the Kubernetes default NodePort range (30000–32767).
 */
export function allocateNodePorts(): number[] {
  const MIN = 30000;
  const MAX = 32767;
  for (let port = MIN; port <= MAX; port += 2) {
    if (!usedNodePorts.has(port)) {
      usedNodePorts.add(port);
      usedNodePorts.add(port + 1);
      return [port, port + 1];
    }
  }
  throw new Error('No free NodePort available');
}

/**
 * Create a Pod manifest for a game server with the given `code`.
 * @param code
 * @param podName
 * @param image
 */
export function getPodManifest(code: string, podName: string = `pod-${code.toLowerCase()}`, image: string = 'kaprele/waves-of-the-fallen_game') {
  return {
    apiVersion: 'v1',
    kind: 'Pod',
    metadata: {
      name: podName,
      labels: {
        app: 'game-server',
        code, // label used to select this Pod
      },
    },
    spec: {
      containers: [
        {
          name: 'game-container',
          image,
          imagePullPolicy: 'IfNotPresent', // Use local image if available
          ports: [
            { containerPort: 3000, protocol: 'UDP' },
            { containerPort: 9999, protocol: 'UDP' },
          ],
          env: [{ name: 'CODE', value: code }], // Pass the game code as an environment variable
        },
      ],
      restartPolicy: 'Never', // Don't restart the Pod automatically
    },
  };
}

/**
 * Create a NodePort Service manifest for a game server pod identified by `code`.
 *
 * @param code      The label value used to select the Pod (e.g. the game “code”)
 * @param serviceName  The name to give the Service (must be unique per game)
 * @param udpTargetPort   The port the Pod is listening on (e.g. 3000)
 */
export function getServiceManifest(code: string, serviceName: string, udpTargetPort: number = 3000, rpcTargetPort: number = 9999) {
  // Grab a free port on the host
  const [udpPort, rpcPort] = allocateNodePorts();

  const serviceManifest = {
    apiVersion: 'v1',
    kind: 'Service',
    metadata: {
      name: serviceName,
      labels: {
        app: 'game-server',
        gameCode: code,
      },
    },
    spec: {
      type: 'NodePort', // expose on the node
      selector: {
        code, // will match pods labeled { code: "<code>" }
      },
      ports: [
        {
          name: 'gameplay',
          port: udpPort, // in-cluster port the Pod listens on
          targetPort: udpTargetPort, // forward to the Pod’s port
          protocol: 'UDP',
          nodePort: udpPort,
        },
        {
          name: 'rpc',
          port: rpcTargetPort, // in-cluster port the Pod listens on
          targetPort: rpcTargetPort, // forward to the Pod’s port
          protocol: 'UDP',
          nodePort: rpcPort,
        },
      ],
    },
  };

  return { serviceManifest, rpcPort, udpPort };
}

export function freeNodePort(port: number) {
  usedNodePorts.delete(port);
}

export function saveLobby(lobbyCode: string, udpPort: number, rpcPort: number) {
  games[lobbyCode] = { udpPort: udpPort, rpcPort: rpcPort };
}

export function getLobbyPorts(lobbyCode: string){
  return games[lobbyCode];
}

export function removeLobby(lobbyCode: string){
  delete games[lobbyCode];
}
