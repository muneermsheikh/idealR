-- SQLite
INSERT INTO DeployStatuses (Id, StatusName, Sequence, NextSequence, WorkingDaysReqdForNextStage)
VALUES ();

UPDATE DeployStatuses Set WorkingDaysReqdForNextStage = 5 Where Sequence=100;
UPDATE DeployStatuses Set WorkingDaysReqdForNextStage = 10 Where Sequence=300;
UPDATE DeployStatuses Set WorkingDaysReqdForNextStage = 2 Where Sequence=400;
UPDATE DeployStatuses Set WorkingDaysReqdForNextStage = 5 Where Sequence=600;
UPDATE DeployStatuses Set WorkingDaysReqdForNextStage = 4 Where Sequence=700;
UPDATE DeployStatuses Set WorkingDaysReqdForNextStage = 2 Where Sequence=900;
UPDATE DeployStatuses Set WorkingDaysReqdForNextStage = 5 Where Sequence=1100;
UPDATE DeployStatuses Set WorkingDaysReqdForNextStage = 5 Where Sequence=1300;
UPDATE DeployStatuses Set WorkingDaysReqdForNextStage = 2 Where Sequence=1500;

Update DeployStatuses Set isOptional = true where Sequence=200 Or Sequence = 500
Or Sequence = 800 Or Sequence = 1200 or Sequence  = 1400 Or Sequence = 1600 or Sequence = 5000;

