-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: laboratory
-- ------------------------------------------------------
-- Server version	5.7.21-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `administer`
--

DROP TABLE IF EXISTS `administer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `administer` (
  `Admin_no` char(5) NOT NULL,
  `Admin_password` char(12) NOT NULL,
  `Admin_name` char(10) NOT NULL,
  PRIMARY KEY (`Admin_no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `administer`
--

LOCK TABLES `administer` WRITE;
/*!40000 ALTER TABLE `administer` DISABLE KEYS */;
INSERT INTO `administer` VALUES ('90000','123456','王五');
/*!40000 ALTER TABLE `administer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `device`
--

DROP TABLE IF EXISTS `device`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `device` (
  `Dev_no` int(11) NOT NULL,
  `Lno` char(5) NOT NULL,
  `Dev_name` char(15) NOT NULL,
  `Available` tinyint(1) NOT NULL,
  PRIMARY KEY (`Dev_no`,`Lno`),
  KEY `Lno` (`Lno`),
  CONSTRAINT `device_ibfk_1` FOREIGN KEY (`Lno`) REFERENCES `lab` (`Lno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `device`
--

LOCK TABLES `device` WRITE;
/*!40000 ALTER TABLE `device` DISABLE KEYS */;
INSERT INTO `device` VALUES (1,'lab1','计算机',1),(2,'lab1','计算机',1),(3,'lab1','计算机',1),(4,'lab1','计算机',1),(5,'lab1','计算机',1),(6,'lab1','计算机',1),(7,'lab1','计算机',1),(8,'lab1','计算机',1),(9,'lab1','计算机',1),(10,'lab1','计算机',1),(11,'lab1','计算机',1),(12,'lab1','计算机',1),(13,'lab1','计算机',1),(14,'lab1','计算机',1),(15,'lab1','计算机',1),(16,'lab1','计算机',1),(17,'lab1','计算机',1),(18,'lab1','计算机',1),(19,'lab1','计算机',1),(20,'lab1','计算机',1),(21,'lab1','计算机',1),(22,'lab1','计算机',1),(23,'lab1','计算机',1),(24,'lab1','计算机',1),(25,'lab1','计算机',1);
/*!40000 ALTER TABLE `device` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lab`
--

DROP TABLE IF EXISTS `lab`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `lab` (
  `Lno` char(5) NOT NULL,
  `Lname` char(15) NOT NULL,
  `Addr` char(20) NOT NULL,
  `Dev_numbers` int(11) NOT NULL,
  PRIMARY KEY (`Lno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lab`
--

LOCK TABLES `lab` WRITE;
/*!40000 ALTER TABLE `lab` DISABLE KEYS */;
INSERT INTO `lab` VALUES ('lab1','机房','先骕楼X4313',25),('lab2','物联网实验室','先骕楼X6510',24);
/*!40000 ALTER TABLE `lab` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `my_order`
--

DROP TABLE IF EXISTS `my_order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `my_order` (
  `Order_no` int(11) NOT NULL AUTO_INCREMENT,
  `Lno` char(5) NOT NULL,
  `Sno` char(12) NOT NULL,
  `Dev_no` int(11) NOT NULL,
  `Order_time` datetime NOT NULL,
  `Start_course` int(11) NOT NULL,
  `End_course` int(11) NOT NULL,
  `All_course` int(11) DEFAULT NULL,
  PRIMARY KEY (`Order_no`),
  KEY `Lno` (`Lno`),
  KEY `Sno` (`Sno`),
  KEY `Dev_no` (`Dev_no`),
  KEY `Start_course` (`Start_course`),
  KEY `End_course` (`End_course`),
  CONSTRAINT `my_order_ibfk_1` FOREIGN KEY (`Lno`) REFERENCES `lab` (`Lno`),
  CONSTRAINT `my_order_ibfk_2` FOREIGN KEY (`Sno`) REFERENCES `student` (`Sno`),
  CONSTRAINT `my_order_ibfk_3` FOREIGN KEY (`Dev_no`) REFERENCES `device` (`Dev_no`),
  CONSTRAINT `my_order_ibfk_4` FOREIGN KEY (`Start_course`) REFERENCES `schedul` (`Course_no`),
  CONSTRAINT `my_order_ibfk_5` FOREIGN KEY (`End_course`) REFERENCES `schedul` (`Course_no`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `my_order`
--

LOCK TABLES `my_order` WRITE;
/*!40000 ALTER TABLE `my_order` DISABLE KEYS */;
INSERT INTO `my_order` VALUES (2,'lab1','201526204041',1,'2018-04-24 00:00:00',1,4,4),(9,'lab1','201526204041',1,'2018-04-24 00:00:00',6,8,3),(12,'lab1','201526204041',1,'2018-04-25 00:00:00',1,2,2),(15,'lab1','201526204041',1,'2018-04-25 00:00:00',3,4,2),(16,'lab1','201526204041',1,'2018-04-25 00:00:00',5,6,2),(17,'lab1','201526204041',1,'2018-04-25 00:00:00',7,8,2),(18,'lab1','201526204041',1,'2018-04-26 00:00:00',1,2,2),(19,'lab1','201526204041',1,'2018-04-26 00:00:00',3,4,2),(20,'lab1','201526204041',1,'2018-04-26 00:00:00',5,6,2),(21,'lab1','201526204041',1,'2018-04-26 00:00:00',7,8,2),(22,'lab1','201526204042',2,'2018-04-25 00:00:00',1,2,2),(23,'lab2','201526204041',1,'2018-04-25 00:00:00',1,2,2),(25,'lab1','201526204041',3,'2018-04-25 00:00:00',1,2,2),(26,'lab1','201526204041',4,'2018-04-25 00:00:00',1,2,2),(27,'lab1','201526204041',14,'2018-04-26 00:00:00',1,2,2),(28,'lab1','201526204041',2,'2018-04-26 00:00:00',1,2,2),(29,'lab1','201526204041',8,'2018-04-26 00:00:00',1,2,2),(30,'lab1','201526204041',1,'2018-04-27 00:00:00',1,2,2),(31,'lab1','201526204041',2,'2018-04-27 00:00:00',1,2,2),(32,'lab1','201526204042',3,'2018-04-27 00:00:00',1,2,2),(33,'lab1','201526204041',1,'2018-05-03 00:00:00',1,2,2),(34,'lab1','201526204044',2,'2018-05-03 00:00:00',1,2,2),(35,'lab1','201526204044',2,'2018-05-03 00:00:00',6,7,2),(36,'lab2','201526204044',2,'2018-05-03 00:00:00',6,7,2);
/*!40000 ALTER TABLE `my_order` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `schedul`
--

DROP TABLE IF EXISTS `schedul`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `schedul` (
  `Course_no` int(11) NOT NULL,
  `Moment` time NOT NULL,
  PRIMARY KEY (`Course_no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `schedul`
--

LOCK TABLES `schedul` WRITE;
/*!40000 ALTER TABLE `schedul` DISABLE KEYS */;
INSERT INTO `schedul` VALUES (1,'08:00:00'),(2,'08:50:00'),(3,'09:40:00'),(4,'10:30:00'),(5,'11:20:00'),(6,'14:00:00'),(7,'14:50:00'),(8,'15:40:00'),(9,'16:30:00');
/*!40000 ALTER TABLE `schedul` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `statistics`
--

DROP TABLE IF EXISTS `statistics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `statistics` (
  `Order_time` date NOT NULL,
  `Lno` char(5) NOT NULL,
  `Dev_no` int(11) NOT NULL,
  `Efficient` char(5) DEFAULT NULL,
  PRIMARY KEY (`Order_time`,`Lno`,`Dev_no`),
  KEY `Lno` (`Lno`),
  KEY `Dev_no` (`Dev_no`),
  CONSTRAINT `statistics_ibfk_1` FOREIGN KEY (`Lno`) REFERENCES `lab` (`Lno`),
  CONSTRAINT `statistics_ibfk_2` FOREIGN KEY (`Dev_no`) REFERENCES `device` (`Dev_no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `statistics`
--

LOCK TABLES `statistics` WRITE;
/*!40000 ALTER TABLE `statistics` DISABLE KEYS */;
INSERT INTO `statistics` VALUES ('2018-04-26','lab1',1,'89%'),('2018-04-26','lab1',2,'22%'),('2018-04-26','lab1',8,'22%'),('2018-04-26','lab1',14,'22%'),('2018-04-27','lab1',1,'22%'),('2018-04-27','lab1',2,'22%'),('2018-04-27','lab1',3,'22%'),('2018-05-03','lab1',1,'22%'),('2018-05-03','lab1',2,'44%'),('2018-05-03','lab2',2,'22%');
/*!40000 ALTER TABLE `statistics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `student`
--

DROP TABLE IF EXISTS `student`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `student` (
  `Sno` char(12) NOT NULL,
  `Sname` char(15) NOT NULL,
  `Sdept` char(15) NOT NULL,
  `Limit` int(5) DEFAULT '2',
  `Password` varchar(12) DEFAULT '123456',
  PRIMARY KEY (`Sno`),
  UNIQUE KEY `Sname` (`Sname`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `student`
--

LOCK TABLES `student` WRITE;
/*!40000 ALTER TABLE `student` DISABLE KEYS */;
INSERT INTO `student` VALUES ('201526204032','丁永琴','计算机信息工程学院',2,'123456'),('201526204034','钟琴','计算机信息工程学院',2,'123456'),('201526204036','吕大卫','计算机信息工程学院',2,'123456'),('201526204037','殷子豪','计算机信息工程学院',2,'123456'),('201526204039','黎业强','计算机信息工程学院',2,'123456'),('201526204041','郭爱斌','计算机信息工程学院',2,'123456'),('201526204042','何雅玲','计算机信息工程学院',2,'123456'),('201526204043','廖紫荆','计算机信息工程学院',2,'123456'),('201526204044','李理通','计算机信息工程学院',2,'123456'),('201526204046','壬瑞磊','计算机信息工程学院',2,'123456'),('201526204050','李鑫勇','计算机信息工程学院',2,'123456'),('201526204052','张梦圆','计算机信息工程学院',2,'123456'),('201526204054','郭非凡','计算机信息工程学院',2,'123456'),('201526204055','刘艳','计算机信息工程学院',2,'123456'),('201526204058','严露霞','计算机信息工程学院',2,'123456'),('201526204059','王文','计算机信息工程学院',2,'123456'),('201526204061','夏丹','计算机信息工程学院',2,'123456'),('201526204063','陈金','计算机信息工程学院',2,'123456'),('201526204064','俞铁阳','计算机信息工程学院',2,'123456');
/*!40000 ALTER TABLE `student` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `teacher`
--

DROP TABLE IF EXISTS `teacher`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `teacher` (
  `Tea_no` char(12) NOT NULL,
  `Tea_password` char(12) NOT NULL,
  `Tea_name` char(10) NOT NULL,
  PRIMARY KEY (`Tea_no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `teacher`
--

LOCK TABLES `teacher` WRITE;
/*!40000 ALTER TABLE `teacher` DISABLE KEYS */;
INSERT INTO `teacher` VALUES ('10000','123456','吴水秀'),('10001','123456','郑燚'),('10002','123456','张练兴'),('10003','123456','谢旭升'),('10004','123456','钟林辉');
/*!40000 ALTER TABLE `teacher` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'laboratory'
--

--
-- Dumping routines for database 'laboratory'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-05-04 13:08:21
