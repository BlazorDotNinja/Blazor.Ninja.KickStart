# Orions.DotNinja.KickStart

DotNinja is OrionsWave's SDK for crafting fast user experiences for web. DotNinja works on top of ASP.NET Core and supports Blazor Server and Blazor WebAssembly and comes with its own BaaS.  

You can find a demo of the SDK here: [kickstart.blazor.ninja](https://kickstart.blazor.ninja)

## Set up a DotNinja App

To run DotNinja you need a DotNinja app. Follow the steps below to set up your app:

1. Sign up to [portal.blazor.ninja](https://portal.blazor.ninja/Account/Signup). 

2. Create a company for your app.
Companies hold resources such as app realms and people together. 

![Create Company](https://portal.blazor.ninja/images/screenshots/create-company.png)

3. Create an app realm for your app.
App realms hold resources such as apps, roles, document data and binary objects together. 

![Create App Realm](https://portal.blazor.ninja/images/screenshots/create-app-realm.png)

4. Create an app. 

![Create App](https://portal.blazor.ninja/images/screenshots/create-app.png)

5. Generate an app token. 
Providing a token is how your app authenticates against DotNinja' BaaS.

![Generate Token](https://portal.blazor.ninja/images/screenshots/create-app-token.png)

6. Paste your app's token into the Program.cs files of the projects below:
  * Orions.DotNinja.KickStart.ServerApp
  * Orions.DotNinja.KickStart.WasmApp
  * Orions.DotNinja.KickStart.WasmServerApp

## Generate a Google API key

To use the Map Form component, provide a Google API key:

1. Sign up to [cloud.google.com](http://console.cloud.google.com).

2. Create a project. 

3. From the side menu go to Google Maps Platform > APIs. 

4. Click on "Maps Javascript API" and hit the "Enable" button.

5. Click on "Geocoding API" and hit the "Enable" button.

6. From the side menu go to APIs & Services > Credentials. 

7. Click on Create Credentials > API key. Copy your API key and paste in the files below:
* Orions.DotNinja.KickStart.ServerApp > Pages > _Host.cshtml
* Orions.DotNinja.KickStart.WasmApp > wwwroot > index.html

## Happy coding!
