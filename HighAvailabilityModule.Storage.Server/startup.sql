IF OBJECT_ID('DataTable') IS NOT NULL
	DROP TABLE DataTable;
GO
CREATE TABLE DataTable
(dpath nvarchar(50),
dkey nvarchar(50),
dvalue nvarchar(50),
dtype nvarchar(50)
);
GO

IF OBJECT_ID('GetDataEntry') IS NOT NULL
	DROP PROCEDURE GetDataEntry;
GO
CREATE PROCEDURE GetDataEntry
	@dpath nvarchar(50),
	@dkey nvarchar(50)
AS
	SET NOCOUNT ON
	SELECT dvalue, dtype FROM dbo.DataTable WHERE dpath = @dpath AND dkey = @dkey;
GO

IF OBJECT_ID('SetDataEntry') IS NOT NULL
	DROP PROCEDURE SetDataEntry;
GO
CREATE PROCEDURE SetDataEntry
	@dpath nvarchar(50),
	@dkey nvarchar(50),
	@dvalue nvarchar(50),
	@dtype nvarchar(50),
	@lastSeenValue nvarchar(50),
	@lastSeenType nvarchar(50)
AS
	SET NOCOUNT ON
	IF NOT EXISTS (SELECT * FROM dbo.DataTable WHERE dpath = @dpath AND dkey = @dkey)
		INSERT INTO dbo.DataTable (dpath, dkey, dvalue, dtype) 
		VALUES (@dpath, @dkey, @dvalue, @dtype);
	ELSE
		UPDATE dbo.DataTable
		SET dpath = @dpath, dkey = @dkey, dvalue = @dvalue, dtype = @dtype
		WHERE dpath = @dpath AND dkey = @dkey AND dvalue = @lastSeenValue AND dtype = @lastSeenType;
GO

IF OBJECT_ID('DeleteDataEntry') IS NOT NULL
	DROP PROCEDURE DeleteDataEntry;
GO
CREATE PROCEDURE DeleteDataEntry
	@dpath nvarchar(50),
	@dkey nvarchar(50),
	@lastSeenValue nvarchar(50),
	@lastSeenType nvarchar(50)
AS
	SET NOCOUNT ON
	DELETE dbo.DataTable WHERE dpath = @dpath AND dkey = @dkey AND dvalue = @lastSeenValue AND dtype = @lastSeenType;
GO

IF OBJECT_ID('EnumerateDataEntry') IS NOT NULL
	DROP PROCEDURE EnumerateDataEntry;
GO
CREATE PROCEDURE EnumerateDataEntry
	@dpath nvarchar(50)
AS
	SET NOCOUNT ON
	SELECT dkey FROM dbo.DataTable WHERE dpath = @dpath;
GO