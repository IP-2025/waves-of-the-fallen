## Backend
### Node.js / Express backend with typescript
- create a `.env` file in the directory `backend` and use the `.env.template` as a template
- the `.env` is necessary for the docker containers to work
- Run `docker compose up` to startup the backend server and DB
- check `http://localhost/api/healthz` for a health check

### Project structure 
- lib/ -> prisma and other libs
- controllers/ -> controllers for the rest api
- services/ -> logic operations for the controllers
- core/ -> settings for server
- routes/ -> routes for the rest api

### Migrations
- the migrations work with the help of prisma
- [prisma](https://www.prisma.io/docs/guides/docker)
- to create new migrations run `npx prisma migrate dev --name init`
- or ping @Pazl27

- you can check the db with the help of prisma studio
- after running the docker containers
- visit `http://localhost:5555` to see the db
