FROM node:20.10.0-alpine AS builder

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm cache clean --force
RUN npm install -g npm@latest
RUN npm install -g @angular/cli
RUN npm install

COPY . .

# TODO --prod
RUN npm run build --platform=node

FROM nginx:latest

COPY --from=builder /app/dist/tic-tac-toe-app/browser /usr/share/nginx/html

RUN rm /etc/nginx/conf.d/default.conf

COPY nginx.conf /etc/nginx/conf.d/

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]