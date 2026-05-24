-- queries.sql
-- Library Management System - Required Queries

-- Q1: Top 5 most-borrowed books of all time
SELECT TOP 5
    b.Title,
    b.Author,
    COUNT(l.Id) AS TotalTimesBorrowed
FROM Books b
INNER JOIN Loans l ON b.Id = l.BookId
GROUP BY b.Id, b.Title, b.Author
ORDER BY TotalTimesBorrowed DESC;

-- Q2: Members with at least one overdue loan (active loan older than 14 days)
SELECT
    m.FullName,
    m.Email,
    COUNT(l.Id) AS OverdueLoans
FROM Members m
INNER JOIN Loans l ON m.Id = l.MemberId
WHERE l.ReturnedDate IS NULL
  AND l.BorrowedDate < DATEADD(DAY, -14, GETUTCDATE())
GROUP BY m.Id, m.FullName, m.Email
HAVING COUNT(l.Id) >= 1;

-- Q3: Total loans per month for the last 12 months (including months with zero loans)
WITH Months AS (
    SELECT DATEFROMPARTS(YEAR(DATEADD(MONTH, -n, GETUTCDATE())),
                         MONTH(DATEADD(MONTH, -n, GETUTCDATE())), 1) AS MonthStart
    FROM (VALUES(0),(1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11)) AS t(n)
)
SELECT
    FORMAT(m.MonthStart, 'yyyy-MM') AS Month,
    COUNT(l.Id) AS TotalLoans
FROM Months m
LEFT JOIN Loans l ON DATEFROMPARTS(YEAR(l.BorrowedDate), MONTH(l.BorrowedDate), 1) = m.MonthStart
GROUP BY m.MonthStart
ORDER BY m.MonthStart;

-- Q4: Books that have never been borrowed
SELECT
    b.Id,
    b.Title,
    b.Author,
    b.ISBN
FROM Books b
LEFT JOIN Loans l ON b.Id = l.BookId
WHERE l.Id IS NULL;

-- Q5: Member with the longest single loan duration (returned loans only)
SELECT TOP 1
    m.FullName,
    b.Title,
    DATEDIFF(DAY, l.BorrowedDate, l.ReturnedDate) AS DaysBorrowed
FROM Loans l
INNER JOIN Members m ON l.MemberId = m.Id
INNER JOIN Books b ON l.BookId = b.Id
WHERE l.ReturnedDate IS NOT NULL
ORDER BY DaysBorrowed DESC;