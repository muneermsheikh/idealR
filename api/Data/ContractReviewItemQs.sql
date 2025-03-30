SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContractReviewItemQs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderItemId] [int] NOT NULL,
	[ContractReviewItemId] [int] NOT NULL,
	[SrNo] [int] NOT NULL,
	[ReviewParameter] [nvarchar](max) NULL,
	[Response] [bit] NOT NULL,
	[ResponseText] [nvarchar](max) NULL,
	[IsResponseBoolean] [bit] NOT NULL,
	[IsMandatoryTrue] [bit] NOT NULL,
	[Remarks] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ContractReviewItemQs] ADD  CONSTRAINT [PK_ContractReviewItemQs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ContractReviewItemQs_ContractReviewItemId] ON [dbo].[ContractReviewItemQs]
(
	[ContractReviewItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ContractReviewItemQs]  WITH CHECK ADD  CONSTRAINT [FK_ContractReviewItemQs_ContractReviewItems_ContractReviewItemId] FOREIGN KEY([ContractReviewItemId])
REFERENCES [dbo].[ContractReviewItems] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ContractReviewItemQs] CHECK CONSTRAINT [FK_ContractReviewItemQs_ContractReviewItems_ContractReviewItemId]
GO
