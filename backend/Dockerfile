FROM node:20

WORKDIR /app

COPY package.json tsconfig.json ./
COPY ./prisma ./prisma

RUN npm install

COPY . .

EXPOSE 3000

CMD ["sh", "-c", "npm run db:deploy && npm run dev"]
