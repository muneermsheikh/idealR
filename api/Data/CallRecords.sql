SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CallRecords](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryRef] [nvarchar](max) NULL,
	[PersonType] [nvarchar](max) NULL,
	[PersonId] [nvarchar](450) NULL,
	[PersonName] [nvarchar](max) NULL,
	[Source] [nvarchar](max) NULL,
	[Subject] [nvarchar](max) NULL,
	[PhoneNo] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Status] [nvarchar](max) NULL,
	[StatusDate] [datetime2](7) NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[Username] [nvarchar](max) NULL,
	[ConcludedOn] [datetime2](7) NULL,
	[Gender] [nvarchar](max) NULL,
	[OrderItemId] [int] NULL,
	[ProfessionId] [int] NOT NULL,
	[ProfessionName] [nvarchar](max) NULL,
	[date] [datetime2](7) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CallRecords] ADD  CONSTRAINT [PK_CallRecords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CallRecords_PersonId] ON [dbo].[CallRecords]
(
	[PersonId] ASC
)
WHERE ([PersonId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CallRecords] ADD  DEFAULT ((0)) FOR [ProfessionId]
GO
