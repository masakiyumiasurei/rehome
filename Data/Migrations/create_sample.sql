create table [dbo].[Tv] (
  [TvID] int identity not null
  , [¼O] nvarchar(64) not null
  , [¶Nú] DATE
  , [XÖÔ] nvarchar(7)
  , [ZP] nvarchar(128)
  , [ZQ] nvarchar(128)
  , [«Ê] smallint
  , [ítO] smallint default 0 not null
  , [o^Òhc] nvarchar(64) not null
  , [o^ú] datetime default getdate() not null
  , [XVÒhc] nvarchar(64)
  , [XVú] datetime
  , primary key (TvID)
);
