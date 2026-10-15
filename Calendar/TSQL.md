
SELECT p.ProductNumber, AVG(sod.UnitPrice) FROM Production.Product AS p INNER JOIN Sales.SalesOrderDetail AS sod ON p.ProductID = sod.ProductID GROUP BY p.ProductNumber HAVING p.ProductNumber LIKE 'L%';

![[Pasted image 20260917005453.png]]
![[Pasted image 20260917005737.png]]

![[Pasted image 20260917005927.png]]