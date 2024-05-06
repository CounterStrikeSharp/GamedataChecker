# GamedataChecker UI

> [!NOTE]  
> Latest version of the GDC should be placed in the `wwwroot/internal` folder (I suggest using -p:PublishSingleFile=true --self-contained true)

# Dependencies
- dotnet-runtime 8.0.4
	- [Windows](https://download.visualstudio.microsoft.com/download/pr/4e4fef83-93d2-4bff-bc74-2c1b0623fbfb/f40b7f2752c23bd0a1046a2a8ed887c5/dotnet-runtime-8.0.4-win-x64.exe)
	- Linux (missing download link)

- aspnetcore-runtime 8.0.4
	- [Windows](https://download.visualstudio.microsoft.com/download/pr/eb04a61f-75c3-43dd-93d9-b6a7248116c7/f884863125730a39d7fa4139a00c0f99/aspnetcore-runtime-8.0.4-win-x64.exe)
	- Linux (missing download link)

# Publish
- `dotnet publish UI/UI.csproj -c Release`

# Deploy
- Upload content from `publish` folder
- `UI --urls http://0.0.0.0:5000` (where 5000 is the port you wish to use)