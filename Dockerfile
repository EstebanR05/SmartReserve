# Stage 1: Build
FROM node:20-alpine AS build

WORKDIR /app

# Copy package files
COPY package.json package-lock.json ./

# Install dependencies
RUN npm install

# Copy prisma schema
COPY prisma ./prisma/

# Generate Prisma Client
RUN npm run prisma:generate

# Copy configuration files
COPY tsconfig.json tsconfig.build.json nest-cli.json ./

# Copy source code
COPY src ./src

# Build the application
RUN npm run build

# Stage 2: Production
FROM node:20-alpine

WORKDIR /app

# Copy package files
COPY package.json package-lock.json ./

# Install only production dependencies
RUN npm i --omit=dev --ignore-scripts

# Copy prisma schema and generate client
COPY prisma ./prisma/
RUN npm run prisma:generate

# Copy built application from build stage
COPY --from=build /app/dist ./dist

# Expose the port (Railway will use $PORT)
EXPOSE 3000

# Start command
CMD ["node", "dist/main"]
