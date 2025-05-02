## Backend
### Node.js / Express backend with typescript
- create a `.env` file in the directory `backend` and use the `.env.template` as a template
- complete [kubernetes setup](#kubernetes-setup-for-local-development)
- check `http://localhost/api/healthz` for a health check

### Project structure
- lib/ -> typeORM and other libs
- controllers/ -> controllers for the rest api
- services/ -> logic operations for the controllers
- repositroires -> logic to interact with db
- core/ -> settings for server
- routes/ -> routes for the rest api


## Kubernetes Setup for Local Development

1. Install Prerequisites

- Install Docker Desktop (Mac/Windows) or Docker Engine (Linux)
  Enable Kubernetes in Docker Desktop (Preferences → Kubernetes → Enable Kubernetes)

- Install Skaffold

  Mac: `brew install skaffold`\
  Linux: `sudo apt install skaffold`\
  Windows: `choco install skaffold`

- Setup a namespace
  `kubectl create namespace waves-of-the-fallen`\
  `kubectl config set-context --current --namespace=waves-of-the-fallen`

- Run the following command to configure the ingress\
  `kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.1/deploy/static/provider/cloud/deploy.yaml`

2. Prepare Secrets

- Edit create db-secret.yaml from sample.db-secret.yaml with the credentials.\
Secrets will be applied automatically by Skaffold\
Do not commit real secrets to Git!

3. Run and Verify

   Start project: `skaffold dev`

- Access services:\
Express backend: http://localhost/api/v1/healthz \
pgAdmin UI: http://localhost/pgadmin \
U can test endpoints using rest-testing.http

4. Troubleshooting

- Check pods: `kubectl get pods`\
Check logs: `kubectl logs deployment/mngmt-depl`\
Check ingress: `kubectl describe ingress app-ingress`

If something breaks, u can message @kaprele
