/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 100132
Source Host           : localhost:3306
Source Database       : newwebsubcontext

Target Server Type    : MYSQL
Target Server Version : 100132
File Encoding         : 65001

Date: 2019-06-20 22:23:12
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for address
-- ----------------------------
DROP TABLE IF EXISTS `address`;
CREATE TABLE `address` (
  `addresskey` int(10) NOT NULL AUTO_INCREMENT,
  `Address1` varchar(255) NOT NULL,
  `Address2` varchar(255) DEFAULT NULL,
  `City` varchar(255) NOT NULL,
  `State` varchar(255) NOT NULL,
  `Zipcode` varchar(255) NOT NULL,
  `Country` varchar(255) DEFAULT NULL,
  `Address_Created` datetime DEFAULT NULL,
  `Address_Modified` datetime DEFAULT NULL,
  PRIMARY KEY (`addresskey`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for subscriptions
-- ----------------------------
DROP TABLE IF EXISTS `subscriptions`;
CREATE TABLE `subscriptions` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) DEFAULT NULL,
  `CustomerId` varchar(255) DEFAULT NULL,
  `SubscriptionId` varchar(255) DEFAULT NULL,
  `Subscription_Started` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Subscription_Ended` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for useraccounts
-- ----------------------------
DROP TABLE IF EXISTS `useraccounts`;
CREATE TABLE `useraccounts` (
  `UserID` int(10) NOT NULL AUTO_INCREMENT,
  `FirstName` varchar(255) NOT NULL,
  `LastName` varchar(255) NOT NULL,
  `UserName` varchar(255) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `CompanyName` varchar(255) DEFAULT NULL,
  `PhoneNumber` varchar(50) NOT NULL,
  `Account_Created` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Account_Modified` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `Hashed_Token` varchar(255) DEFAULT NULL,
  `Role` enum('Admin','Customer') DEFAULT 'Customer',
  `AddressId` int(10) NOT NULL,
  PRIMARY KEY (`UserID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
