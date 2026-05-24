-- seed.sql
-- Sample data for Library Management System

-- Books (10 books)
INSERT INTO Books (Title, Author, ISBN, PublishedYear, TotalCopies) VALUES
('Clean Code', 'Robert C. Martin', '978-0132350884', 2008, 3),
('The Pragmatic Programmer', 'David Thomas', '978-0135957059', 2019, 2),
('Design Patterns', 'Gang of Four', '978-0201633610', 1994, 2),
('The Clean Coder', 'Robert C. Martin', '978-0137081073', 2011, 3),
('Refactoring', 'Martin Fowler', '978-0134757599', 2018, 2),
('Domain-Driven Design', 'Eric Evans', '978-0321125217', 2003, 2),
('Working Effectively with Legacy Code', 'Michael Feathers', '978-0131177055', 2004, 1),
('The Mythical Man-Month', 'Frederick Brooks', '978-0201835953', 1995, 2),
('Code Complete', 'Steve McConnell', '978-0735619678', 2004, 3),
('Introduction to Algorithms', 'Thomas Cormen', '978-0262033848', 2009, 2);

-- Members (5 members)
INSERT INTO Members (SsoSubject, FullName, Email, JoinedDate) VALUES
('sub-001', 'Alice Johnson', 'alice@library.com', '2024-01-15'),
('sub-002', 'Bob Smith', 'bob@library.com', '2024-02-20'),
('sub-003', 'Carol White', 'carol@library.com', '2024-03-10'),
('sub-004', 'David Brown', 'david@library.com', '2024-04-05'),
('sub-005', 'Eve Davis', 'eve@library.com', '2024-05-01');

-- Loans (10 loans - mix of returned and active)
INSERT INTO Loans (BookId, MemberId, BorrowedDate, ReturnedDate) VALUES
(1, 1, '2024-06-01', '2024-06-15'),  -- returned
(2, 1, '2024-07-01', '2024-07-20'),  -- returned
(3, 2, '2024-07-15', '2024-08-01'),  -- returned
(4, 2, '2024-08-10', NULL),          -- active
(5, 3, '2024-08-01', '2024-08-20'),  -- returned
(6, 3, '2024-09-01', NULL),          -- active
(7, 4, '2024-09-10', '2024-10-01'),  -- returned
(1, 4, '2024-10-01', NULL),          -- active (overdue)
(2, 5, '2024-10-15', '2024-11-01'),  -- returned
(3, 5, '2026-05-01', NULL);          -- active