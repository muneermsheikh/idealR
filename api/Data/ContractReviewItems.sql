SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContractReviewItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContractReviewId] [int] NOT NULL,
	[OrderItemId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProfessionName] [nvarchar](max) NULL,
	[Quantity] [int] NOT NULL,
	[Ecnr] [bit] NOT NULL,
	[SourceFrom] [nvarchar](max) NULL,
	[RequireAssess] [nvarchar](1) NULL,
	[Charges] [int] NOT NULL,
	[HrExecUsername] [nvarchar](max) NULL,
	[ReviewItemStatus] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ContractReviewItems] ADD  CONSTRAINT [PK_ContractReviewItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ContractReviewItems_ContractReviewId] ON [dbo].[ContractReviewItems]
(
	[ContractReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ContractReviewItems_OrderItemId] ON [dbo].[ContractReviewItems]
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ContractReviewItems]  WITH CHECK ADD  CONSTRAINT [FK_ContractReviewItems_ContractReviews_ContractReviewId] FOREIGN KEY([ContractReviewId])
REFERENCES [dbo].[ContractReviews] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ContractReviewItems] CHECK CONSTRAINT [FK_ContractReviewItems_ContractReviews_ContractReviewId]
GO
ALTER TABLE [dbo].[ContractReviewItems]  WITH CHECK ADD  CONSTRAINT [FK_ContractReviewItems_OrderItems_OrderItemId] FOREIGN KEY([OrderItemId])
REFERENCES [dbo].[OrderItems] ([Id])
GO
ALTER TABLE [dbo].[ContractReviewItems] CHECK CONSTRAINT [FK_ContractReviewItems_OrderItems_OrderItemId]
GO
