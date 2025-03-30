SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CandidateFlightItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateFlightGrpId] [int] NOT NULL,
	[DepId] [int] NOT NULL,
	[CvRefId] [int] NOT NULL,
	[DepItemId] [int] NOT NULL,
	[ApplicationNo] [int] NOT NULL,
	[CandidateName] [nvarchar](max) NULL,
	[CustomerName] [nvarchar](max) NULL,
	[CustomerCity] [nvarchar](max) NULL,
	[CategoryName] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CandidateFlightItems] ADD  CONSTRAINT [PK_CandidateFlightItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CandidateFlightItems_CandidateFlightGrpId] ON [dbo].[CandidateFlightItems]
(
	[CandidateFlightGrpId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CandidateFlightItems_CvRefId] ON [dbo].[CandidateFlightItems]
(
	[CvRefId] ASC
)
WHERE ([CvRefId]<>(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CandidateFlightItems]  WITH CHECK ADD  CONSTRAINT [FK_CandidateFlightItems_CandidateFlightGrps_CandidateFlightGrpId] FOREIGN KEY([CandidateFlightGrpId])
REFERENCES [dbo].[CandidateFlightGrps] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CandidateFlightItems] CHECK CONSTRAINT [FK_CandidateFlightItems_CandidateFlightGrps_CandidateFlightGrpId]
GO
