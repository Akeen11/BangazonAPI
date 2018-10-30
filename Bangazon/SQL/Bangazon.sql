DELETE FROM OrderProduct;
DELETE FROM ComputerEmployee;
DELETE FROM EmployeeTraining;
DELETE FROM Employee;
DELETE FROM TrainingProgram;
DELETE FROM Computer;
DELETE FROM Department;
DELETE FROM [Order];
DELETE FROM PaymentType;
DELETE FROM Product;
DELETE FROM ProductType;
DELETE FROM Customer;


ALTER TABLE Employee DROP CONSTRAINT [FK_EmployeeDepartment];
ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Employee];
ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Computer];
ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Employee];
ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Training];
ALTER TABLE Product DROP CONSTRAINT [FK_Product_ProductType];
ALTER TABLE Product DROP CONSTRAINT [FK_Product_Customer];
ALTER TABLE PaymentType DROP CONSTRAINT [FK_PaymentType_Customer];
ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Customer];
ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Payment];
ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Product];
ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Order];


DROP TABLE IF EXISTS OrderProduct;
DROP TABLE IF EXISTS ComputerEmployee;
DROP TABLE IF EXISTS EmployeeTraining;
DROP TABLE IF EXISTS Employee;
DROP TABLE IF EXISTS TrainingProgram;
DROP TABLE IF EXISTS Computer;
DROP TABLE IF EXISTS Department;
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS PaymentType;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS ProductType;
DROP TABLE IF EXISTS Customer;


CREATE TABLE Department (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL,
	Budget 	INTEGER NOT NULL
);

CREATE TABLE Employee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	DepartmentId INTEGER NOT NULL,
	IsSuperVisor BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);

CREATE TABLE Computer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	PurchaseDate DATETIME NOT NULL,
	DecomissionDate DATETIME
);

CREATE TABLE ComputerEmployee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	ComputerId INTEGER NOT NULL,
	AssignDate DATETIME NOT NULL,
	UnassignDate DATETIME,
    CONSTRAINT FK_ComputerEmployee_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_ComputerEmployee_Computer FOREIGN KEY(ComputerId) REFERENCES Computer(Id)
);


CREATE TABLE TrainingProgram (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,
	MaxAttendees INTEGER NOT NULL
);

CREATE TABLE EmployeeTraining (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);

CREATE TABLE ProductType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL
);

CREATE TABLE Customer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL
);

CREATE TABLE Product (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	ProductTypeId INTEGER NOT NULL,
	CustomerId INTEGER NOT NULL,
	Price INTEGER NOT NULL,
	Title VARCHAR(255) NOT NULL,
	[Description] VARCHAR(255) NOT NULL,
	Quantity INTEGER NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);


CREATE TABLE PaymentType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	AcctNumber INTEGER NOT NULL,
	[Name] VARCHAR(55) NOT NULL,
	CustomerId INTEGER NOT NULL,
    CONSTRAINT FK_PaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);

CREATE TABLE [Order] (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	CustomerId INTEGER NOT NULL,
	PaymentTypeId INTEGER,
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_Order_Payment FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
);

CREATE TABLE OrderProduct (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	OrderId INTEGER NOT NULL,
	ProductId INTEGER NOT NULL,
    CONSTRAINT FK_OrderProduct_Product FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_OrderProduct_Order FOREIGN KEY(OrderId) REFERENCES [Order](Id)
);

--Aaron customer insert 
INSERT INTO Customer 
    (FirstName, LastName) 
VALUES ('FirstName1', 'FirstName1')
;

INSERT INTO Customer
    (FirstName, LastName)
VALUES ('FirstName2', 'LastName2')
;

INSERT INTO Customer
    (FirstName, LastName)
VALUES ('FirstName3', 'LastName3')
;

INSERT INTO Customer
    (FirstName, LastName)
VALUES ('FirstName4', 'LastName4')
;

--Aaron insert into ProductType
INSERT INTO ProductType
    (Name)
VALUES ('Book')
;

INSERT INTO ProductType
    (Name)
VALUES ('Clothing')
;

INSERT INTO ProductType
    (Name)
VALUES ('Kitchen Appliance')
;

INSERT INTO ProductType
    (Name)
VALUES ('Vehicle')
;


--Aaron insert into Product 
INSERT INTO Product
    (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
VALUES (1, 2, 100, 'Harry Potter and the Chamber of Secrets', 'First Edition', 2)
;

INSERT INTO Product
    (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
VALUES (2, 4, 250, 'Louie Vuitton Belt', 'Saucy', 4)
;

INSERT INTO Product
    (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
VALUES (3, 1, 25, 'Knife', 'Sharp', 50)
;

INSERT INTO Product
    (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
VALUES (4, 3, 7500, 'Corvette', 'Great Condition', 1)
;

--Aaron Insert into PaymentType

INSERT INTO PaymentType
    (AcctNumber, Name, CustomerId)
VALUES
    (846724, 'Visa', 1)
;

INSERT INTO PaymentType
    (AcctNumber, Name, CustomerId)
VALUES
    (915387, 'MasterCard', 2)
;

INSERT INTO PaymentType
    (AcctNumber, Name, CustomerId)
VALUES
    (017338, 'MasterCard', 3)
;

INSERT INTO PaymentType
    (AcctNumber, Name, CustomerId)
VALUES
    (156483, 'Discover', 4)
;

--Gretchen Insert into Department 

INSERT INTO Department
(Name,Budget)
VALUES
('Accounting', 300000);

INSERT INTO Department
(Name,Budget)
VALUES
('CustomerService', 250000);

INSERT INTO Department
(Name,Budget)
VALUES
('Training', 125000);

INSERT INTO Department
(Name,Budget)
VALUES
('Shipping', 500000);

--Gretchen Insert into TrainingProgram
INSERT INTO TrainingProgram
(StartDate,EndDate,MaxAttendees)
Values
('12-15-2018','12-20-2018', 25);

INSERT INTO TrainingProgram
(StartDate,EndDate,MaxAttendees)
Values
('09-15-2018','09-20-2018', 25);

INSERT INTO TrainingProgram
(StartDate,EndDate,MaxAttendees)
Values
('11-15-2018','11-20-2018', 25);

INSERT INTO TrainingProgram
(StartDate,EndDate,MaxAttendees)
Values
('1-15-2019','1-20-2019', 25);


--Kayla Computer insert statements 
INSERT INTO Computer
	(PurchaseDate, DecomissionDate)
	VALUES ('2018-06-12 10:34:09 PM', NULL)
;

INSERT INTO Computer
	(PurchaseDate, DecomissionDate)
	VALUES ('2018-09-30 10:34:09 PM', NULL)
;

INSERT INTO Computer
	(PurchaseDate, DecomissionDate)
	VALUES ('2016-2-23 10:34:09 PM', '2018-12-3 12:42:09 PM')
;

INSERT INTO Computer
	(PurchaseDate, DecomissionDate)
	VALUES ('2015-12-2 10:34:09 PM', '2017-3-23 09:03:09 PM')
;

--Kayla Employee insert statements

INSERT INTO Employee 
	(FirstName, LastName, DepartmentId, IsSuperVisor)
	VALUES ('FirstName1', 'LastName1', 1, 0)
;

INSERT INTO Employee 
	(FirstName, LastName, DepartmentId, IsSuperVisor)
	VALUES ('FirstName2', 'LastName2', 2, 0)
;

INSERT INTO Employee 
	(FirstName, LastName, DepartmentId, IsSuperVisor)
	VALUES ('FirstName3', 'LastName3', 3, 1)
;

INSERT INTO Employee 
	(FirstName, LastName, DepartmentId, IsSuperVisor)
	VALUES ('FirstName4', 'LastName4', 4, 1)
;

--Kayla Insert into ComputerEmployee

INSERT INTO ComputerEmployee
	(EmployeeId, ComputerId, AssignDate, UnassignDate)
	VALUES (1, 1, '2015-12-2 10:34:09 PM', '2018-2-23 01:34:09 PM')
;

INSERT INTO ComputerEmployee
	(EmployeeId, ComputerId, AssignDate, UnassignDate)
	VALUES (2, 2, '2017-12-2 04:34:09 AM', '2018-12-3 01:34:09 PM')
;

INSERT INTO ComputerEmployee
	(EmployeeId, ComputerId, AssignDate, UnassignDate)
	VALUES (3, 3, '2016-11-2 09:23:09 PM', NULL)
;

INSERT INTO ComputerEmployee
	(EmployeeId, ComputerId, AssignDate, UnassignDate)
	VALUES (4, 4, '2012-10-2 07:12:23 PM', NULL)
;

--Kayla Insert into order 

INSERT INTO [Order]
	(CustomerId, PaymentTypeId)
	VALUES (1, 1)
;

INSERT INTO [Order]
	(CustomerId, PaymentTypeId)
	VALUES (2, 2)
;

INSERT INTO [Order]
	(CustomerId, PaymentTypeId)
	VALUES (3, 3)
;

INSERT INTO [Order]
	(CustomerId, PaymentTypeId)
	VALUES (4, 4)
;

--Kayla Insert into OrderProduct

INSERT INTO OrderProduct
	(OrderId, ProductId)
	VALUES (1, 1)
;

INSERT INTO OrderProduct
	(OrderId, ProductId)
	VALUES (2, 2)
;

INSERT INTO OrderProduct
	(OrderId, ProductId)
	VALUES (3, 3)
;

INSERT INTO OrderProduct
	(OrderId, ProductId)
	VALUES (4, 4)
;

---Getchen Insert into EmployeeTraining

INSERT INTO EmployeeTraining
(EmployeeId, TrainingProgramId)
VALUES(1, 1);

INSERT INTO EmployeeTraining
(EmployeeId, TrainingProgramId)
VALUES(3, 2);

INSERT INTO EmployeeTraining
(EmployeeId, TrainingProgramId)
VALUES(4, 3);

INSERT INTO EmployeeTraining
(EmployeeId, TrainingProgramId)
VALUES(2, 4);