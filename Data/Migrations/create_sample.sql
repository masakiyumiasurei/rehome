create table [dbo].[�T���v��] (
  [�T���v��ID] int identity not null
  , [���O] nvarchar(64) not null
  , [���N����] DATE
  , [�X�֔ԍ�] nvarchar(7)
  , [�Z���P] nvarchar(128)
  , [�Z���Q] nvarchar(128)
  , [����] smallint
  , [�폜�t���O] smallint default 0 not null
  , [�o�^�҂h�c] nvarchar(64) not null
  , [�o�^����] datetime default getdate() not null
  , [�X�V�҂h�c] nvarchar(64)
  , [�X�V����] datetime
  , primary key (�T���v��ID)
);
