# StravaApp
This is an app I am creating to track strava data for my company's fitness challenges

az keyvault set-policy --name "profisee-strava-keyvault" --object-id "d7fc9484-fd8f-4f58-b866-58f4d86a5f5d" --secret-permissions get list

#TODO
- Strava OAuth
    - Site is currently broken. Should reread strava auth documentation, but I think we are getting closer and closer. Secrets and public url. 
    - https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers

https://learn.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/
