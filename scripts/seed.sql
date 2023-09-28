INSERT INTO public."AspNetUsers"("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount")
	VALUES ('e0616888-3719-4559-ae23-daf5503845d4', 'pedro_csproj', 'PEDRO_CSPROJ', 'email@provider.com', 'EMAIL@PROVIDER.COM', true, '', '', '581705ac-c85b-47a4-9a46-d83f136cf894', '', false, false, NOW(), false, 0);

INSERT INTO public."AspNetRoles"("Id", "Name", "NormalizedName", "ConcurrencyStamp")
    VALUES ('d86839bf-a346-41cd-9d5d-a5bcd4132fb3', 'admin', 'ADMIN', 'c0899a86-0d3d-48f8-9b2f-9489c2b5d55e');

INSERT INTO public."AspNetUserRoles"("UserId", "RoleId")
	VALUES ('e0616888-3719-4559-ae23-daf5503845d4', 'd86839bf-a346-41cd-9d5d-a5bcd4132fb3');

INSERT INTO public."AspNetRoleClaims"("Id", "RoleId", "ClaimType", "ClaimValue")
	VALUES 
		(1, 'd86839bf-a346-41cd-9d5d-a5bcd4132fb3', 'policy', 'users.list'), 
		(2, 'd86839bf-a346-41cd-9d5d-a5bcd4132fb3', 'policy', 'users.register'), 
		(3, 'd86839bf-a346-41cd-9d5d-a5bcd4132fb3', 'policy', 'roles.create'), 
		(4, 'd86839bf-a346-41cd-9d5d-a5bcd4132fb3', 'policy', 'roles.listPolicies'),
		(5, 'd86839bf-a346-41cd-9d5d-a5bcd4132fb3', 'policy', 'roles.addClaimsToRole'),
		(6, 'd86839bf-a346-41cd-9d5d-a5bcd4132fb3', 'policy', 'roles.listRoles');
