SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CandidatesAssessmentItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateAssessmentId] [int] NOT NULL,
	[QuestionNo] [int] NOT NULL,
	[AssessmentGroup] [nvarchar](max) NULL,
	[Question] [nvarchar](max) NULL,
	[IsMandatory] [bit] NOT NULL,
	[AssessedOnTheParameter] [bit] NOT NULL,
	[MaxPoints] [int] NOT NULL,
	[Points] [int] NOT NULL,
	[Remarks] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CandidatesAssessmentItems] ADD  CONSTRAINT [PK_CandidatesAssessmentItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CandidatesAssessmentItems_CandidateAssessmentId] ON [dbo].[CandidatesAssessmentItems]
(
	[CandidateAssessmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CandidatesAssessmentItems]  WITH CHECK ADD  CONSTRAINT [FK_CandidatesAssessmentItems_CandidateAssessments_CandidateAssessmentId] FOREIGN KEY([CandidateAssessmentId])
REFERENCES [dbo].[CandidateAssessments] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CandidatesAssessmentItems] CHECK CONSTRAINT [FK_CandidatesAssessmentItems_CandidateAssessments_CandidateAssessmentId]
GO
