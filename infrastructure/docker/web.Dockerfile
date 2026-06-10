# Vue.js Web App Dockerfile (Multi-stage)
FROM node:20-alpine AS base
WORKDIR /app

# Development stage
FROM base AS development
COPY apps/web/package*.json ./
RUN npm install
COPY apps/web/ .
EXPOSE 3000
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]

# Build stage
FROM base AS build
COPY apps/web/package*.json ./
RUN npm ci
COPY apps/web/ .
RUN npm run build

# Production stage
FROM nginx:alpine AS production
COPY --from=build /app/dist /usr/share/nginx/html
COPY infrastructure/docker/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
