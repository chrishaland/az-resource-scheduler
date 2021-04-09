FROM node:14 as build
WORKDIR /src
COPY [".yarn/releases/*", ".yarn/releases/"]
COPY ["yarn.lock", "."]
COPY [".yarnrc.yml", "."]
COPY ["package*.json", "."]
RUN yarn install
COPY . .
RUN yarn build

FROM nginx:stable
EXPOSE 8080
COPY --from=build /src/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf