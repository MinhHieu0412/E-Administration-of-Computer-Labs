USE [ADMIN_MMN]
GO

/****** Object: Table [dbo].[RepairAssignments] Script Date: 26/12/2024 8:08:15 CH ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RepairAssignments] (
    [ID]            INT           IDENTITY (1, 1) NOT NULL,
    [IssueReportID] INT           NOT NULL,
    [TechnicianID]  INT           NOT NULL,
    [CreatedAt]     DATETIME2 (7) NULL,
    [UpdatedAt]     DATETIME2 (7) NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_RepairAssignments_IssueReportID]
    ON [dbo].[RepairAssignments]([IssueReportID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RepairAssignments_TechnicianID]
    ON [dbo].[RepairAssignments]([TechnicianID] ASC);


GO
ALTER TABLE [dbo].[RepairAssignments]
    ADD CONSTRAINT [PK_RepairAssignments] PRIMARY KEY CLUSTERED ([ID] ASC);


GO
ALTER TABLE [dbo].[RepairAssignments]
    ADD CONSTRAINT [FK_RepairAssignments_IssueReports_IssueReportID] FOREIGN KEY ([IssueReportID]) REFERENCES [dbo].[IssueReports] ([ID]);


	SELECT * FROM Users;
