# StravaApp

## Getting Started
- Local Dev: copy appsettings.json and create appsettings.Development.json 


Example URL: 
https://profisee-strava-app.azurewebsites.net/forward/Profisee1/activities?after=1682035200&per_page=200
https://www.strava.com/api/v3/clubs/Profisee1/activities?after=1682035200&per_page=200



This is an app I am creating to track strava data for my company's fitness challenges

az keyvault set-policy --name "profisee-strava-keyvault" --object-id "d7fc9484-fd8f-4f58-b866-58f4d86a5f5d" --secret-permissions get list

#TODO
- Switch to use profisee bot creds
- Remove creds from source code and figure out proper fix for local debug 
- Update return type. No. Leave it count. Then make it total time.... 
- Create ProfiseeBot Account. Use your email. Takes summary information up to a single page.  

https://learn.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/
