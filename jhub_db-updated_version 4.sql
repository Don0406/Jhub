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

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ 'c64c716e-f453-11f0-b744-6f4a24d5c351:1-1443';

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
  `IsArchived` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_Bookings_Tours_TourId` (`TourId`),
  KEY `IX_Bookings_Tracking` (`ReferenceNumber`,`LeadEmail`),
  CONSTRAINT `FK_Bookings_Tours_TourId` FOREIGN KEY (`TourId`) REFERENCES `tours` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=162 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bookings`
--

LOCK TABLES `bookings` WRITE;
/*!40000 ALTER TABLE `bookings` DISABLE KEYS */;
INSERT INTO `bookings` VALUES (153,'JH-82743',42,'Frey Calot',12,'mango@gmail.com','09672189373','none',NULL,4500.00,'Validated','2026-09-30 16:27:52',NULL,1,'2026-09-30 16:27:52',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0x01,0x00),(154,'JH-83155',42,'Frey Calot',12,'mango@gmail.com','09672189373','none',NULL,4500.00,'Validated','2026-09-30 16:28:03',NULL,1,'2026-09-30 16:28:03','Requested','Personal / Medical Emergency','09677789373','hello refund po','2026-09-30 16:31:39','eto na refund mo','/uploads/refunds/0220da75-b4e2-4f6e-b7d4-6b4395013f05_sagada.webp','2026-09-30 16:31:10',0x01,0x01),(155,'JH-22750',41,'Frey Calot',12,'mango@gmail.com','09672189373','none',NULL,4500.00,'Validated','2026-09-30 19:43:46',NULL,1,'2026-09-30 19:43:46',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0x01,0x00),(156,'JH-72610',41,'Frey Calot',12,'mango@gmail.com','09672189373','none',NULL,450000.00,'Validated','2026-09-30 21:41:13',NULL,1,'2026-09-30 21:41:13',NULL,NULL,NULL,NULL,NULL,NULL,NULL,'2026-09-30 21:48:08',0x01,0x01),(157,'JH-99196',41,'Frey Calot',12,'mango@gmail.com','09672189373','none',NULL,450000.00,'Validated','2026-09-30 21:41:16',NULL,1,'2026-09-30 21:41:16',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0x01,0x01),(158,'JH-65021',41,'Frey Calot',12,'mango@gmail.com','09672189373','none',NULL,450000.00,'Validated','2026-09-30 21:41:21',NULL,1,'2026-09-30 21:41:21','Requested','Personal / Medical Emergency','Gcash - 09888888888',NULL,'2026-09-30 21:47:38','eto na refund','/uploads/refunds/7f1b1bb2-8683-4099-accc-85ba2a481a8a_sagada.webp','2026-09-30 21:47:24',0x01,0x01),(159,'JH-12041',44,'Frey Calot',12,'mango@gmail.com','09672189373','none',NULL,4500.00,'Rejected','2026-09-30 21:49:32',NULL,1,'2026-09-30 21:49:32',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0x00,0x01),(160,'JH-25064',45,'Frey Calot',12,'mango@gmail.com','09672189373','none',NULL,4500.00,'Validated','2026-10-01 10:17:43',NULL,1,'2026-10-01 10:17:43','Requested','Personal / Medical Emergency','Gcash - 09888888888','hello gusto ko ng refund','2026-10-01 10:21:36','hello eto na refund','/uploads/refunds/c16284f1-8022-4914-be76-385bfb502df7_sagada.webp','2026-10-01 10:20:20',0x01,0x01),(161,'JH-77848',45,'Frey Calot',12,'mango@gmail.com','09672189373','none','Ralph',900000.00,'Rejected','2026-10-01 10:22:52',NULL,2,'2026-10-01 10:22:52',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0x00,0x01);
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
  `IsComplete` tinyint(1) NOT NULL DEFAULT '0',
  `CompletedAt` datetime DEFAULT NULL,
  `SlotsFilledAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_tour_vendor` (`VendorId`),
  CONSTRAINT `fk_tour_vendor` FOREIGN KEY (`VendorId`) REFERENCES `vendors` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_Tours_Vendors_VendorId` FOREIGN KEY (`VendorId`) REFERENCES `vendors` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=47 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tours`
--

LOCK TABLES `tours` WRITE;
/*!40000 ALTER TABLE `tours` DISABLE KEYS */;
INSERT INTO `tours` VALUES (41,'Sagada, Mountain Province Adventure Experience','Sagada, Mountain Province','2026-10-06 15:22:00','2026-10-09 15:22:00','3d2n mountain tour',450000.00,2000.00,4,0,0,'0001-01-01 00:00:00',NULL,NULL,'Archived','/images/default.jpg','Scenic View, Local Food Trip, Guided Tour, Souvenir Shopping',20,'2026-09-30 21:42:25','Moderate','Mountain / Highlands','Hiking & Trekking',0,NULL,0x00,NULL,0,NULL,NULL),(42,'Basco, Batanes Adventure Experience','Basco, Batanes','2026-10-06 16:34:00','2026-10-09 16:34:00','3d2n',4500.00,2000.00,4,0,0,'0001-01-01 00:00:00',NULL,NULL,'Archived','/images/53c36c50-0c15-462b-be4b-aad8b5f7b836.jpeg','Scenic View, Local Food Trip, Guided Tour, Souvenir Shopping',19,'2026-09-30 16:33:40','Moderate','Mountain / Highlands','Hiking & Trekking',1,'2026-09-30 16:33:27',0x00,NULL,0,NULL,NULL),(43,'Basco, Batanes Adventure Experience','Basco, Batanes','2026-10-07 08:34:00','2026-10-10 08:34:00','3d2n beach tour',4500.00,2000.00,4,0,0,'0001-01-01 00:00:00',NULL,NULL,'Archived','/images/6835586f-1fdb-4430-84b3-81a1bd8443d6.jpeg','Scenic View, Local Food Trip, Guided Tour, Souvenir Shopping',19,'2026-09-30 16:36:51','Moderate','Mountain / Highlands','Hiking & Trekking',0,NULL,0x01,'2026-09-30 16:36:51',0,NULL,NULL),(44,'Basco, Batanes Adventure Experience','Basco, Batanes','2026-10-07 08:37:00','2026-10-10 08:37:00','3d2n beach tour',4500.00,2000.00,4,0,0,'0001-01-01 00:00:00',NULL,NULL,'Archived','/images/15dc50c0-cf91-4415-8493-96cac0483507.jpeg','Scenic View, Local Food Trip, Guided Tour, Souvenir Shopping',19,'2026-09-30 21:44:43','Moderate','Mountain / Highlands','Hiking & Trekking',0,NULL,0x01,'2026-09-30 21:44:43',0,NULL,NULL),(45,'Sagada, Mountain Province Adventure Experience','Sagada, Mountain Province','2026-10-07 13:43:00','2026-10-10 13:43:00','3d2n beach tour',4500.00,2000.00,14,0,0,'0001-01-01 00:00:00',NULL,NULL,'Active','/images/db6431fc-6909-4a81-9cc3-daaf69cb98c1.webp','Scenic View, Local Food Trip, Guided Tour, Souvenir Shopping',19,NULL,'Moderate','Mountain / Highlands','Hiking & Trekking',0,NULL,0x00,NULL,0,NULL,NULL),(46,'Basco, Batanes Adventure Experience','Basco, Batanes','2026-10-07 13:45:00','2026-10-10 13:45:00','3d2n beach tour',4500.00,2000.00,4,0,0,'0001-01-01 00:00:00',NULL,NULL,'Active','/images/00caff7d-7640-4e08-a6e6-dbfdf561db80.jpeg','Scenic View, Local Food Trip, Guided Tour, Souvenir Shopping',20,NULL,'Moderate','Mountain / Highlands','Hiking & Trekking',0,NULL,0x00,NULL,0,NULL,NULL);
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
  `IsApproved` bit(1) NOT NULL DEFAULT b'0',
  `IsRejected` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (38,'colberry@gmail.com','12345','admin',18,'female','Alice Calot','09672189373','Active',0x00,0x00),(39,'caramel@gmail.com','12345','operator',18,'male','caramel','09672189373','Rejected',0x00,0x00),(40,'berry@gmail.com','hello','operator',18,'male','caramel','09672189373','Active',0x00,0x00),(42,'banana@gmail.com','hello','operator',18,'female','caramel','09672189373','Rejected',0x00,0x00),(43,'hello@gmail.com','12345','operator',18,'male','caramel','09672189373','Active',0x00,0x00);
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
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vendors`
--

LOCK TABLES `vendors` WRITE;
/*!40000 ALTER TABLE `vendors` DISABLE KEYS */;
INSERT INTO `vendors` VALUES (19,'Transport A','Transport','09603456743','\"30% booking deposit via Bank Transfer 7 days before tour date. 70% balance via ACH within 7 days post-trip, contingent on digital submission of signed Trip Ticket, fuel logs, and toll receipts. Flat-rate vehicle pricing applies; no per-passenger adjustments.\"','Active',45,2000.00),(20,'Transport B','Transport','09603456743','\"40% booking deposit via Bank Transfer 7 days before tour date. 70% balance via ACH within 7 days post-trip, contingent on digital submission of signed Trip Ticket, fuel logs, and toll receipts. Flat-rate vehicle pricing applies; no per-passenger adjustments.\"','Active',46,2000.00),(21,'Hotel B','Accommodation','09603456743','\"40% booking deposit via Bank Transfer 7 days before tour date. 70% balance via ACH within 7 days post-trip, contingent on digital submission of signed Trip Ticket, fuel logs, and toll receipts. Flat-rate vehicle pricing applies; no per-passenger adjustments.\"','Active',NULL,2000.00),(22,'Hotel C','Accommodation','09603456743','\"40% booking deposit via Bank Transfer 7 days before tour date. 70% balance via ACH within 7 days post-trip, contingent on digital submission of signed Trip Ticket, fuel logs, and toll receipts. Flat-rate vehicle pricing applies; no per-passenger adjustments.\"','Active',NULL,2000.00);
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

-- Dump completed on 2026-10-01 19:03:46
