USE [IMFinance]
GO

/****** Object:  Table [dbo].[Attachments]    Script Date: 8/9/2022 9:32:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Attachments](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [QuoteId] [int] NOT NULL,
    [FileName] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
    [PhysicalPath] [nvarchar](max) NULL,
    [UploadBy] [nvarchar](128) NULL,
    [CreatedDate] [datetime] NULL,
    [LastAccessDate] [datetime] NULL,
 CONSTRAINT [PK_QuoteAttachments] PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Attachments]  WITH CHECK ADD  CONSTRAINT [FK_QuoteAttachments_Quotes1] FOREIGN KEY([QuoteId])
REFERENCES [dbo].[Quotes] ([Id])
GO

ALTER TABLE [dbo].[Attachments] CHECK CONSTRAINT [FK_QuoteAttachments_Quotes1]
GO