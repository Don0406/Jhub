CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    PRIMARY KEY (`MigrationId`)
);

START TRANSACTION;
CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Email` varchar(150) NOT NULL,
    `Password` varchar(255) NOT NULL,
    `Role` varchar(50) NOT NULL,
    PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260616045821_InitialMySQLCreate', '10.0.9');

COMMIT;

