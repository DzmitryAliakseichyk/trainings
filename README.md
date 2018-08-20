# TRAININGS
Here will be placed results of understanding new features and improvements of .Net

## Setup MongoDB

### 1. Create MongoDB admin and enable authentication
[Enable Authentication (MongoDB 4.0 documentation)](https://docs.mongodb.com/manual/tutorial/enable-authentication/)
1. Start MongoDB without access control
    Open cmd and then execute these commands
    ```
    cd <mongodb installation dir (e.g. "c:\Program Files\MongoDB\Server\4.0\bin")>
    mongo
    ```
2. Create the user administrator
    ```
    use admin
    db.createUser(
        {
            user: "admin",
            pwd: "2dm1np244",
            roles: ["root"]
        }
    )
    ```
    [root role (MongoDB 4.0 documentation)](https://docs.mongodb.com/manual/reference/built-in-roles/#root)
3. Update configuration
[security.authorization option (MongoDB 4.0 documentation)](https://docs.mongodb.com/manual/reference/configuration-options/#security.authorization)
    Open "<mongodb installation dir ..\bin\mongod.cfg" and add next lines to config file:
    ```
    security:
        authorization: enabled
    ```
4. Re-start the MongoDB service
    Open cmd with admin rights and execute:
    ```
    net stop mongodb
    net start mongodb
    ```
### 2. Create xyzApp user
1. Start mongo shell with admin credentials
    Open cmd
    ```
    cd <mongodb installation dir (e.g. "c:\Program Files\MongoDB\Server\4.0\bin")>
    mongo -u admin -p 2dm1np244 --authenticationDatabase admin
    ```
2. Create xyzApp db user for webApp
    ```
    use xyzApp
    db.createUser(
        {
            user: "xyzAdmin",
            pwd: "xyzP244",
            roles: ["readWrite"]
        }
    )
    ```
    [readWrite role (MongoDB 4.0 documentation)](https://docs.mongodb.com/manual/reference/built-in-roles/#readWrite)


