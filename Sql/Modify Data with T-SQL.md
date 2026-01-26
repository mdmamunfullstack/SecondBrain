![[Pasted image 20260127012113.png]]![[Pasted image 20260127012230.png]] ![[Pasted image 20260127012251.png]] ![[Pasted image 20260127012321.png]]
![[Pasted image 20260127012611.png]] 
![[Pasted image 20260127012634.png]] 
![[Pasted image 20260127012746.png]]
![[Pasted image 20260127012807.png]]

![[Pasted image 20260127012828.png]] 
#   OutPut Keyword

```sql
DELETE FROM ArchivableOrders
OUTPUT deleted.OrderID, deleted.OrderDate, deleted.Status -- The receipt
WHERE Status IN ('Shipped', 'Cancelled')
AND OrderDate < DATEADD(YEAR, -2, GETDATE());
```

#SQL 