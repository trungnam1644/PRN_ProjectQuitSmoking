-- =============================================
-- Database Schema for Smoking Cessation Support System
-- =============================================

USE master;
GO

-- Tạo database nếu chưa tồn tại
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SmokingCessationDB')
BEGIN
    CREATE DATABASE SmokingCessationDB;
END
GO

USE SmokingCessationDB;
GO

-- =============================================
-- Tạo bảng Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(255) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL UNIQUE,
        [FullName] NVARCHAR(100) NOT NULL,
        [DateOfBirth] DATETIME NOT NULL,
        [Role] NVARCHAR(20) NOT NULL DEFAULT 'User'
    );
END
GO

-- =============================================
-- Tạo bảng Coach (mở rộng từ Users)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Coach]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Coach] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Specialization] NVARCHAR(200) NOT NULL,
        [ExperienceYears] INT NOT NULL,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
END
GO

-- =============================================
-- Tạo bảng ChatMessage
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChatMessage]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ChatMessage] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [SenderId] INT NOT NULL,
        [ReceiverId] INT NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [SentAt] DATETIME NOT NULL DEFAULT GETDATE(),
        [IsRead] BIT NOT NULL DEFAULT 0,
        FOREIGN KEY ([SenderId]) REFERENCES [Users]([Id]),
        FOREIGN KEY ([ReceiverId]) REFERENCES [Users]([Id])
    );
END
GO

-- =============================================
-- Tạo bảng QuitPlan
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QuitPlan]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[QuitPlan] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Reason] NVARCHAR(500) NOT NULL,
        [StartDate] DATETIME NOT NULL,
        [TargetDate] DATETIME NOT NULL,
        [Stages] NVARCHAR(MAX) NOT NULL,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
END
GO

-- =============================================
-- Tạo bảng SmokingStatus
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SmokingStatus]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SmokingStatus] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [CigarettesPerDay] INT NOT NULL,
        [CostPerPack] DECIMAL(10,2) NOT NULL,
        [PacksPerWeek] INT NOT NULL,
        [RecordDate] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
END
GO

-- =============================================
-- Tạo bảng CommunityPost
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommunityPost]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CommunityPost] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Content] NVARCHAR(MAX) NOT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
END
GO

-- =============================================
-- Tạo bảng Comment
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Comment]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Comment] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [PostId] INT NOT NULL,
        [UserId] INT NOT NULL,
        [Content] NVARCHAR(500) NOT NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([PostId]) REFERENCES [CommunityPost]([Id]) ON DELETE CASCADE,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
END
GO

-- =============================================
-- Tạo bảng Notification
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Notification]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Notification] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Message] NVARCHAR(500) NOT NULL,
        [SentDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [IsRead] BIT NOT NULL DEFAULT 0,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
END
GO

-- =============================================
-- Tạo bảng Feedback
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Feedback]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Feedback] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Content] NVARCHAR(500) NOT NULL,
        [Rating] INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
END
GO

-- =============================================
-- Tạo bảng Membership
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Membership]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Membership] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [PackageName] NVARCHAR(100) NOT NULL,
        [Price] DECIMAL(10,2) NOT NULL,
        [StartDate] DATETIME NOT NULL,
        [EndDate] DATETIME NOT NULL,
        [UserId] INT NOT NULL,
        FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
    );
END
GO

-- =============================================
-- Tạo Indexes để tối ưu hiệu suất
-- =============================================

-- Index cho Users
CREATE INDEX IX_Users_Username ON [Users]([Username]);
CREATE INDEX IX_Users_Email ON [Users]([Email]);

-- Index cho ChatMessage
CREATE INDEX IX_ChatMessage_SenderId ON [ChatMessage]([SenderId]);
CREATE INDEX IX_ChatMessage_ReceiverId ON [ChatMessage]([ReceiverId]);
CREATE INDEX IX_ChatMessage_SentAt ON [ChatMessage]([SentAt]);

-- Index cho QuitPlan
CREATE INDEX IX_QuitPlan_UserId ON [QuitPlan]([UserId]);

-- Index cho SmokingStatus
CREATE INDEX IX_SmokingStatus_UserId ON [SmokingStatus]([UserId]);
CREATE INDEX IX_SmokingStatus_RecordDate ON [SmokingStatus]([RecordDate]);

-- Index cho CommunityPost
CREATE INDEX IX_CommunityPost_UserId ON [CommunityPost]([UserId]);
CREATE INDEX IX_CommunityPost_CreatedAt ON [CommunityPost]([CreatedAt]);

-- Index cho Comment
CREATE INDEX IX_Comment_PostId ON [Comment]([PostId]);
CREATE INDEX IX_Comment_UserId ON [Comment]([UserId]);

-- Index cho Notification
CREATE INDEX IX_Notification_UserId ON [Notification]([UserId]);
CREATE INDEX IX_Notification_IsRead ON [Notification]([IsRead]);

-- Index cho Feedback
CREATE INDEX IX_Feedback_UserId ON [Feedback]([UserId]);

-- Index cho Membership
CREATE INDEX IX_Membership_UserId ON [Membership]([UserId]);

-- Index cho Coach
CREATE INDEX IX_Coach_UserId ON [Coach]([UserId]);

GO

-- =============================================
-- Thêm dữ liệu mẫu cho Users
-- =============================================
IF NOT EXISTS (SELECT * FROM [Users] WHERE [Username] = 'user1')
BEGIN
    INSERT INTO [Users] ([Username], [PasswordHash], [Email], [FullName], [DateOfBirth], [Role])
    VALUES 
    ('user1', '123', 'user1@example.com', 'Nguyen Van A', '1990-05-15', 'User'),
    ('user2', '123', 'user2@example.com', 'Tran Thi B', '1988-12-20', 'User'),
    ('user3', '123', 'user3@example.com', 'Le Van C', '1995-08-10', 'User'),
    ('coach1', '123', 'coach1@example.com', 'John Doe', '1985-01-15', 'Coach'),
    ('coach2', '123', 'coach2@example.com', 'Jane Smith', '1988-03-20', 'Coach'),
    ('coach3', '123', 'coach3@example.com', 'Alice Johnson', '1982-07-10', 'Coach'),
	('admin', '123', 'admin@example.com', 'Huy', '2004-06-14', 'Admin');
END
GO

-- =============================================
-- Thêm dữ liệu mẫu cho Coach
-- =============================================
IF NOT EXISTS (SELECT * FROM [Coach] WHERE [Name] = 'John Doe')
BEGIN
    INSERT INTO [Coach] ([UserId], [Name], [Specialization], [ExperienceYears])
    VALUES 
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'coach1'), 'John Doe', 'Behavioral Therapy', 5),
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'coach2'), 'Jane Smith', 'Cognitive Behavioral Therapy', 8),
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'coach3'), 'Alice Johnson', 'Motivational Interviewing', 6);
END
GO

-- =============================================
-- Thêm dữ liệu mẫu cho CommunityPost
-- =============================================
IF NOT EXISTS (SELECT * FROM [CommunityPost] WHERE [UserId] = (SELECT [Id] FROM [Users] WHERE [Username] = 'user1'))
BEGIN
    INSERT INTO [CommunityPost] ([UserId], [Content], [CreatedAt])
    VALUES 
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'user1'), N'Hôm nay tôi đã giảm được 5 điếu thuốc! Cảm thấy rất tốt về bản thân.', GETDATE()),
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'user2'), N'Ai có kinh nghiệm về việc sử dụng nicotine patch không?', DATEADD(DAY, -1, GETDATE())),
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'user3'), N'Chia sẻ: Tôi đã bỏ thuốc được 1 tháng rồi!', DATEADD(DAY, -2, GETDATE()));
END
GO

-- =============================================
-- Thêm dữ liệu mẫu cho Comment
-- =============================================
IF NOT EXISTS (SELECT * FROM [Comment] WHERE [UserId] = (SELECT [Id] FROM [Users] WHERE [Username] = 'user2'))
BEGIN
    INSERT INTO [Comment] ([PostId], [UserId], [Content], [CreatedAt])
    VALUES 
    ((SELECT TOP 1 [Id] FROM [CommunityPost] WHERE [UserId] = (SELECT [Id] FROM [Users] WHERE [Username] = 'user1')), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'user2'), N'Chúc mừng bạn! Tiếp tục phát huy nhé!', GETDATE()),
    ((SELECT TOP 1 [Id] FROM [CommunityPost] WHERE [UserId] = (SELECT [Id] FROM [Users] WHERE [Username] = 'user2')), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'user3'), N'Tôi đã dùng nicotine patch, hiệu quả lắm!', DATEADD(HOUR, -2, GETDATE())),
    ((SELECT TOP 1 [Id] FROM [CommunityPost] WHERE [UserId] = (SELECT [Id] FROM [Users] WHERE [Username] = 'user3')), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'user1'), N'Thật tuyệt vời! Bạn có bí quyết gì không?', DATEADD(HOUR, -1, GETDATE()));
END
GO

-- =============================================
-- Thêm dữ liệu mẫu cho Feedback
-- =============================================
IF NOT EXISTS (SELECT * FROM [Feedback] WHERE [UserId] = (SELECT [Id] FROM [Users] WHERE [Username] = 'user1'))
BEGIN
    INSERT INTO [Feedback] ([UserId], [Content], [Rating], [CreatedAt])
    VALUES 
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'user1'), N'Đánh giá trải nghiệm: Ứng dụng rất hữu ích, giúp tôi theo dõi tiến độ tốt', 5, GETDATE()),
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'user2'), N'Đánh giá trải nghiệm: Giao diện dễ sử dụng, nhưng cần thêm tính năng', 4, DATEADD(DAY, -1, GETDATE())),
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'user3'), N'Đánh giá trải nghiệm: Tuyệt vời! Đã giúp tôi bỏ thuốc thành công', 5, DATEADD(DAY, -2, GETDATE()));
END
GO

-- =============================================
-- Thêm dữ liệu mẫu cho Membership
-- =============================================
IF NOT EXISTS (SELECT * FROM [Membership] WHERE [UserId] = (SELECT [Id] FROM [Users] WHERE [Username] = 'user1'))
BEGIN
    INSERT INTO [Membership] ([PackageName], [Price], [StartDate], [EndDate], [UserId])
    VALUES 
    ('Premium Monthly', 199000, GETDATE(), DATEADD(MONTH, 1, GETDATE()), (SELECT [Id] FROM [Users] WHERE [Username] = 'user1')),
    ('Basic Monthly', 99000, GETDATE(), DATEADD(MONTH, 1, GETDATE()), (SELECT [Id] FROM [Users] WHERE [Username] = 'user2')),
    ('Premium Yearly', 1990000, GETDATE(), DATEADD(YEAR, 1, GETDATE()), (SELECT [Id] FROM [Users] WHERE [Username] = 'user3'));
END
GO

-- =============================================
-- Thêm dữ liệu mẫu cho ChatMessage
-- =============================================
IF NOT EXISTS (SELECT * FROM [ChatMessage] WHERE [SenderId] = (SELECT [Id] FROM [Users] WHERE [Username] = 'coach1'))
BEGIN
    INSERT INTO [ChatMessage] ([SenderId], [ReceiverId], [Message], [SentAt], [IsRead])
    VALUES 
    -- Chat giữa coach1 và user1
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'coach1'), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'user1'), 
     N'Xin chào! Tôi có thể giúp gì cho bạn hôm nay?', 
     DATEADD(MINUTE, -30, GETDATE()), 1),
    
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'user1'), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'coach1'), 
     N'Tôi cần tư vấn về chế độ tập luyện để bỏ thuốc', 
     DATEADD(MINUTE, -25, GETDATE()), 1),
    
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'coach1'), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'user1'), 
     N'Tuyệt vời! Tôi sẽ hướng dẫn bạn các bài tập thở và thiền', 
     DATEADD(MINUTE, -20, GETDATE()), 0),
    
    -- Chat giữa coach2 và user2
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'coach2'), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'user2'), 
     N'Chào bạn! Tôi thấy bạn đã giảm được nhiều thuốc lá', 
     DATEADD(HOUR, -2, GETDATE()), 1),
    
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'user2'), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'coach2'), 
     N'Vâng, tôi đang cố gắng. Có cách nào để vượt qua cơn thèm không?', 
     DATEADD(HOUR, -1, GETDATE()), 1),
    
    -- Chat giữa coach3 và user3
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'coach3'), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'user3'), 
     N'Chúc mừng bạn đã bỏ thuốc được 1 tháng!', 
     DATEADD(DAY, -1, GETDATE()), 1),
    
    ((SELECT [Id] FROM [Users] WHERE [Username] = 'user3'), 
     (SELECT [Id] FROM [Users] WHERE [Username] = 'coach3'), 
     N'Cảm ơn coach! Tôi cảm thấy sức khỏe tốt hơn nhiều', 
     DATEADD(HOUR, -12, GETDATE()), 1);
END
GO

PRINT 'Database schema and sample data created successfully!'; 


ALTER TABLE QuitPlan
ADD MainGoal NVARCHAR(255) NOT NULL DEFAULT N'',
    TargetCigarettesPerDay INT NOT NULL DEFAULT 0;

-- Nếu bạn không cần trường Stages nữa:
ALTER TABLE QuitPlan
DROP COLUMN Stages;

-- Thêm cột HealthStatus
ALTER TABLE SmokingStatus
ADD HealthStatus NVARCHAR(255) NULL;

-- Xóa 2 cột không còn dùng nữa
ALTER TABLE SmokingStatus
DROP COLUMN CostPerPack, PacksPerWeek;