IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Orders' AND xtype = 'U')
BEGIN
	CREATE TABLE Orders (
		Id INT IDENTITY(1,1) PRIMARY KEY,
		CustomerName NVARCHAR(100) NOT NULL,
		ProductName nvarchar(200) not null,
		OrderDate DATETIME NOT NULL,
		Status NVARCHAR(50) NOT NULL
	);
END
