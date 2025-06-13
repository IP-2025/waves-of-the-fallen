import { Request, Response } from 'express';
import { getLobbyPorts, getPodManifest, getServiceManifest, k8sApi, namespace } from 'services/k8sService';

export async function joinGameController(req: Request, res: Response) {
  let lobbyCode;
  try {
    lobbyCode = req.body.lobbyCode;

    const { udpPort, rpcPort } = getLobbyPorts(lobbyCode);

    res.status(200).json({ udpPort, rpcPort, lobbyCode});
  } catch(error) {
    console.error('Error requesting Lobby:', error);
    res.status(500).json({ error: `Failed to request Lobby for lobbycode ${lobbyCode}` });
  }

}