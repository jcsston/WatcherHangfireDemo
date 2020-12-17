IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'hangfire_schedule')
BEGIN
	CREATE TABLE hangfire_schedule
	(
		job_name varchar(255) not null,
		cron_expression varchar(255) not null,
		inactive char(1) not null default 'N'
	);

	INSERT INTO hangfire_schedule (job_name, cron_expression, inactive) VALUES ('AutoEmailService', '*/5 * * * * *', 'N');
	INSERT INTO hangfire_schedule (job_name, cron_expression, inactive) VALUES ('AutoTextService',  '* * * * * *', 'N');
	INSERT INTO hangfire_schedule (job_name, cron_expression, inactive) VALUES ('LargeExport',      '*/15 * * * * *', 'N');
	INSERT INTO hangfire_schedule (job_name, cron_expression, inactive) VALUES ('LargeExport2',      '*/15 * * * * *', 'N');
	INSERT INTO hangfire_schedule (job_name, cron_expression, inactive) VALUES ('LargeExport3',      '*/15 * * * * *', 'N');
	INSERT INTO hangfire_schedule (job_name, cron_expression, inactive) VALUES ('LargeExport4',      '*/15 * * * * *', 'N');
END

-- cron expression documentation https://github.com/HangfireIO/Cronos
UPDATE hangfire_schedule SET cron_expression = '*/5 * * * *' WHERE job_name = 'AutoEmailService';
UPDATE hangfire_schedule SET cron_expression = '* * * * * *' WHERE job_name = 'AutoTextService';
UPDATE hangfire_schedule SET cron_expression = '*/15 * * *' WHERE job_name = 'LargeExport';
UPDATE hangfire_schedule SET cron_expression = '*/30 * * * *' WHERE job_name = 'LargeExport2';
UPDATE hangfire_schedule SET cron_expression = '0 0 ? 1 MON#1' WHERE job_name = 'LargeExport3';
UPDATE hangfire_schedule SET cron_expression = '0 0 L * *' WHERE job_name = 'LargeExport4';

SELECT * FROM hangfire_schedule;


