
Normalization is a process we use to organize a database into tables and columns.

Reasons for Database Normalization

- Minimize duplicate data
- Minimize or avoid data modification issues
- simplify queries

Cardinality **Notation**

- zero or one-to-many **0..***
- one-to-many **1..***
- zero or one-to-one **0..1**
- one-to-one **1..1**

When we cover normalization we’ll use these diagrams to show how we organize the database.  I think it is important you become comfortable with these diagrams and concepts before we begin that journey. As such make sure you understand the following:

- Table
- Field
- Primary Key
- Compound Key
- Foreign Key
- One to Many Relationship
- One to One Relationship

# First Normal Form

- The Information is stored in a relational table and each column contains atomic values and there not repeating groups of columns
- Data is in a relational database table
- Each Column contains atomic values
![[Pasted image 20260903033336.png]]
# Second Normal Form

- The table is in first normal form and all the columns depend on the table's primary key
- Ask yourself what is the purpose of this table
- ![[Pasted image 20260903033436.png]] 
অবশ্যই। **Third Normal Form (3NF)**-এর এই সংজ্ঞাটা প্রথমে একটু কঠিন মনে হয়। সহজভাবে বুঝি।

> **“Table is in Second Normal Form and all of its columns are not transitively dependent on the primary key.”**

এর সহজ অর্থ:

> **Primary Key-এর ওপর সরাসরি depend করবে non-key columns। একটি non-key column অন্য একটি non-key column-এর ওপর depend করবে না।**

### একটা example দেখি

ধরুন আমাদের table:

|StudentId (PK)|StudentName|DepartmentId|DepartmentName|
|---|---|---|---|
|1|Mamun|10|Computer Science|
|2|Rahim|20|Mathematics|
|3|Karim|10|Computer Science|

এখানে:

```
StudentId → StudentName
StudentId → DepartmentId
DepartmentId → DepartmentName
```

খেয়াল করুন:

```
StudentId
    ↓
DepartmentId
    ↓
DepartmentName
```

অর্থাৎ `DepartmentName` **সরাসরি StudentId-এর ওপর depend করছে না**।

এটি `DepartmentId`-এর ওপর depend করছে।

এটাই হলো **transitive dependency**।

### সমস্যাটা কোথায়?

`DepartmentName` আসলে Student-এর property নয়।

একজন Student কোন Department-এ আছে:

```
Student → DepartmentId
```

আর Department-এর নাম:

```
DepartmentId → DepartmentName
```

তাই একই Department-এর জন্য `DepartmentName` বারবার store হচ্ছে।

### 3NF করতে কী করব?

দুটি table বানাব:

**Student**

|StudentId|StudentName|DepartmentId|
|---|---|---|
|1|Mamun|10|
|2|Rahim|20|
|3|Karim|10|

**Department**

|DepartmentId|DepartmentName|
|---|---|
|10|Computer Science|
|20|Mathematics|

এখন:

```
StudentId → StudentName
StudentId → DepartmentId

DepartmentId → DepartmentName
```

প্রতিটি table-এ non-key attribute তার নিজের table-এর **primary key-এর ওপর সরাসরি depend করছে**।

### 🧠 মনে রাখার সহজ rule

**2NF:**

> কোনো column যেন Primary Key-এর **একটা অংশের ওপর** depend না করে।

**3NF:**

> কোনো non-key column যেন **অন্য non-key column-এর ওপর** depend না করে।

এক লাইনে:

```
3NF = No transitive dependency
```

অর্থাৎ:

```
❌ Primary Key → Non-Key → Non-Key

✅ Primary Key → Non-Key
```