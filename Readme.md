Version part

Schedule upgrades

Development part

<!-- PROJECT LOGO -->
<br />
  <a href="">
    <img src="./Store.jfif" alt="Logo" height="128">
  </a>

<h1>Winget Nexus</h1>

Winget Nexus is a self-hosted winget package source. 
It's done with Dotnet core and can run on Linux, Windows, in Docker, locally and in any cloud.
It is currently in [pre-1.0](https://semver.org/#spec-item-4) development so configuration options, output, database and behavior may change at any time, I'm trying my best though to avoid breaking things.


## 🚀 Features

- Openid Connect authentication for Web UI.
- Intuitive and easy to use Webinterface
- Add your own packages for internal or customized software with multiple versions, installers including architectures and scope
- Search, list, show and install software - the core winget features all work
- Support for nested installers and installer switches
- Easy updating of package metadata
- Package download counter
- Support for multiple users and authentication
- Runs on Windows, Linux etc. using Docker

## 🚀 Features triggering

## 🚧 Not Yet Working or Complete
- Authentication (it's currently [not supported by winget](https://github.com/microsoft/winget-cli-restsource/issues/100))
- Probably other stuff? It's work-in-progress - please submit an issue and/or PR if you notice anything!

## 🧭 Getting Started

### 🐋 Docker
1. Install Docker on your machine. Refer to the [official Docker documentation](https://docs.docker.com/get-docker/) for instructions specific to your operating system.
2. Download the docker-compose.yml file from the main branch.
3. Open the docker-compose.yml file and modify the configuration values according to your preferences.  
The configurable options are:
* WINGETTY_SQLALCHEMY_DATABASE_URI: This parameter allows you to specify the database URI for storing WinGetty's data. By default, it is set to use SQLite with a file named database.db. You can use any database URI supported by SQLAlchemy, such as MySQL or PostgreSQL.
* WINGETTY_SECRET_KEY: This parameter sets the secret key used for securing WinGetty's sessions and other cryptographic operations. Replace the value with a random string.
* WINGETTY_ENABLE_REGISTRATION: By default, user registration is enabled (1). If you want to disable user registration, set this value to 0 after you have created your first user.
* WINGETTY_REPO_NAME: This parameter specifies the name of your WinGetty repository. You can change it to any desired name.
4. Start the WinGetty application using Docker Compose:
`docker-compose up -d`  
This command launches the WinGetty container in the background.
5. Access the web interface by opening your browser and navigating to http://localhost:8080.  
If you're running WinGetty on a remote server, replace localhost with the appropriate IP address or hostname.
6. Upon accessing the web interface for the first time, you will be prompted to register a user, this user will become the admin user by default.

> ⚠️ **Note**: WinGet requires HTTPS for secure communication and without it WinGet will throw an error. It is recommended to put WinGetty behind a reverse proxy with a client-trusted SSL/TLS certificate.  
By using a reverse proxy with HTTPS, you can ensure secure transmission of data between clients and WinGetty. Popular reverse proxy solutions include NGINX, Apache, and Caddy. Please refer to the documentation of your chosen reverse proxy for detailed instructions on configuring SSL/TLS certificates.

### 🪄 Using WinGetty

You can test the WinGet API by opening `http://localhost:8080/wg/information` in a browser or with `curl` / `Invoke-RestMethod`.

Now you can add it as a package source in winget using the command provided in the 'Setup' tab in the webinterface:

```
winget source add -n WingetNexus -t "Microsoft.Rest" -a https://localhost:5001/winget/
```

and query it:

```
❯ winget search Signal -s WinGetty
Name   ID            Version
----------------------------
Signal Signal.Signal 1.0.0
```
and install packages:
```
❯ winget install Signal.Signal -s WinGetty
Found bottom [Signal.Signal] Version 1.0.0
This application is licensed to you by its owner.
Microsoft is not responsible for, nor does it grant any licenses to, third-party packages.
Downloading  https://wingetty.dev/api/download/Signal.Signal/1.0.0/x64/both
  ██████████████████████████████  112 MB /  112 MB
Successfully verified installer hash
Starting package install...
Successfully installed

~ took 12s
❯
```