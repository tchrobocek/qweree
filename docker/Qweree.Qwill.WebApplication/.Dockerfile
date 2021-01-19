FROM node:alpine AS build
WORKDIR /app
COPY ./src/Qweree.Qwill.WebApplication .
RUN npm install
RUN npm run build

RUN ls -al dist

FROM nginx:alpine
COPY ./src/Qweree.Qwill.WebApplication/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/dist/Qwill /usr/share/nginx/html
EXPOSE 80
