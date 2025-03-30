SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AudioMessages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RecipientUsername] [nvarchar](max) NULL,
	[SenderUsername] [nvarchar](max) NULL,
	[MessageText] [nvarchar](max) NULL,
	[FileName] [nvarchar](255) NULL,
	[ContentType] [nvarchar](100) NULL,
	[CandidateName] [nvarchar](max) NULL,
	[DateComposed] [datetime2](7) NOT NULL,
	[ApplicationNo] [int] NOT NULL,
	[DatePlayedback] [datetime2](7) NOT NULL,
	[FeedbackReceived] [int] NOT NULL,
	[Subject] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AudioMessages] ADD  CONSTRAINT [PK_AudioMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AudioMessages] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [DateComposed]
GO
ALTER TABLE [dbo].[AudioMessages] ADD  DEFAULT ((0)) FOR [ApplicationNo]
GO
ALTER TABLE [dbo].[AudioMessages] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [DatePlayedback]
GO
ALTER TABLE [dbo].[AudioMessages] ADD  DEFAULT ((0)) FOR [FeedbackReceived]
GO
