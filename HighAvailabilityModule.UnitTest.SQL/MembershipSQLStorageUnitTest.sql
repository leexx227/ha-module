USE HighAvailabilityStorage
GO
EXEC tSQLt.NewTestClass 'MembershipSQLStorageUnitTest';
GO

--TestMethod
USE HighAvailabilityStorage
GO
CREATE OR ALTER PROCEDURE MembershipSQLStorageUnitTest.[test GetDataEntry1]
AS
BEGIN
	IF OBJECT_ID('MembershipSQLStorageUnitTest_Excepted') IS NOT NULL
		DROP TABLE MembershipSQLStorageUnitTest_Excepted;
	DELETE dbo.DataTable;
	DECLARE @dpath nvarchar(50);
	DECLARE @dkey nvarchar(50);
	DECLARE @dvalue nvarchar(50);
	DECLARE @dtype nvarchar(50);
	DECLARE @TempTable TABLE
	(dvalue nvarchar(50),
	dtype nvarchar(50)
	);
	CREATE TABLE MembershipSQLStorageUnitTest_Excepted
	(dvalue nvarchar(50),
	dtype nvarchar(50)
	);

	SET @dpath = 'local\hpc';
	SET @dkey = 'A';
	SET @dvalue = '111';
	SET @dtype = 'System.String';

	INSERT INTO dbo.DataTable(dpath, dkey, dvalue, dtype) VALUES(@dpath, @dkey, @dvalue, @dtype);
	INSERT INTO @TempTable EXEC dbo.GetDataEntry @dpath, @dkey;

	SELECT dvalue, dtype INTO MembershipSQLStorageUnitTest_Actual FROM @TempTable;
	INSERT INTO MembershipSQLStorageUnitTest_Excepted(dvalue, dtype) VALUES(@dvalue, @dtype);

	EXEC tSQLt.AssertEqualsTable MembershipSQLStorageUnitTest_Excepted, MembershipSQLStorageUnitTest_Actual;
END
GO

--TestMethod
USE HighAvailabilityStorage
GO
CREATE OR ALTER PROCEDURE MembershipSQLStorageUnitTest.[test SetDataEntry1]
AS
BEGIN
	IF OBJECT_ID('MembershipSQLStorageUnitTest_Excepted') IS NOT NULL
		DROP TABLE MembershipSQLStorageUnitTest_Excepted;
	DELETE dbo.DataTable;
	DECLARE @dpath nvarchar(50);
	DECLARE @dkey nvarchar(50);
	DECLARE @dvalue nvarchar(50);
	DECLARE @dtype nvarchar(50);
	DECLARE @lastSeenValue nvarchar(50);
	DECLARE @lastSeenType nvarchar(50);
	CREATE TABLE MembershipSQLStorageUnitTest_Excepted
	(dpath nvarchar(50),
	dkey nvarchar(50),
	dvalue nvarchar(50),
	dtype nvarchar(50)
	);

	SET @dpath = 'local\hpc';
	SET @dkey = 'A';
	SET @dvalue = '111';
	SET @dtype = 'System.String';
	SET @lastSeenValue = '';
	SET @lastSeenType = '';

	EXEC dbo.SetDataEntry @dpath, @dkey, @dvalue, @dtype, @lastSeenValue, @lastSeenType;
	SELECT dpath, dkey, dvalue, dtype INTO MembershipSQLStorageUnitTest_Actual FROM dbo.DataTable WHERE dpath = @dpath AND dkey = @dkey;
	INSERT INTO MembershipSQLStorageUnitTest_Excepted(dpath, dkey, dvalue, dtype) VALUES(@dpath, @dkey, @dvalue, @dtype);

	EXEC tSQLt.AssertEqualsTable MembershipSQLStorageUnitTest_Excepted, MembershipSQLStorageUnitTest_Actual;
END
GO

--TestMethod
USE HighAvailabilityStorage
GO
CREATE OR ALTER PROCEDURE MembershipSQLStorageUnitTest.[test SetDataEntry2]
AS
BEGIN
	IF OBJECT_ID('MembershipSQLStorageUnitTest_Excepted') IS NOT NULL
		DROP TABLE MembershipSQLStorageUnitTest_Excepted;
	DELETE dbo.DataTable;
	DECLARE @dpath nvarchar(50);
	DECLARE @dkey nvarchar(50);
	DECLARE @dvalue1 nvarchar(50);
	DECLARE @dvalue2 nvarchar(50);
	DECLARE @dtype nvarchar(50);
	DECLARE @lastSeenValue nvarchar(50);
	DECLARE @lastSeenType nvarchar(50);
	DECLARE @TempTable TABLE
	(lastSeenValue nvarchar(50),
	lastSeenType nvarchar(50)
	);
	CREATE TABLE MembershipSQLStorageUnitTest_Excepted
	(dpath nvarchar(50),
	dkey nvarchar(50),
	dvalue nvarchar(50),
	dtype nvarchar(50)
	);

	SET @dpath = 'local\hpc';
	SET @dkey = 'A';
	SET @dvalue1 = '111';
	SET @dvalue2 = '222';
	SET @dtype = 'System.String';

	EXEC dbo.SetDataEntry @dpath, @dkey, @dvalue1, @dtype, '', '';
	INSERT INTO @TempTable EXEC dbo.GetDataEntry @dpath, @dkey;
	SELECT @lastSeenValue = lastSeenValue FROM @TempTable;
	SELECT @lastSeenType = lastSeenType FROM @TempTable;

	EXEC dbo.SetDataEntry @dpath, @dkey, @dvalue2, @dtype, @lastSeenValue, @lastSeenType;
	SELECT dpath, dkey, dvalue, dtype INTO MembershipSQLStorageUnitTest_Actual FROM dbo.DataTable WHERE dpath = @dpath AND dkey = @dkey;
	INSERT INTO MembershipSQLStorageUnitTest_Excepted(dpath, dkey, dvalue, dtype) VALUES(@dpath, @dkey, @dvalue2, @dtype);

	EXEC tSQLt.AssertEqualsTable MembershipSQLStorageUnitTest_Excepted, MembershipSQLStorageUnitTest_Actual;
END
GO

--TestMethod
USE HighAvailabilityStorage
GO
CREATE OR ALTER PROCEDURE MembershipSQLStorageUnitTest.[test SetDataEntry3]
AS
BEGIN
	IF OBJECT_ID('MembershipSQLStorageUnitTest_Excepted') IS NOT NULL
		DROP TABLE MembershipSQLStorageUnitTest_Excepted;
	DELETE dbo.DataTable;
	DECLARE @dpath nvarchar(50);
	DECLARE @dkey nvarchar(50);
	DECLARE @dvalue1 nvarchar(50);
	DECLARE @dvalue2 nvarchar(50);
	DECLARE @dvalue3 nvarchar(50);
	DECLARE @dtype nvarchar(50);
	DECLARE @lastSeenValue nvarchar(50);
	DECLARE @lastSeenType nvarchar(50);
	DECLARE @TempTable TABLE
	(lastSeenValue nvarchar(50),
	lastSeenType nvarchar(50)
	);
	CREATE TABLE MembershipSQLStorageUnitTest_Excepted
	(dpath nvarchar(50),
	dkey nvarchar(50),
	dvalue nvarchar(50),
	dtype nvarchar(50)
	);

	SET @dpath = 'local\hpc';
	SET @dkey = 'A';
	SET @dvalue1 = '111';
	SET @dvalue2 = '222';
	SET @dvalue3 = '333';
	SET @dtype = 'System.String';

	EXEC dbo.SetDataEntry @dpath, @dkey, @dvalue1, @dtype, '', '';
	INSERT INTO @TempTable EXEC dbo.GetDataEntry @dpath, @dkey;
	SELECT @lastSeenValue = lastSeenValue FROM @TempTable;
	SELECT @lastSeenType = lastSeenType FROM @TempTable;

	UPDATE dbo.DataTable SET dvalue = @dvalue3 WHERE dpath = @dpath AND dkey = @dkey;
	EXEC dbo.SetDataEntry @dpath, @dkey, @dvalue2, @dtype, @lastSeenValue, @lastSeenType;
	SELECT dpath, dkey, dvalue, dtype INTO MembershipSQLStorageUnitTest_Actual FROM dbo.DataTable WHERE dpath = @dpath AND dkey = @dkey;
	INSERT INTO MembershipSQLStorageUnitTest_Excepted(dpath, dkey, dvalue, dtype) VALUES(@dpath, @dkey, @dvalue3, @dtype);

	EXEC tSQLt.AssertEqualsTable MembershipSQLStorageUnitTest_Excepted, MembershipSQLStorageUnitTest_Actual;
END
GO

--TestMethod
USE HighAvailabilityStorage
GO
CREATE OR ALTER PROCEDURE MembershipSQLStorageUnitTest.[test SetDataEntry4]
AS
BEGIN
	IF OBJECT_ID('MembershipSQLStorageUnitTest_Excepted') IS NOT NULL
		DROP TABLE MembershipSQLStorageUnitTest_Excepted;
	DELETE dbo.DataTable;
	DECLARE @dpath nvarchar(50);
	DECLARE @dkey nvarchar(50);
	DECLARE @dvalue1 nvarchar(50);
	DECLARE @dvalue2 nvarchar(50);
	DECLARE @dtype1 nvarchar(50);
	DECLARE @dtype2 nvarchar(50);
	DECLARE @lastSeenValue nvarchar(50);
	DECLARE @lastSeenType nvarchar(50);
	DECLARE @TempTable TABLE
	(lastSeenValue nvarchar(50),
	lastSeenType nvarchar(50)
	);
	CREATE TABLE MembershipSQLStorageUnitTest_Excepted
	(dpath nvarchar(50),
	dkey nvarchar(50),
	dvalue nvarchar(50),
	dtype nvarchar(50)
	);

	SET @dpath = 'local\hpc';
	SET @dkey = 'A';
	SET @dvalue1 = '111';
	SET @dvalue2 = '222';
	SET @dtype1 = 'System.String';
	SET @dtype1 = 'System.Int32';

	EXEC dbo.SetDataEntry @dpath, @dkey, @dvalue1, @dtype1, '', '';
	INSERT INTO @TempTable EXEC dbo.GetDataEntry @dpath, @dkey;
	SELECT @lastSeenValue = lastSeenValue FROM @TempTable;
	SELECT @lastSeenType = lastSeenType FROM @TempTable;

	UPDATE dbo.DataTable SET dtype = @dtype2 WHERE dpath = @dpath AND dkey = @dkey;
	EXEC dbo.SetDataEntry @dpath, @dkey, @dvalue2, @dtype1, @lastSeenValue, @lastSeenType;
	SELECT dpath, dkey, dvalue, dtype INTO MembershipSQLStorageUnitTest_Actual FROM dbo.DataTable WHERE dpath = @dpath AND dkey = @dkey;
	INSERT INTO MembershipSQLStorageUnitTest_Excepted(dpath, dkey, dvalue, dtype) VALUES(@dpath, @dkey, @dvalue1, @dtype2);

	EXEC tSQLt.AssertEqualsTable MembershipSQLStorageUnitTest_Excepted, MembershipSQLStorageUnitTest_Actual;
END
GO

--TestMethod
USE HighAvailabilityStorage
GO
CREATE OR ALTER PROCEDURE MembershipSQLStorageUnitTest.[test DeleteDataEntry1]
AS
BEGIN
	IF OBJECT_ID('MembershipSQLStorageUnitTest_Excepted') IS NOT NULL
		DROP TABLE MembershipSQLStorageUnitTest_Excepted;
	DELETE dbo.DataTable;
	DECLARE @dpath nvarchar(50);
	DECLARE @dkey nvarchar(50);
	DECLARE @dvalue nvarchar(50);
	DECLARE @dtype nvarchar(50);
	DECLARE @lastSeenValue nvarchar(50);
	DECLARE @lastSeenType nvarchar(50);
	DECLARE @Actual int;
	DECLARE @TempTable TABLE
	(lastSeenValue nvarchar(50),
	lastSeenType nvarchar(50)
	);
	CREATE TABLE MembershipSQLStorageUnitTest_Excepted
	(dpath nvarchar(50),
	dkey nvarchar(50),
	dvalue nvarchar(50),
	dtype nvarchar(50)
	);

	SET @dpath = 'local\hpc';
	SET @dkey = 'A';
	SET @dvalue = '111';
	SET @dtype = 'System.String';

	EXEC dbo.SetDataEntry @dpath, @dkey, @dvalue, @dtype, '', '';
	INSERT INTO @TempTable EXEC dbo.GetDataEntry @dpath, @dkey;
	SELECT @lastSeenValue = lastSeenValue FROM @TempTable;
	SELECT @lastSeenType = lastSeenType FROM @TempTable;
	EXEC dbo.DeleteDataEntry @dpath, @dkey, @lastSeenValue, @lastSeenType;

	SELECT dpath, dkey, dvalue, dtype INTO MembershipSQLStorageUnitTest_Actual FROM dbo.DataTable WHERE dpath = @dpath AND dkey = @dkey;
	SELECT @Actual = COUNT(*) FROM dbo.DataTable WHERE dpath = @dpath AND dkey = @dkey;

	EXEC tSQLt.AssertEquals 0, @Actual;
END
GO

--TestMethod
USE HighAvailabilityStorage
GO
CREATE OR ALTER PROCEDURE MembershipSQLStorageUnitTest.[test EnumerateDataEntry1]
AS
BEGIN
	IF OBJECT_ID('MembershipSQLStorageUnitTest_Excepted') IS NOT NULL
		DROP TABLE MembershipSQLStorageUnitTest_Excepted;
	DELETE dbo.DataTable;
	DECLARE @dpath nvarchar(50);
	DECLARE @dkeyA nvarchar(50);
	DECLARE @dkeyB nvarchar(50);
	DECLARE @dvalue1 nvarchar(50);
	DECLARE @dvalue2 nvarchar(50);
	DECLARE @dtype1 nvarchar(50);
	DECLARE @dtype2 nvarchar(50);
	DECLARE @lastSeenValue nvarchar(50);
	DECLARE @lastSeenType nvarchar(50);
	DECLARE @TempTable TABLE
	(dkey nvarchar(50)
	);
	CREATE TABLE MembershipSQLStorageUnitTest_Excepted
	(dkey nvarchar(50)
	);

	SET @dpath = 'local\hpc';
	SET @dkeyA = 'A';
	SET @dkeyA = 'B';
	SET @dvalue1 = '111';
	SET @dvalue2 = '222';
	SET @dtype1 = 'System.String';
	SET @dtype1 = 'System.Int32';

	INSERT INTO dbo.DataTable (dpath, dkey, dvalue, dtype) VALUES(@dpath, @dkeyA, @dvalue1, @dtype1);
	INSERT INTO dbo.DataTable (dpath, dkey, dvalue, dtype) VALUES(@dpath, @dkeyB, @dvalue2, @dtype2);
	INSERT INTO @TempTable EXEC dbo.EnumerateDataEntry @dpath;

	SELECT dkey INTO MembershipSQLStorageUnitTest_Actual FROM @TempTable;
	INSERT INTO MembershipSQLStorageUnitTest_Excepted(dkey) VALUES(@dkeyA);
	INSERT INTO MembershipSQLStorageUnitTest_Excepted(dkey) VALUES(@dkeyB);

	EXEC tSQLt.AssertEqualsTable MembershipSQLStorageUnitTest_Excepted, MembershipSQLStorageUnitTest_Actual;
END
GO