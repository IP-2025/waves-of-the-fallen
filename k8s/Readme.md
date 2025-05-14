## New Release 

1. Set Release tag
```bash
export VERSION=v0.0.1
```

2. Run Skaffold with your prod profile:
```bash
skaffold run --profile prod
```
This will build your image, push it to Docker Hub under kaprele/waves-of-the-fallen-backend:$VERSION, and apply your Kubernetes manifests.

3. Verify Rollout
```bash
kubectl get pods,svc -n production
kubectl rollout status deploy/mngmt-depl -n production
```

Check that:
- All pods come up in Running state.
- Services are correctly exposed.
- No CrashLoop or ImagePull errors appear.