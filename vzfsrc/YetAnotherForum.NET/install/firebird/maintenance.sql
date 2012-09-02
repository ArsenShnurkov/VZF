/******************************************************************************/
/*                ����� �.����������� (ssdi)                                  */
/******************************************************************************/
/****    ��������� ��� ������������ �������� ��                            ****/
/******************************************************************************/
 
SET TERM ^ ;
 
-- ����������� ������������� ���� ��������
CREATE OR ALTER PROCEDURE INDICES_REBUILD_SELECTIVITY
AS
DECLARE VARIABLE S VARCHAR(200);
BEGIN
  /*��������� ��� ����������� ������������� ��������, ��������� ������������ (���� �� ���� ���������� ��),
  ���� � ������ ������� ��������� � ��.*/
  FOR SELECT RDB$INDEX_NAME FROM RDB$INDICES INTO :S DO
  BEGIN
    S = 'SET statistics INDEX ' || S || ';';
    EXECUTE STATEMENT :S;
  END
  SUSPEND;
END^
 
-- ��������/��������� ������� � ��
CREATE OR ALTER PROCEDURE INDICES_SWITCH (
    ENABLE_FLAG INTEGER)
AS
DECLARE VARIABLE RELATION_NAME VARCHAR(256);
DECLARE VARIABLE INDEX_INACTIVE INTEGER;
DECLARE VARIABLE ACTION_NAME VARCHAR(50);
DECLARE VARIABLE SQL VARCHAR(256);
BEGIN
  /* ����������� ��������� �������� */
  /* source SQL
  SELECT R.RDB$CONSTRAINT_NAME, R.RDB$INDEX_NAME AS REFINDEXNAME, I.RDB$INDEX_NAME AS REALINDEX, I.RDB$RELATION_NAME, I.RDB$INDEX_INACTIVE 
  FROM RDB$INDICES I RIGHT JOIN RDB$RELATION_CONSTRAINTS R ON I.RDB$INDEX_NAME = R.RDB$INDEX_NAME 
  WHERE R.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY' OR R.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY'
  ORDER BY R.RDB$CONSTRAINT_NAME
  */
  --��� �������� ��������� � ���������� (default)
  INDEX_INACTIVE = 0;
  ACTION_NAME = 'INACTIVE';
  IF (ENABLE_FLAG > 0) THEN
  BEGIN
    --������� � �������� ���������
    INDEX_INACTIVE = 3;
    ACTION_NAME = 'ACTIVE';
  END
 
  FOR SELECT I.RDB$INDEX_NAME
  FROM RDB$INDICES I
  RIGHT JOIN RDB$RELATION_CONSTRAINTS R ON I.RDB$INDEX_NAME = R.RDB$INDEX_NAME 
  WHERE (R.RDB$CONSTRAINT_TYPE = 'FOREIGN KEY' OR R.RDB$CONSTRAINT_TYPE = 'PRIMARY KEY')
        AND (I.RDB$INDEX_INACTIVE = :INDEX_INACTIVE)
  INTO :RELATION_NAME DO
  BEGIN
    SQL = 'ALTER INDEX ' || RELATION_NAME || ' ' || ACTION_NAME;
    IF (SQL IS NOT NULL) THEN EXECUTE STATEMENT SQL;
  END
END^
 
-- ������������� �������� ����� ����������-���������
CREATE OR ALTER PROCEDURE INDICES_REACTIVATE
AS
BEGIN
  EXECUTE PROCEDURE INDICES_SWITCH(0);
  EXECUTE PROCEDURE INDICES_SWITCH(1);
END^
 
SET TERM ; ^