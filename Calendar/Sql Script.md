
How to Create a FOR Loop in SQL ServerT-SQL Enhancements in SQL Server
```sql
DECLARE @i TINYINT = 0;
DECLARE @length TINYINT = 10;
 
WHILE @i <= @length
BEGIN
 
    PRINT @i;
    SET @i += 1;
END
```

## Backup Table
```sql
DECLARE @name VARCHAR(50) -- database name  
DECLARE @path VARCHAR(256) -- path for backup files  
DECLARE @fileName VARCHAR(256)= getDate() -- filename for backup  
DECLARE @fileDate VARCHAR(20) -- used for file name
 
-- specify database backup directory
SET @path = 'F:\Database Backup of my pc 24 03 2021\'  
 
-- specify filename format
SELECT @fileDate = CONVERT(VARCHAR(20),GETDATE(),112) 
 
DECLARE db_cursor CURSOR READ_ONLY FOR  
SELECT name 
FROM master.sys.databases 
WHERE name NOT IN ('master','model','msdb','tempdb')  -- exclude these databases
AND state = 0 -- database is online
AND is_in_standby = 0 -- database is not read only for log shipping
 
OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @name   
 
WHILE @@FETCH_STATUS = 0   
BEGIN   
   SET @fileName = @path + @name + '_' + @fileDate + '.BAK'  
   BACKUP DATABASE @name TO DISK = @fileName  
 
   FETCH NEXT FROM db_cursor INTO @name   
END   

 
CLOSE db_cursor   
DEALLOCATE db_cursor
```


## Get Single Table Paged data
```sql
create proc [dbo].[GET_SINGLE_TABLE_PAGED_LOOKUPDATA]
@Table varchar(1000),
@ColumnList varchar(max),
@WhereClause varchar(max),
@OrderBy varchar (max)  ,
@PageIndex bigint,
@PageSize bigint

as

DECLARE @PageLowerBound int
DECLARE @PageUpperBound int

set @PageIndex=@PageIndex-1
SET @PageLowerBound = @PageSize * @PageIndex
SET @PageUpperBound = @PageLowerBound + @PageSize

DECLARE @SQL AS nvarchar(MAX)

SET @SQL = 'WITH Page'+CONVERT(nvarchar, @Table)+' AS ('
SET @SQL = @SQL + ' SELECT'

SET @SQL = @SQL + ' ROW_NUMBER() OVER (ORDER BY ' + @OrderBy + ') as RowIndex, '
SET @SQL = @SQL + + CONVERT(varchar(max), @ColumnList)
SET @SQL = @SQL + ' FROM ['+ CONVERT(nvarchar, @Table)+']'
IF LEN(@WhereClause) > 0
BEGIN
	SET @SQL = @SQL + ' WHERE ' + @WhereClause
END
SET @SQL = @SQL + ' ) SELECT * FROM Page'+CONVERT(nvarchar, @Table)
--SET @SQL = @SQL + ' WHERE RowIndex > ' + CONVERT(nvarchar, @PageLowerBound)
--IF @PageSize > 0
--BEGIN
--	SET @SQL = @SQL + ' AND RowIndex <= ' + CONVERT(nvarchar, @PageUpperBound)
--END
SET @SQL = @SQL + ' ORDER BY ' + @OrderBy

EXEC sp_executesql @SQL
--select @SQL
-- get row count
SET @SQL = 'SELECT COUNT(*) AS TotalRowCount'
SET @SQL = @SQL + ' FROM ['+ CONVERT(nvarchar, @Table)+']'
IF LEN(@WhereClause) > 0
BEGIN
	SET @SQL = @SQL + ' WHERE ' + @WhereClause
END
EXEC sp_executesql @SQL


create proc [dbo].[GET_SINGLE_TABLE_PAGED_LOOKUPDATA]
@Table varchar(1000),
@ColumnList varchar(max),
@WhereClause varchar(max),
@OrderBy varchar (max)  ,
@PageIndex bigint,
@PageSize bigint

as

DECLARE @PageLowerBound int
DECLARE @PageUpperBound int

set @PageIndex=@PageIndex-1
SET @PageLowerBound = @PageSize * @PageIndex
SET @PageUpperBound = @PageLowerBound + @PageSize

DECLARE @SQL AS nvarchar(MAX)

SET @SQL = 'WITH Page'+CONVERT(nvarchar, @Table)+' AS ('
SET @SQL = @SQL + ' SELECT'

SET @SQL = @SQL + ' ROW_NUMBER() OVER (ORDER BY ' + @OrderBy + ') as RowIndex, '
SET @SQL = @SQL + + CONVERT(varchar(max), @ColumnList)
SET @SQL = @SQL + ' FROM ['+ CONVERT(nvarchar, @Table)+']'
IF LEN(@WhereClause) > 0
BEGIN
	SET @SQL = @SQL + ' WHERE ' + @WhereClause
END
SET @SQL = @SQL + ' ) SELECT * FROM Page'+CONVERT(nvarchar, @Table)
--SET @SQL = @SQL + ' WHERE RowIndex > ' + CONVERT(nvarchar, @PageLowerBound)
--IF @PageSize > 0
--BEGIN
--	SET @SQL = @SQL + ' AND RowIndex <= ' + CONVERT(nvarchar, @PageUpperBound)
--END
SET @SQL = @SQL + ' ORDER BY ' + @OrderBy

EXEC sp_executesql @SQL
--select @SQL
-- get row count
SET @SQL = 'SELECT COUNT(*) AS TotalRowCount'
SET @SQL = @SQL + ' FROM ['+ CONVERT(nvarchar, @Table)+']'
IF LEN(@WhereClause) > 0
BEGIN
	SET @SQL = @SQL + ' WHERE ' + @WhereClause
END
EXEC sp_executesql @SQL
```