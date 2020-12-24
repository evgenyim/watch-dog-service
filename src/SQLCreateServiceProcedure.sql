create procedure InsertService(@Id int, @Url varchar(max), @CheckUrl varchar(max), @TimeCheck int)
as
begin tran
			if exists (select * from Services where Id = @Id)
			begin
				update Services
				set TimeCheck = @TimeCheck where Id = @Id
				update WebServices
				set CheckUrl = @CheckUrl where Id = @Id
			end
			else
			begin
				insert into Services
				values (@Id, 'WebService', @Url, @TimeCheck)
				insert into WebServices
				values (@Id, @CheckUrl)
			end
commit tran