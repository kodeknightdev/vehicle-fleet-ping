FROM node:12.16-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npx ng build --prod

FROM nginx:1.18-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/fleet-ping-web /usr/share/nginx/html
EXPOSE 80
