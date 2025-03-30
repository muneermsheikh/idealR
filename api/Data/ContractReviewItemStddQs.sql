SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContractReviewItemStddQs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SrNo] [int] NOT NULL,
	[ReviewParameter] [nvarchar](max) NULL,
	[ResponseText] [nvarchar](max) NULL,
	[ButtonText] [nvarchar](max) NULL,
	[Button2Text] [nvarchar](max) NULL,
	[TextInput] [bit] NOT NULL,
	[IsMandatoryTrue] [bit] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ContractReviewItemStddQs] ADD  CONSTRAINT [PK_ContractReviewItemStddQs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
