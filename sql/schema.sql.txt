-- schema.sql
-- Library Management System - Table Definitions

-- Books table
-- Index on Title and Author for fast filtering/searching
CREATE TABLE Books (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Title       NVARCHAR(200) NOT NULL,
    Author      NVARCHAR(200) NOT NULL,
    ISBN        NVARCHAR(20)  NOT NULL,
    PublishedYear INT          NOT NULL,
    TotalCopies INT           NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Books_ISBN UNIQUE (ISBN)
);

-- Index for case-insensitive partial match on Title and Author
CREATE INDEX IX_Books_Title  ON Books (Title);
CREATE INDEX IX_Books_Author ON Books (Author);

-- Members table
-- SsoSubject is the stable identifier from the OIDC provider (sub claim)
CREATE TABLE Members (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    SsoSubject  NVARCHAR(200) NOT NULL,
    FullName    NVARCHAR(200) NOT NULL,
    Email       NVARCHAR(200) NOT NULL,
    JoinedDate  DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UQ_Members_SsoSubject UNIQUE (SsoSubject)
);

-- Index on SsoSubject for fast SSO lookup on every request
CREATE INDEX IX_Members_SsoSubject ON Members (SsoSubject);

-- Loans table
-- ReturnedDate NULL means the loan is still active
CREATE TABLE Loans (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    BookId       INT       NOT NULL,
    MemberId     INT       NOT NULL,
    BorrowedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ReturnedDate DATETIME2 NULL,
    CONSTRAINT FK_Loans_Books   FOREIGN KEY (BookId)   REFERENCES Books(Id),
    CONSTRAINT FK_Loans_Members FOREIGN KEY (MemberId) REFERENCES Members(Id)
);

-- Index on BookId to quickly count active loans per book (availability check)
CREATE INDEX IX_Loans_BookId   ON Loans (BookId);
-- Index on MemberId to quickly count active loans per member (loan limit check)
CREATE INDEX IX_Loans_MemberId ON Loans (MemberId);