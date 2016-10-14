set nocount on

print ' '
print 'BDT_CS30_Trade'
print ' '

go

if object_id('tempdb..#temptable') is not null
	drop table #temptable
go
select associated_trade,
	CASE when charindex(',',associated_trade,0) > 1 or charindex('m',associated_trade,0) = 1 THEN -1 --Cases where it has two or more TIs
	When charindex('/',associated_trade,0) > 1 THEN substring(associated_trade,0,charindex('/',associated_trade,0)) -- one TI
	ELSE -2 END as tradenum, 
	inv_num,inv_b_d_num into #temptable from inventory_build_draw
go

if object_id('tempdb..#temptable') is not null
	drop table #temptable
go