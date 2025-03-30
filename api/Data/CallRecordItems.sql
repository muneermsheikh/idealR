SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CallRecordItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CallRecordId] [int] NOT NULL,
	[IncomingOutgoing] [nvarchar](3) NOT NULL,
	[PhoneNo] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[DateOfContact] [datetime2](7) NOT NULL,
	[Username] [nvarchar](max) NULL,
	[GistOfDiscussions] [nvarchar](max) NULL,
	[ContactResult] [nvarchar](50) NOT NULL,
	[NextAction] [nvarchar](max) NULL,
	[NextActionOn] [datetime2](7) NOT NULL,
	[AdvisoryBy] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CallRecordItems] ADD  CONSTRAINT [PK_CallRecordItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CallRecordItems_CallRecordId] ON [dbo].[CallRecordItems]
(
	[CallRecordId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CallRecordItems] ADD  DEFAULT (N'') FOR [IncomingOutgoing]
GO
ALTER TABLE [dbo].[CallRecordItems] ADD  DEFAULT (N'') FOR [ContactResult]
GO
ALTER TABLE [dbo].[CallRecordItems]  WITH CHECK ADD  CONSTRAINT [FK_CallRecordItems_CallRecords_CallRecordId] FOREIGN KEY([CallRecordId])
REFERENCES [dbo].[CallRecords] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CallRecordItems] CHECK CONSTRAINT [FK_CallRecordItems_CallRecords_CallRecordId]
GO
