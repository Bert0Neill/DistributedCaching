-- MySQL dump 10.13  Distrib 8.0.36, for Win64 (x86_64)
--
-- Host: localhost    Database: employeedatabase
-- ------------------------------------------------------
-- Server version	8.0.37

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `employee`
--

DROP TABLE IF EXISTS `employee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employee` (
  `EmployeeID` int NOT NULL,
  `DepartmentID` int NOT NULL,
  `EmployeeName` varchar(50) NOT NULL,
  `JobTitle` varchar(50) NOT NULL,
  `ManagerID` int DEFAULT NULL,
  `HireDate` date NOT NULL,
  PRIMARY KEY (`EmployeeID`),
  KEY `fk_employee_department` (`DepartmentID`),
  CONSTRAINT `fk_employee_department` FOREIGN KEY (`DepartmentID`) REFERENCES `department` (`DepartmentID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employee`
--

LOCK TABLES `employee` WRITE;
/*!40000 ALTER TABLE `employee` DISABLE KEYS */;
INSERT INTO `employee` VALUES (1,1,'John Smith','Sales Manager',NULL,'2022-01-15'),(2,2,'Jane Doe','Marketing Coordinator',1,'2022-02-28'),(3,2,'Michael Johnson','Marketing Specialist',2,'2022-03-10'),(4,3,'Emily Brown','Financial Analyst',1,'2022-04-05'),(5,4,'David Williams','HR Manager',NULL,'2022-05-20'),(6,5,'Jennifer Martinez','IT Specialist',5,'2022-06-15'),(7,6,'Daniel Jones','Operations Manager',NULL,'2022-07-01'),(8,7,'Sarah Wilson','Research Analyst',7,'2022-08-12'),(9,8,'James Taylor','Customer Service Representative',7,'2022-09-25'),(10,9,'Emma Anderson','Administrator',7,'2022-10-30'),(11,3,'Employee11','JobTitle',8,'2023-02-08'),(12,10,'Employee12','JobTitle',11,'2022-08-20'),(13,7,'Employee13','JobTitle',1,'2022-09-19'),(14,8,'Employee14','JobTitle',11,'2020-05-08'),(15,6,'Employee15','JobTitle',4,'2023-01-27'),(16,9,'Employee16','JobTitle',9,'2023-12-27'),(17,7,'Employee17','JobTitle',13,'2020-09-17'),(18,6,'Employee18','JobTitle',5,'2020-10-27'),(20,6,'Employee20','JobTitle',14,'2017-03-24'),(22,10,'Employee22','JobTitle',10,'2024-02-26'),(325,3,'Employee325','JobTitle',174,'2019-02-03'),(326,2,'Employee326','JobTitle',232,'2021-02-28'),(327,2,'Employee327','JobTitle',62,'2018-02-13'),(328,9,'Employee328','JobTitle',311,'2024-03-05');
/*!40000 ALTER TABLE `employee` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-07-08 13:33:23
