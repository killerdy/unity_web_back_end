create table usertable(
    sno int PRIMARY KEY auto_increment,
    username varchar(20) not null,
    password varchar(20) not null,
    qq varchar(20) not null,
    phone varchar(20) unique not null
);
insert into usertable(username,password,qq,phone) values("killerdy","123321","3045719705","19895620129");
create table lwy(
    sno int PRIMARY KEY  auto_increment,
    username varchar(20),
    password varchar(20),
    qq varchar(20),
    phone varchar(20)
);
