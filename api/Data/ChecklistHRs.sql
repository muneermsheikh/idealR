SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChecklistHRs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CandidateId] [int] NOT NULL,
	[OrderItemId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[CheckedOn] [datetime2](7) NOT NULL,
	[HrExecUsername] [nvarchar](max) NULL,
	[HrExecComments] [nvarchar](max) NULL,
	[Charges] [int] NOT NULL,
	[ChargesAgreed] [int] NOT NULL,
	[ExceptionApproved] [bit] NOT NULL,
	[ExceptionApprovedBy] [nvarchar](max) NULL,
	[ExceptionApprovedOn] [datetime2](7) NOT NULL,
	[ChecklistedOk] [bit] NOT NULL,
	[Username] [nvarchar](max) NULL,
	[SalaryExpectation] [int] NOT NULL,
	[SalaryOffered] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ChecklistHRs] ADD  CONSTRAINT [PK_ChecklistHRs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ChecklistHRs_OrderItemId_CandidateId] ON [dbo].[ChecklistHRs]
(
	[OrderItemId] ASC,
	[CandidateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ChecklistHRs] ADD  DEFAULT ((0)) FOR [SalaryExpectation]
GO
