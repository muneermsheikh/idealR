SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssessmentBankQs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssessmentBankId] [int] NOT NULL,
	[AssessmentParameter] [nvarchar](max) NULL,
	[QNo] [int] NOT NULL,
	[IsStandardQ] [bit] NOT NULL,
	[IsMandatory] [bit] NOT NULL,
	[Question] [nvarchar](max) NULL,
	[MaxPoints] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AssessmentBankQs] ADD  CONSTRAINT [PK_AssessmentBankQs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AssessmentBankQs_AssessmentBankId] ON [dbo].[AssessmentBankQs]
(
	[AssessmentBankId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AssessmentBankQs]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentBankQs_AssessmentBanks_AssessmentBankId] FOREIGN KEY([AssessmentBankId])
REFERENCES [dbo].[AssessmentBanks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AssessmentBankQs] CHECK CONSTRAINT [FK_AssessmentBankQs_AssessmentBanks_AssessmentBankId]
GO
