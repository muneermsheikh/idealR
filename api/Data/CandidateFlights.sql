SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CandidateFlights](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DepId] [int] NOT NULL,
	[CvRefId] [int] NOT NULL,
	[DepItemId] [int] NOT NULL,
	[ApplicationNo] [int] NOT NULL,
	[CandidateName] [nvarchar](max) NULL,
	[CustomerName] [nvarchar](max) NULL,
	[CustomerCity] [nvarchar](max) NULL,
	[DateOfFlight] [datetime2](7) NOT NULL,
	[FlightNo] [nvarchar](max) NULL,
	[AirportOfBoarding] [nvarchar](max) NULL,
	[AirportOfDestination] [nvarchar](max) NULL,
	[ETD_Boarding] [datetime2](7) NOT NULL,
	[ETA_Destination] [datetime2](7) NOT NULL,
	[AirportVia] [nvarchar](max) NULL,
	[FightNoVia] [nvarchar](max) NULL,
	[ETA_Via] [datetime2](7) NULL,
	[ETD_Via] [datetime2](7) NULL,
	[FullPath] [nvarchar](250) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[CandidateFlights] ADD  CONSTRAINT [PK_CandidateFlights] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
