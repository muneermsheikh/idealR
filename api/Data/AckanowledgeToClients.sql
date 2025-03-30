SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AckanowledgeToClients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[DateAcknowledged] [datetime2](7) NOT NULL,
	[RecipientUsername] [nvarchar](max) NULL,
	[RecipientEmailId] [nvarchar](max) NULL,
	[SenderUsername] [nvarchar](max) NULL,
	[SenderEmailId] [nvarchar](max) NULL,
	[CustomerId] [int] NOT NULL,
	[CustomerName] [nvarchar](max) NULL,
	[MessageType] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AckanowledgeToClients] ADD  CONSTRAINT [PK_AckanowledgeToClients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AckanowledgeToClients_OrderId] ON [dbo].[AckanowledgeToClients]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
