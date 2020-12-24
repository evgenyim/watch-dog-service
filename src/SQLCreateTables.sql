create table Services(
	Id int primary key,
	"Type" varchar(50),
	"Url" varchar(50),
	TimeCheck int
)

create table WebServices(
	Id int primary key,
	CheckUrl varchar(50),
	foreign key (Id) references Services(Id)
)

create table Denials(
	Id int primary key,
	ServiceId int,
	StartWorking int,
	Time datetime
	foreign key (ServiceId) references Services(Id)
	)

