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

> ⚠️ **Note**: WinGet requires HTTPS for secure communication and without it WinGet will throw an error. It is recommended to put WinGetty behind a reverse proxy with a client-trusted SSL/TLS certificate.  
By using a reverse proxy with HTTPS, you can ensure secure transmission of data between clients and WinGetty. Popular reverse proxy solutions include NGINX, Apache, and Caddy. Please refer to the documentation of your chosen reverse proxy for detailed instructions on configuring SSL/TLS certificates.

You can test the WinGet API by opening `http://localhost:8080/api/v1/winget/information` in a browser or with `curl` / `Invoke-RestMethod`.

Now you can add it as a package source in winget using the command provided in the 'Setup' tab in the webinterface:

```
winget source add -n WingetNexus -t "Microsoft.Rest" -a https://localhost:5001/api/v1/winget/
```
