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

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ 'c64c716e-f453-11f0-b744-6f4a24d5c351:1-721';

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
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20260616045821_InitialMySQLCreate','10.0.9');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

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
  PRIMARY KEY (`Id`),
  KEY `FK_Bookings_Tours_TourId` (`TourId`),
  KEY `IX_Bookings_Tracking` (`ReferenceNumber`,`LeadEmail`),
  CONSTRAINT `FK_Bookings_Tours_TourId` FOREIGN KEY (`TourId`) REFERENCES `tours` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=66 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bookings`
--

LOCK TABLES `bookings` WRITE;
/*!40000 ALTER TABLE `bookings` DISABLE KEYS */;
INSERT INTO `bookings` VALUES (29,'JH-68993',18,'Alice Calot',23,'blueberry@gmail.com','09672189373','none',NULL,250000.00,'Validated','2026-09-18 13:50:57','/uploads/e393f34a-5a40-42bb-8386-447263e94040.png',1),(31,'JH-91744',18,'Frey Calot',23,'coaltfran@gmail.com','09672189373','none',NULL,2500.00,'Validated','2026-09-25 09:23:04',NULL,1),(65,'JH-95342',22,'fin',23,'hey@gmail.com','09672189373','none',NULL,4500.00,'Validated','2026-09-25 16:23:13','/uploads/054e1faa-8a59-4d65-b079-8acc3d74cf4b.jpeg',1);
/*!40000 ALTER TABLE `bookings` ENABLE KEYS */;
UNLOCK TABLES;

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
  PRIMARY KEY (`Id`),
  KEY `fk_tour_vendor` (`VendorId`),
  CONSTRAINT `fk_tour_vendor` FOREIGN KEY (`VendorId`) REFERENCES `vendors` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_Tours_Vendors_VendorId` FOREIGN KEY (`VendorId`) REFERENCES `vendors` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tours`
--

LOCK TABLES `tours` WRITE;
/*!40000 ALTER TABLE `tours` DISABLE KEYS */;
INSERT INTO `tours` VALUES (18,'Pangasinan Package','Bolinao','2026-09-09 10:12:00','2026-09-14 10:12:00',NULL,2500.00,0.00,2,2,0,'0001-01-01 00:00:00',NULL,NULL,'Archived','/images/540837d2-0963-4890-ae80-5716ffd09a63.jpg','patar beach',19,NULL,NULL,NULL,NULL,0,NULL,0x00,NULL),(19,'Sagada Highland Exploration','Sagada, Mountain Province','2026-10-01 04:13:00','2026-10-04 04:13:00',NULL,3500.00,0.00,14,0,0,'0001-01-01 00:00:00',NULL,NULL,'Archived','/images/390a76a5-06c2-4210-807b-5255c0764a4e.png','Sumaguing Cave, Hanging Coffins, Kiltepan Viewpoint',21,'2026-09-25 11:38:49',NULL,NULL,NULL,0,NULL,0x00,NULL),(20,'Sagada, Mountain Province Adventure Experience','Sagada, Mountain Province','2026-10-02 02:38:00','2026-10-05 02:38:00',NULL,4500.00,0.00,14,0,0,'0001-01-01 00:00:00',NULL,NULL,'Active','/images/314f318e-ba49-4eb8-b63d-52ec4826f887.jpg','Scenic View, Local Food Trip, Guided Tour, Souvenir Shopping',19,NULL,NULL,NULL,NULL,0,NULL,0x00,NULL),(21,'Basco, Batanes Adventure Experience','Basco, Batanes','2026-10-02 03:18:00','2026-10-05 03:18:00','3d2n beach tour',4500.00,0.00,14,0,0,'0001-01-01 00:00:00',NULL,NULL,'Archived','/images/2cd99538-f7d2-4f86-ac51-994e2d57fb55.jpeg','Scenic View, Local Food Trip, Guided Tour, Souvenir Shopping',21,'2026-09-26 04:52:11','Moderate','Mountain / Highlands','Hiking & Trekking',0,NULL,0x00,NULL),(22,'Sagada, Mountain Province Adventure Experience','Sagada, Mountain Province','2026-10-02 08:22:00','2026-10-05 08:22:00','3d2n beach tour',4500.00,0.00,14,1,0,'0001-01-01 00:00:00',NULL,NULL,'Active','/images/144bdcb6-ec3d-4566-8a47-4441e2744bcd.jpeg','Scenic View, Local Food Trip, Guided Tour, Souvenir Shopping',19,NULL,'Moderate','Mountain / Highlands','Hiking & Trekking',1,'2026-09-25 16:52:02',0x00,NULL);
/*!40000 ALTER TABLE `tours` ENABLE KEYS */;
UNLOCK TABLES;

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
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (16,'colberry@gmail.com','12345','admin',23,'female','Alice Calot','09672189373','Active');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

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
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendors`
--

LOCK TABLES `vendors` WRITE;
/*!40000 ALTER TABLE `vendors` DISABLE KEYS */;
INSERT INTO `vendors` VALUES (19,'Transport A','Transport','09603456743','\"30% booking deposit via Bank Transfer 7 days before tour date. 70% balance via ACH within 7 days post-trip, contingent on digital submission of signed Trip Ticket, fuel logs, and toll receipts. Flat-rate vehicle pricing applies; no per-passenger adjustments.\"','Active',22,2000.00),(20,'Transport B','Transport','09603456743','\"40% booking deposit via Bank Transfer 7 days before tour date. 70% balance via ACH within 7 days post-trip, contingent on digital submission of signed Trip Ticket, fuel logs, and toll receipts. Flat-rate vehicle pricing applies; no per-passenger adjustments.\"','Active',NULL,2000.00),(21,'Hotel B','Accommodation','09603456743','\"40% booking deposit via Bank Transfer 7 days before tour date. 70% balance via ACH within 7 days post-trip, contingent on digital submission of signed Trip Ticket, fuel logs, and toll receipts. Flat-rate vehicle pricing applies; no per-passenger adjustments.\"','Active',21,2000.00);
/*!40000 ALTER TABLE `vendors` ENABLE KEYS */;
UNLOCK TABLES;
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-26 17:57:41
