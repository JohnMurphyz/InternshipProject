CREATE TABLE [dbo].[CMTable]
(
	[PartnerID] INT NOT NULL PRIMARY KEY, 
    [ParentID] INT NOT NULL, 
    [ERPID] INT NOT NULL, 
    [AccountName] NVARCHAR(50) NOT NULL, 
    [Address] NVARCHAR(200) NOT NULL, 
    [Country] NVARCHAR(50) NOT NULL, 
    [PartnerContactName] NVARCHAR(50) NOT NULL, 
    [PartnerContactEmail] NVARCHAR(100) NOT NULL, 
    [CAMName] NVARCHAR(50) NOT NULL, 
    [CAMEmail] NVARCHAR(50) NOT NULL, 
    [OnBoardDate] DATETIME NOT NULL, 
    [Status] NVARCHAR(50) NOT NULL, 
    [Type] NVARCHAR(50) NOT NULL, 
    [CurrentLevel] NVARCHAR(50) NOT NULL, 
    [LastLevelChangeDate] DATETIME NOT NULL, 
    [NextComplianceReviewDate] DATETIME NOT NULL
)
