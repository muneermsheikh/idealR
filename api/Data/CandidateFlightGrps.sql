SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CandidateFlightGrps](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateOfFlight] [datetime2](7) NOT NULL,
	[OrderNo] [int] NOT NULL,
	[AirlineName] [nvarchar](max) NULL,
	[FlightNo] [nvarchar](450) NULL,
	[AirportOfBoarding] [nvarchar](max) NULL,
	[AirportOfDestination] [nvarchar](max) NULL,
	[ETD_Boarding] [datetime2](7) NOT NULL,
	[ETA_Destination] [datetime2](7) NOT NULL,
	[AirportVia] [nvarchar](max) NULL,
	[FightNoVia] [nvarchar](max) NULL,
	[ETA_Via] [datetime2](7) NULL,
	[ETD_Via] [datetime2](7) NULL,
	[FullPath] [nvarchar](250) NULL,
	[CustomerId] [int] NOT NULL,
	[ETA_DestinationString] [nvarchar](18) NULL,
	[ETD_BoardingString] [nvarchar](18) NULL,
	[ETA_ViaString] [nvarchar](18) NULL,
	[ETD_ViaString] [nvarchar](18) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CandidateFlightGrps] ADD  CONSTRAINT [PK_CandidateFlightGrps] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_CandidateFlightGrps_FlightNo] ON [dbo].[CandidateFlightGrps]
(
	[FlightNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CandidateFlightGrps] ADD  DEFAULT ((0)) FOR [CustomerId]
GO
