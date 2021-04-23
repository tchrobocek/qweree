FROM node:alpine AS build
WORKDIR /app
COPY ./src/Qweree.QAdmin.WebApplication .
RUN npm install
RUN npm run build

RUN ls -al dist

FROM nginx:alpine
COPY ./src/Qweree.QAdmin.WebApplication/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/dist/QAdmin /usr/share/nginx/html
EXPOSE 80