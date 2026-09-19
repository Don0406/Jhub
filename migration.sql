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

ALTER TABLE `Users` ADD `Age` int NOT NULL DEFAULT 0;

ALTER TABLE `Users` ADD `FullName` varchar(255) NOT NULL DEFAULT '';

ALTER TABLE `Users` ADD `PhoneNumber` varchar(20) NOT NULL DEFAULT '';

ALTER TABLE `Users` ADD `Sex` varchar(20) NOT NULL DEFAULT '';

ALTER TABLE `Users` ADD `Status` longtext NOT NULL;

CREATE TABLE `Bookings` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ReferenceNumber` longtext NOT NULL,
    `TourId` int NOT NULL,
    `LeadName` longtext NOT NULL,
    `LeadAge` int NOT NULL,
    `LeadEmail` longtext NOT NULL,
    `LeadPhone` longtext NOT NULL,
    `SpecialCategory` longtext NOT NULL,
    `CompanionNames` longtext NULL,
    `Slots` int NOT NULL,
    `TotalAmount` decimal(18,2) NOT NULL,
    `Status` longtext NOT NULL,
    `BookingDate` datetime(6) NOT NULL,
    `ProofOfPaymentUrl` longtext NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Tours` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `TourName` longtext NOT NULL,
    `Destination` longtext NOT NULL,
    `DepartureDate` datetime(6) NOT NULL,
    `ReturnDate` datetime(6) NOT NULL,
    `Description` longtext NULL,
    `BasePrice` decimal(18,2) NOT NULL,
    `OperationalCost` decimal(18,2) NOT NULL,
    `TotalCapacity` int NOT NULL,
    `SlotsFilled` int NOT NULL,
    `MinParticipants` int NOT NULL,
    `Deadline` datetime(6) NOT NULL,
    `VehicleAssignment` longtext NULL,
    `AccommodationAssignment` longtext NULL,
    `ImageUrl` longtext NULL,
    `ItineraryHighlights` longtext NULL,
    `Status` longtext NOT NULL,
    `IsBoosted` tinyint(1) NOT NULL,
    `BoostedUntil` datetime(6) NULL,
    `VendorId` int NULL,
    PRIMARY KEY (`Id`)
);

CREATE TABLE `Vendors` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VendorName` longtext NOT NULL,
    `ServiceType` longtext NOT NULL,
    `ContactInfo` longtext NOT NULL,
    `PaymentTerms` LONGTEXT NOT NULL,
    `Payables` decimal(18,2) NOT NULL,
    `Status` longtext NOT NULL,
    `TourId` int NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Vendors_Tours_TourId` FOREIGN KEY (`TourId`) REFERENCES `Tours` (`Id`)
);

CREATE INDEX `IX_Bookings_TourId` ON `Bookings` (`TourId`);

CREATE INDEX `IX_Tours_VendorId` ON `Tours` (`VendorId`);

CREATE INDEX `IX_Vendors_TourId` ON `Vendors` (`TourId`);

ALTER TABLE `Bookings` ADD CONSTRAINT `FK_Bookings_Tours_TourId` FOREIGN KEY (`TourId`) REFERENCES `Tours` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Tours` ADD CONSTRAINT `FK_Tours_Vendors_VendorId` FOREIGN KEY (`VendorId`) REFERENCES `Vendors` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260916154357_InitialCreate', '10.0.9');

COMMIT;

