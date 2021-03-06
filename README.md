- It is a cross-platform utility designed with extensibility in mind, where it could easily be hooked up with various services in public. 
- Objective is to understand with a small narrow code if basic connectivity parameters are valid. 
- And, if there are any challenges with .NET echosystem or platform, we can easily get the idea fair quickly and act accordingly. 
- Currently, it just have integration capabilities developed for:
    - AZURE SQL
    - REST

## Next
- It is envisoned to cover most of the regular services that are heavily used. There is no restriction which all services makes the best sense over here.
- It just needs contribution to make it even better via `PR`.
- The next level development effort would e.g. be on AZURE REDIS CACHE, AZURE STORAGE, AZURE COSMOS, etc..

## Download
- Download the utility from [here](../../releases/tag/v1.0)
    - If .NET Framework based test on Windows OS, download [PeepAcross-fx-win-x64.zip](../../releases/download/v1.0/PeepAcross-fx-win-x64.zip)
    - If .NET Core based test on Windows OS, download [PeepAcross-win-x64.zip](../../releases/download/v1.0/PeepAcross-win-x64.zip)
    - If .NET Core based test on Linux OS, download [PeepAcross-linux-x64.tgz](../../releases/download/v1.0/PeepAcross-linux-x64.tgz)
- Unzip in Windows via [Extract all] or PS
    > `> Expand-Archive -LiteralPath .\PeepAcross-win-x64.Zip -DestinationPath .
- Unzip in Linux via [tar]
    > $ tar -xvf PeepAcross-linux-x64.tgz

## How to run??
- Open PS or cmd or a termnial
- Go to the directory where package is downloaded (via `cd`)
- If .NET Framework based test, use `PeepAcross-Full.exe` (expects >= 4.6.1 .NET framework installed)
    ```
    > .\PeepAcross-Full.exe
    Usage: PeepAcross-Full [-sql]
                           [-httpclient]
    ```
- If .NET Core based test, use `PeepAcross-Core.exe` (app is self-contained)
    ```
    > .\PeepAcross-Core.exe
    Usage: PeepAcross-Core [-sql]
                           [-httpclient]
    ```
- If .NET Core based test on any [linux distro](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#linux-rids), use `PeepAcross-Core.exe` (app is self-contained)
    ```
    $ ./PeepAcross-Core.exe
    Usage: PeepAcross-Core [-sql]
                           [-httpclient]
    ```

### For SQL
This section have detailed on SQL related connectivity test.

#### Options
```
>>.\PeepAcross-Core.exe -sql -sqlServer ""  
                            -sqlServerPort ""  
                            -sqlDatabase ""  
                            -sqlQuery ""  
                            -aadTenantId ""  
                            -aadClientId "" 
                            -aadClientSecretKey ""
```
#### Examples
- If SQL Managed Identity based communication
```
.\PeepAcross-Core.exe -sql -sqlServer YOURAZSQLSERVER.database.windows.net -sqlServerPort YOURSQLPORT -sqlDatabase YOURAZSQLDB -sqlQuery "YOUR SQL QUERY" -aadTenantId YOUR-AAD-TENANT-ID -aadClientId YOUR-AAD-CLIENT-ID -aadClientSecretKey YOUR-AAD-CLIENT-SECRET-KEY
```
- If SQL userid password based communication
```
.\PeepAcross-Core.exe -sql -sqlServer YOURAZSQLSERVER.database.windows.net -sqlServerPort YOURSQLPORT -sqlDatabase YOURAZSQLDB -sqlQuery "YOUR SQL QUERY" -sqlUserID "YOUR-SQL-USERID" -sqlUserPassword "YOUR-SQL-USER-PASSWORD"
```

### For HTTPCLIENT
- All CRUD operations (POST, GET, PUT, DELETE + HEAD, OPTIONS, PATCH) are open to be tested.
- There is an option to load test here too. However, the code block would reuse the client socket for each outbound communication.

#### Options
```
>>.\PeepAcross-Core.exe -httpclient -serviceUri ""  
                            -methodKind ""  
                            -bypassServerCertValidation "" 
                            -clientCertificate ","  
                            -headers ""  
                            -body ""  
                            -loadTest ""
```
#### Examples
- GET call
```
.\PeepAcross-Core.exe -httpclient -serviceUri YOUR-SERVICE-URI
```
- GET call if needs server certificate validation override
```
.\PeepAcross-Core.exe -httpclient -serviceUri YOUR-SERVICE-URI -bypassServerCertValidation true
```
- GET call if needs client certificate be presented from certificate store
    - Pass client certificate thumbprint
    - Pass CurrentUser or LocalMachine after comma delimiter
    - Internally hard-coded to pull certificate from `personal` store
```
.\PeepAcross-Core.exe -httpclient -serviceUri YOUR-SERVICE-URI -methodKind get -clientCertificate "YOUR-CLIENTCERT-THUMBPRINT,YOUR-STORE-LOCATION" 
```
- GET call if needs client certificate be presented from filesystem with pfx or pem
    - Provide the full path name to your pfx or pem file (could be relative too)
    - Provide the password if exists, otherwise just pass empty after comma
```
.\PeepAcross-Core.exe -httpclient -serviceUri YOUR-SERVICE-URI -methodKind get -clientCertificate "YOUR-CLIENTCERT-FILE-PATH-NAME,YOUR-CERT-PASSWORD" 
```
- GET with headers and bypass server certificate validation
    - Raw headers in json and just one line without passing any double quotes
    - e.g. -headers "{h1:k1,h2:k2}"
```
.\PeepAcross-Core.exe -httpclient -serviceUri YOUR-SERVICE-URI -methodKind get -headers "YOUR-RAW-HEADERS-IN-JSON" -bypassServerCertValidation true
```
- GET with headers JSON file and bypass server certificate validation
    - Pass json headers file with full path name, which can have a well-formed JSON content
    - e.g. -headers ".\header1.json"
```
.\PeepAcross-Core.exe -httpclient -serviceUri YOUR-SERVICE-URI -methodKind get -headers "YOUR-HEADERS-JSON-FILE-PATH" -bypassServerCertValidation true
```
- POST with body JSON in RAW string and bypass server certificate validation
    - Raw body in json and just one line without passing any double quotes
    - e.g. -body "{myProperty1:string1,myProperty2:string2,myProperty3:string3}"
```
.\PeepAcross-Core.exe -httpclient -serviceUri YOUR-SERVICE-URI -methodKind post -headers "YOUR-RAW-BODY-IN-JSON" -bypassServerCertValidation true
```
- POST with body JSON file and bypass server certificate validation
    - Pass json body file with full path name, which can have a well-formed JSON content
    - e.g. -body ".\body1.json"
```
.\PeepAcross-Core.exe -httpclient -serviceUri YOUR-SERVICE-URI -methodKind post -body "YOUR-BODY-JSON-FILE-PATH" -bypassServerCertValidation true
```
- Load test for POST with body JSON file and bypass server certificate validation
    - Pass json body file with full path name, which can have a well-formed JSON content
    - e.g. -body ".\body1.json"
    - Pass load test parameter number (>0 and INT32.MAXVALUE is the limit)
    - e.g. -loadTest 10
```
.\PeepAcross-Core.exe -httpclient -serviceUri YOUR-SERVICE-URI -methodKind post -body "YOUR-BODY-JSON-FILE-PATH" -bypassServerCertValidation true -loadTest COUNT
```
