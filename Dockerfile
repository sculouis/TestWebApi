FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
  COPY bin/Release/netcoreapp3.1/ App/
  WORKDIR /App
#   ENTRYPOINT ["dotnet", "TestWebApi.dll"]
  # Use the following instead for Heroku
  CMD ASPNETCORE_URLS=http://*:$PORT dotnet TestWebApi.dll