SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Candidates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationNo] [int] NOT NULL,
	[Gender] [nvarchar](1) NOT NULL,
	[FirstName] [nvarchar](max) NULL,
	[SecondName] [nvarchar](max) NULL,
	[FamilyName] [nvarchar](max) NULL,
	[KnownAs] [nvarchar](max) NULL,
	[CustomerId] [int] NOT NULL,
	[Source] [nvarchar](max) NULL,
	[DOB] [datetime2](7) NULL,
	[PpNo] [nvarchar](max) NULL,
	[AadharNo] [nvarchar](max) NULL,
	[Ecnr] [nvarchar](1) NULL,
	[Address] [nvarchar](max) NULL,
	[City] [nvarchar](max) NULL,
	[Pin] [nvarchar](max) NULL,
	[Country] [nvarchar](max) NULL,
	[Nationality] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Created] [datetime2](7) NOT NULL,
	[LastActive] [datetime2](7) NOT NULL,
	[AppUserId] [int] NOT NULL,
	[Status] [nvarchar](max) NULL,
	[Qualifications] [nvarchar](max) NULL,
	[NotificationDesired] [bit] NOT NULL,
	[Username] [nvarchar](max) NULL,
	[CVRefId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Candidates] ADD  CONSTRAINT [PK_Candidates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Candidates_CVRefId] ON [dbo].[Candidates]
(
	[CVRefId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Candidates] ADD  DEFAULT ((0)) FOR [CustomerId]
GO
ALTER TABLE [dbo].[Candidates]  WITH CHECK ADD  CONSTRAINT [FK_Candidates_CVRefs_CVRefId] FOREIGN KEY([CVRefId])
REFERENCES [dbo].[CVRefs] ([Id])
GO
ALTER TABLE [dbo].[Candidates] CHECK CONSTRAINT [FK_Candidates_CVRefs_CVRefId]
GO
