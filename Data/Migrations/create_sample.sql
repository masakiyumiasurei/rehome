create table [dbo].[ƒTƒ“ƒvƒ‹] (
  [ƒTƒ“ƒvƒ‹ID] int identity not null
  , [–¼‘O] nvarchar(64) not null
  , [¶”NŒ“ú] DATE
  , [—X•Ö”Ô†] nvarchar(7)
  , [ZŠ‚P] nvarchar(128)
  , [ZŠ‚Q] nvarchar(128)
  , [«•Ê] smallint
  , [íœƒtƒ‰ƒO] smallint default 0 not null
  , [“o˜^Ò‚h‚c] nvarchar(64) not null
  , [“o˜^“ú] datetime default getdate() not null
  , [XVÒ‚h‚c] nvarchar(64)
  , [XV“ú] datetime
  , primary key (ƒTƒ“ƒvƒ‹ID)
);
