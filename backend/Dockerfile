FROM node:20

WORKDIR /app

COPY package.json tsconfig.json ./

RUN npm install

COPY . .

EXPOSE 3000

CMD ["sh", "-c", "npm run dev"]
