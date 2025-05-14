# Use lightweight Node.js base image
FROM node:20-alpine

# Set working directory inside container
WORKDIR /app

# Copy only the package.json + lock first (for layer caching)
COPY package*.json ./

# Install production dependencies only
RUN npm install --production

# Copy the rest of your app
COPY . .

# Expose the port the app listens on
EXPOSE 3000

# Start the server
CMD ["node", "server.js"]
