## Backend
### Node.js / Express backend with typescript
- create a `.env` file in the directory `backend` and use the `.env.template` as a template
- the `.env` is necessary for the docker containers to work
- Run `docker compose up` to startup the backend server and DB
- check `http://localhost/api/healthz` for a health check

### Project structure
- lib/ -> typeORM and other libs
- controllers/ -> controllers for the rest api
- services/ -> logic operations for the controllers
- repositroires -> logic to interact with db
- core/ -> settings for server
- routes/ -> routes for the rest api



```
