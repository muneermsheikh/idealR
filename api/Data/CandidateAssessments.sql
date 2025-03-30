SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CandidateAssessments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderItemId] [int] NOT NULL,
	[CandidateId] [int] NOT NULL,
	[CustomerName] [nvarchar](max) NULL,
	[CategoryRefAndName] [nvarchar](max) NULL,
	[AssessedOn] [datetime2](7) NOT NULL,
	[AssessedByEmployeeId] [int] NOT NULL,
	[AssessedByEmployeeName] [nvarchar](max) NULL,
	[RequireInternalReview] [bit] NOT NULL,
	[ChecklistHRId] [int] NOT NULL,
	[AssessResult] [nvarchar](max) NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[CVRefId] [int] NOT NULL,
	[TaskIdDocControllerAdmin] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CandidateAssessments] ADD  CONSTRAINT [PK_CandidateAssessments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CandidateAssessments_CandidateId_OrderItemId] ON [dbo].[CandidateAssessments]
(
	[CandidateId] ASC,
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CandidateAssessments_OrderItemId] ON [dbo].[CandidateAssessments]
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CandidateAssessments]  WITH CHECK ADD  CONSTRAINT [FK_CandidateAssessments_OrderItems_OrderItemId] FOREIGN KEY([OrderItemId])
REFERENCES [dbo].[OrderItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CandidateAssessments] CHECK CONSTRAINT [FK_CandidateAssessments_OrderItems_OrderItemId]
GO
