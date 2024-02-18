CREATE TABLE [Kurs] (
    [KursId] int NOT NULL IDENTITY,
    [Nazwa] nvarchar(60) NOT NULL,
    CONSTRAINT [PK_Kurs] PRIMARY KEY ([KursId])
);
GO


CREATE TABLE [Uczniowie] (
    [UczenId] int NOT NULL IDENTITY,
    [Imie] nvarchar(max) NULL,
    [Nazwisko] nvarchar(30) NOT NULL,
    CONSTRAINT [PK_Uczniowie] PRIMARY KEY ([UczenId])
);
GO


CREATE TABLE [KursUczen] (
    [KursyKursId] int NOT NULL,
    [UczniowieUczenId] int NOT NULL,
    CONSTRAINT [PK_KursUczen] PRIMARY KEY ([KursyKursId], [UczniowieUczenId]),
    CONSTRAINT [FK_KursUczen_Kurs_KursyKursId] FOREIGN KEY ([KursyKursId]) REFERENCES [Kurs] ([KursId]) ON DELETE CASCADE,
    CONSTRAINT [FK_KursUczen_Uczniowie_UczniowieUczenId] FOREIGN KEY ([UczniowieUczenId]) REFERENCES [Uczniowie] ([UczenId]) ON DELETE CASCADE
);
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'KursId', N'Nazwa') AND [object_id] = OBJECT_ID(N'[Kurs]'))
    SET IDENTITY_INSERT [Kurs] ON;
INSERT INTO [Kurs] ([KursId], [Nazwa])
VALUES (1, N'C# 10 i .NET 6'),
(2, N'Tworzenie stron WWW'),
(3, N'Python dla początkujących');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'KursId', N'Nazwa') AND [object_id] = OBJECT_ID(N'[Kurs]'))
    SET IDENTITY_INSERT [Kurs] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UczenId', N'Imie', N'Nazwisko') AND [object_id] = OBJECT_ID(N'[Uczniowie]'))
    SET IDENTITY_INSERT [Uczniowie] ON;
INSERT INTO [Uczniowie] ([UczenId], [Imie], [Nazwisko])
VALUES (1, N'Alicja', N'Nowak'),
(2, N'Bartek', N'Kowalski'),
(3, N'Celina', N'Poranna');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UczenId', N'Imie', N'Nazwisko') AND [object_id] = OBJECT_ID(N'[Uczniowie]'))
    SET IDENTITY_INSERT [Uczniowie] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'KursyKursId', N'UczniowieUczenId') AND [object_id] = OBJECT_ID(N'[KursUczen]'))
    SET IDENTITY_INSERT [KursUczen] ON;
INSERT INTO [KursUczen] ([KursyKursId], [UczniowieUczenId])
VALUES (1, 1),
(1, 2),
(1, 3),
(2, 2),
(3, 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'KursyKursId', N'UczniowieUczenId') AND [object_id] = OBJECT_ID(N'[KursUczen]'))
    SET IDENTITY_INSERT [KursUczen] OFF;
GO


CREATE INDEX [IX_KursUczen_UczniowieUczenId] ON [KursUczen] ([UczniowieUczenId]);
GO


