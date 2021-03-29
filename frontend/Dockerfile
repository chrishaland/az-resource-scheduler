FROM node:14 as build
WORKDIR /src
COPY ["package*.json", "."]
RUN npm ci --only=production
COPY . .
RUN npm run build

FROM nginx:stable
EXPOSE 8080
COPY --from=build /src/build /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf