-- SQLite
SELECT Id, Divn, COAId, AccountName, VoucherNo, VoucherDated, Amount, Narration, EmployeeId
FROM FinanceVouchers;

DELETE FROM FinanceVouchers where Id>20;

delete from Processes;

UPDATE CVRefs SET SelectionStatus =  "";

UPDATE CVRefs SET SelectionStatus = "Selected" where Id=135 or Id=136 or Id=137;