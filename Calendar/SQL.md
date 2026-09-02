- **JSON support**
    - **ISJSON** – test a string to see if it contains valid JSON
    - **JSON_VALUE** – extract a scalar value from a JSON string
    - **JSON_QUERY** – extracts an object or an array from a JSON string


![[Pasted image 20260903032945.png]]
## SQL Date Data Type

- The date data type is used to store only dates without the time. It comprises three main parts: the year, month, and day.
- This data type ranges from 0001-01-01 through 9999-12-31.
- The default format of a date value is yyyy-MM-dd.
```sql
DECLARE @date DATE SET @date = '2009-01-01 10:00:00'
 SELECT @date as date_value Go
```


## SQL Datetime Data Type

- The datetime data type is used to store the date and time value.
- This data type ranges from 1753-01-01 00:00:00.000 to 9999-12-31 23:59:59.997 and allows storing three milliseconds fractions and the third fraction is rounded to 0, 3, or 7.
- The default format of a datetime value is yyyy-MM-dd HH:mm:ss.fff. 
```sql
DECLARE @date DATETIME
SET @date = '2009-01-01 10:00:00.122'
SELECT @date as datetime_value
```


## Timestamp

- The timestamp data type has nothing to do with dates. This datatype is also known as "rowversion".
- The timestamp column is automatically updated whenever the row is updated, and the value is monotonically increasing in the database.
- This data type may help implement data change capture logic in a data warehousing project.
### **SQL Server CURRENT_TIMESTAMP, GETDATE() and GETUTCDATE() Functions**

- CURRENT_TIMESTAMP - returns the date and time of the machine the SQL Server is running on
- GETDATE() - returns the date and time of the machine the SQL Server is running on
- GETUTCDATE() - returns the date and time of the machine the SQL Server is running on as UTC

### SQL Server DATEADD, EOMONTH, SWITCHOFFSET and TODATETIMEOFFSET Functions

- DATEADD - returns datepart with added interval as a datetime
- EOMONTH – returns last day of month of offset as type of start_date
- SWITCHOFFSET - returns date and time offset and time zone offset
- TODATETIMEOFFSET - returns date and time with time zone offset

### সংক্ষেপে

- `DATEDIFF()` নিজে ভুল নয়; **এটি কীভাবে কাজ করে সেটাই মূল বিষয়**।
- `DATEDIFF()` নির্দিষ্ট **date part-এর boundary** গণনা করে—যেমন `YEAR`, `MONTH`, `DAY`, `HOUR`।
- তাই মাত্র **১ সেকেন্ডের ব্যবধানেও** `DATEDIFF(MONTH, ...)` এর ফল `1` হতে পারে, যদি January → February boundary পার হয়।
- Global application-এ locale-dependent date format যেমন `31/01/2025` বা `01/31/2025` ব্যবহার করা ঝুঁকিপূর্ণ।
- তার পরিবর্তে **unambiguous numeric format** যেমন `YYYY-MM-DD` → `2025-01-31` ব্যবহার করা ভালো।
- Date সাধারণত তিনভাবে বিবেচনা করা যায়:
    - **Presentation** → মানুষের দেখার জন্য
    - **Storage** → Database-এ সংরক্ষণের জন্য
    - **Data interchange** → API/system-এর মধ্যে আদান-প্রদানের জন্য

**মূল শিক্ষা:**

> `DATEDIFF()` elapsed time নয়, নির্দিষ্ট date-part-এর **boundary difference** গণনা করে; আর global system-এ date-এর জন্য **standard/unambiguous format** ব্যবহার করা উচিত। 



![[Pasted image 20260903031609.png]]

![[Pasted image 20260903031505.png]]


![[AdobeExpressPhotos_9e18de8d0d37422f85e8186ed3811c8f_CopyEdited.png]]
## Literals
- DATE: YYYY-MM-DD; YYYYMMDD 
- TIME: hh:mm:ss.nnnnnnn(24-hour clock) 
- SMALLDATETIME: YYYY-MM-DD hh:mm:ss.nnn(24-hour clock) 
- DATETIME: YYYY-MM-DD hh:mm:ss.nnn(24-hour clock)
- DATETIME2: YYYY-MM-DD hh:mm:ss.nnnnnnn(24-hour clock)