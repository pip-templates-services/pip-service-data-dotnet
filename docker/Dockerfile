FROM mcr.microsoft.com/dotnet/core/runtime:3.1

# Set working directory
WORKDIR /app

# Copy compiled binaries
COPY ./obj ./bin

# Copy configuration
COPY config/*.* ./config/

# Set the required environment variables
ENV HTTP_PORT=8080
ENV MONGO_SERVICE_HOST=
ENV MONGO_SERVICE_PORT=27017

# Expose the HTTP port
EXPOSE "8080:8080"

CMD sh -c sleep 50 && dotnet ./bin/main.dll -c ./config/config.yml