# cs-large-db
C# project for large operations on a database

Properly chunks read and write operations. Includes pagination. Does basic validations and integrity checking on the data. It requires a locally set up MySQL server and database with a person table. The person table can be created by running:

```
    CREATE TABLE `<database>`.`person` (
    `idperson` INT NOT NULL AUTO_INCREMENT,
    `name` VARCHAR(45) NOT NULL,
    `date` VARCHAR(45) NOT NULL,
    `notes` VARCHAR(45) NOT NULL,
    PRIMARY KEY (`idperson`));
```

You can run the application by cloning the repo and simply calling it if you have dotnet installed: dotnet run .\ConsoleApp.csproj <user> <password> <database>
