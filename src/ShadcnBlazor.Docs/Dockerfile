# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the entire src across to preserve relative paths for project references
# (We need the whole src because of the docs compiler and library dependencies)
COPY . .

# Restore the entire solution to ensure all dependencies (including compiler) are available
RUN dotnet restore "ShadcnBlazor.sln"

# Build and Publish
WORKDIR "/app/src/ShadcnBlazor.Docs"
RUN dotnet publish "ShadcnBlazor.Docs.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Clean default nginx files
RUN rm -rf ./*

# Copy published files
COPY --from=build /app/publish/wwwroot .

# Copy nginx config
COPY src/ShadcnBlazor.Docs/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
