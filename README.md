# 232127 DelaCruz IPT CRUD

This is a small ASP.NET MVC application (student/course/user CRUD) that connects to SQL Server. The app expects a connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection`.

Quick setup

- Open `appsettings.json` and update `DefaultConnection` to point to your SQL Server instance and desired database. If you are running the app on your Windows host and your server name is `SAMIE` (as shown in your SSMS), set it like this:

```
"ConnectionStrings": {
  "DefaultConnection": "Server=SAMIE;Database=232127DelaCruzSTUDENTCRUD;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

- Run the SQL script to create the database and tables:

Using `sqlcmd`:

```bash
sqlcmd -S localhost\\SQLEXPRESS01 -i database/CreateTables_232127DelaCruz.sql
```

Or open `database/CreateTables_232127DelaCruz.sql` in SQL Server Management Studio and execute it.

Frontend prompt (create tables from UI)

I added a small admin page where you can input your `studentNumber` and `lastName` and the app will create tables named `<studentNumber><lastName>USER`, `<studentNumber><lastName>STUDENT`, and `<studentNumber><lastName>COURSE`.

- Start the app (e.g., `dotnet run` from the `232127DelaCruzIPTCRUD` project folder).
- Visit `/ _232127DelaCruzAdmin/CreateTables` (e.g., `https://localhost:5001/_232127DelaCruzAdmin/CreateTables`) and fill the form.

- On the same admin page there's a **Create User** form. Fill `First Name`, `Last Name`, and `Password` then click **Create User**. The page will show the new `USERID` — use that ID and the password on the login page (`/_232127DelaCruzUser/Login`).

Security note

This admin page performs DDL commands (CREATE TABLE). It validates the inputs to allow only digits for student number and letters for last name, but you should only use it in a development environment and protect it before deploying publicly.

Running inside a dev container or Codespace

- If you're running the app inside a container (Dev Container, Codespace, or Docker), `localhost` in `appsettings.json` refers to the container itself. To connect to SQL Server running on your host machine you can:
  - Use `host.docker.internal` (recommended) or the host IP in the connection string.
  - Use SQL Server authentication (username/password) rather than Windows `Trusted_Connection` because Windows authentication won't work from Linux containers.

Example SQL Auth connection strings (replace credentials):

```
ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=232127DelaCruzSTUDENTCRUD;User Id=sa;Password=YourStrongPassword!;TrustServerCertificate=True;"
```

Set the environment variable in the container shell before running the app:

```bash
export ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=232127DelaCruzSTUDENTCRUD;User Id=sa;Password=YourStrongPassword!;TrustServerCertificate=True;"
dotnet run
```

I added a startup DB connectivity check that logs a helpful message if the app cannot reach your SQL Server instance; check the console logs for guidance.
