FROM node:20.18-alpine AS build

WORKDIR /app

COPY apps/frontend/package.json ./

RUN npm install

COPY apps/frontend .

RUN npm run build

FROM nginx:1.27.2-alpine

COPY apps/frontend/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]