# SimpleCmsWebApi

Test project implementing simple webapi.

Used .net 5, aps.net core and entity framework.

## Sulution structure

- `SimpleCmsWebApi` Main project implementing WebAPI
  - `Authentication` Contains implementation of the *AuthenticationHandler* to autentificate by secret token 'SuperToken'. Token is stored in the appsettings as *SuperSercretToken*.
  - `Controllers` ArticleController implements get, post, put, path and delete methods.
  - `Data` DB related classes. When saving entities that implement *ITrackable* interface timestamp update automatically.
  - `DTO` Objects to transfer
  - `Mappings` Profile for *AutoMapper* to map models and DTOs.
  - `Models` DB models
- `SimpleCmsWebApi.IntegrationTests`  
Integration tests using in-memory server and in-memory DB (do not cover all cases)
- `SimpleCmsWebApi.UnitTests`  
Unit tests (do not cover all cases)

## Settings
Sql Server is used to store data. And the connection string can be changed in `appsettings.json` ConnectionStrings.SimpleCMSConnection. 

## Using api
API information is available using swagger `<url>/swagger`

Methods **post**, **put**, **path** and **delete** require autorization. To do this, you need to pass the header `SuperToken` with value stored in appsettings as *SuperSercretToken*.

Docker container available at [docker hub](https://hub.docker.com/repository/docker/smirnik/simplecms)
