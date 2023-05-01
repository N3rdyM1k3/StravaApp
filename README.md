# StravaApp

## Getting Started
- Local Dev: Create appsettings.Development.json and add the following: 
  "StravaCredentials": {
    "ClientId": "", 
    "ClientSecret": "", 
    "RefreshToken": "", 
    "AccessToken": ""
  }


Example URL: 
https://profisee-strava-app.azurewebsites.net/may?teamName=1120789&after=1682668800
https://profisee-strava-app.azurewebsites.net/forward/Profisee1/activities?after=1682035200&per_page=200
https://www.strava.com/api/v3/clubs/Profisee1/activities?after=1682035200&per_page=200

#TODO
- Create read out for bot
- Create daily azure trigger
