-- MySQL dump 10.13  Distrib 9.5.0, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: jhub_db
-- ------------------------------------------------------
-- Server version	9.5.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ 'c64c716e-f453-11f0-b744-6f4a24d5c351:1-1085';

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `bookings`
--

DROP TABLE IF EXISTS `bookings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bookings` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ReferenceNumber` varchar(50) NOT NULL,
  `TourId` int NOT NULL,
  `LeadName` varchar(255) NOT NULL,
  `LeadAge` int NOT NULL,
  `LeadEmail` varchar(255) NOT NULL,
  `LeadPhone` varchar(50) NOT NULL,
  `SpecialCategory` varchar(50) NOT NULL,
  `CompanionNames` text,
  `TotalAmount` decimal(18,2) NOT NULL,
  `Status` varchar(50) NOT NULL DEFAULT 'Pending Validation',
  `BookingDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ProofOfPaymentUrl` text,
  `Slots` int NOT NULL DEFAULT '1',
  `BookedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `RefundStatus` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `RefundReason` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `RefundPayoutChannel` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `RefundNotes` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `RefundRequestedAt` datetime DEFAULT NULL,
  `RefundOperatorMessage` varchar(1000) DEFAULT NULL,
  `RefundProofPath` varchar(500) DEFAULT NULL,
  `RefundUpdatedAt` datetime DEFAULT NULL,
  `IsApproved` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_Bookings_Tours_TourId` (`TourId`),
  KEY `IX_Bookings_Tracking` (`ReferenceNumber`,`LeadEmail`),
  CONSTRAINT `FK_Bookings_Tours_TourId` FOREIGN KEY (`TourId`) REFERENCES `tours` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=102 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tours`
--

DROP TABLE IF EXISTS `tours`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tours` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `TourName` varchar(255) NOT NULL,
  `Destination` varchar(255) NOT NULL,
  `DepartureDate` datetime NOT NULL,
  `ReturnDate` datetime NOT NULL,
  `Description` text,
  `BasePrice` decimal(18,2) NOT NULL,
  `OperationalCost` decimal(18,2) NOT NULL,
  `TotalCapacity` int NOT NULL,
  `SlotsFilled` int NOT NULL DEFAULT '0',
  `MinParticipants` int NOT NULL,
  `Deadline` datetime NOT NULL,
  `VehicleAssignment` varchar(150) DEFAULT NULL,
  `AccommodationAssignment` varchar(150) DEFAULT NULL,
  `Status` varchar(50) NOT NULL DEFAULT 'Pending',
  `ImageUrl` longtext,
  `ItineraryHighlights` text,
  `VendorId` int DEFAULT NULL,
  `ArchivedAt` datetime DEFAULT NULL,
  `IntensityLevel` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `LandscapeType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Activities` text,
  `IsBoosted` tinyint(1) NOT NULL DEFAULT '0',
  `BoostedAt` datetime DEFAULT NULL,
  `IsCancelled` bit(1) NOT NULL DEFAULT b'0',
  `CancelledAt` datetime DEFAULT NULL,
  `IsComplete` tinyint(1) NOT NULL DEFAULT '0',
  `CompletedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_tour_vendor` (`VendorId`),
  CONSTRAINT `fk_tour_vendor` FOREIGN KEY (`VendorId`) REFERENCES `vendors` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_Tours_Vendors_VendorId` FOREIGN KEY (`VendorId`) REFERENCES `vendors` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Email` varchar(150) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Role` varchar(50) NOT NULL,
  `Age` int NOT NULL DEFAULT '0',
  `Sex` varchar(20) NOT NULL DEFAULT '',
  `FullName` varchar(255) NOT NULL DEFAULT '',
  `PhoneNumber` varchar(20) NOT NULL DEFAULT '',
  `Status` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Active',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `vendors`
--

DROP TABLE IF EXISTS `vendors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vendors` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `VendorName` varchar(255) NOT NULL,
  `ServiceType` varchar(100) NOT NULL,
  `ContactInfo` varchar(255) NOT NULL,
  `PaymentTerms` longtext,
  `Status` varchar(50) DEFAULT 'Active',
  `TourId` int DEFAULT NULL,
  `Payables` decimal(18,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`Id`),
  KEY `TourId` (`TourId`),
  CONSTRAINT `vendors_ibfk_1` FOREIGN KEY (`TourId`) REFERENCES `tours` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-29 15:04:58
