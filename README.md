# BLOGGING Platform Web API
##### For a UI client that consumes this API, please go to [Blogging-UI client](https://github.com/pbravi/Blogging-UI)
## Available functions:
- Login
- Create posts
- List posts
- Get post by id
- Delete posts
- Search posts by text

## Security considerations
- Login password require at least one uppercase, one lowercase, one number and one special character
- The authorization type used is Bearer Token
- Create posts require to be logged with a valid token
- Delete posts require to be logged with a valid token
- An author only can create posts under his own name
- An author only can delete posts created by himself

## Technologies used
- .NET Core 2.1
- EntityFramework Core 2.1
- MS Test (for Unit and Integration tests)
- [Moq](https://github.com/moq/moq4) (for Unit tests)
- [DotCover](https://www.jetbrains.com/dotcover/) (for Test Coverage calculation)
- MSSQLServer Express

## Usage
- Ensure to have a valid connection string in `appsettings.json` file
- Build and run
