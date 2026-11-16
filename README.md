# RevayatCMS

**An ASP.NET MVC content management system for Persian editorial publishing.**

RevayatCMS combines a responsive, right-to-left reading experience with an administration area for publishing articles, organizing categories, and tracking content activity. The application uses ASP.NET Identity for reader and administrator accounts.

## Features

| Public experience | Administration and accounts |
| --- | --- |
| Featured articles, archive, category pages, search, and popular content | Article and category management |
| Article details with images, tags, and comments | Image upload and content editing |
| Responsive Persian RTL interface | Dashboard with article, category, comment, and visit totals |
| Registration, sign-in, profile, and password change | Seven-day publishing chart and recent articles |

Comments require a signed-in account. The admin area is accessible only to users in the `Admin` role.

## Tech stack

| Concern | Technology |
| --- | --- |
| Application | ASP.NET MVC 5, Razor, .NET Framework 4.8 |
| Authentication | ASP.NET Identity 2.2.1, OWIN cookie authentication |
| Persistence | Entity Framework 6.2, SQL Server LocalDB |
| Frontend | Custom RTL CSS, Bootstrap, jQuery, Font Awesome, CKEditor |
| Build and checks | MSBuild, NuGet, GitHub Actions on Windows |

This is a classic ASP.NET MVC application targeting **.NET Framework 4.8**.

## Architecture

```text
RevayatCMS.sln
├── Revayat.Web/
│   ├── Controllers/          Public pages, search, news, and accounts
│   ├── Areas/Admin/          Role-protected dashboard and content management
│   ├── App_Start/            Routes, Identity configuration, and initialization
│   ├── Models/               Identity and web view models
│   ├── Views/                Razor views for the public site and accounts
│   └── Content/              Styles, scripts, fonts, and images
├── DataLayer/
│   ├── Models/               Content entities
│   ├── Repositories/         Content data access
│   └── Migrations/           Entity Framework content migrations
├── tests/                    Focused password and upload checks
└── .github/workflows/        Windows build workflow
```

The application separates content data from account data:

- **`RevayatContext` / `RevayatDB`** stores articles, categories, and comments. Entity Framework applies the included content migrations.
- **`RevayatIdentityContext` / `RevayatIdentityDB`** stores Identity users and roles. It is created when the application first runs.

Authentication uses OWIN cookies and ASP.NET Identity. New registrations receive the `User` role; admin controllers require `Admin`. The existing content database also contains a historical `AdminLogins` model, but active sign-in uses Identity.

## Run locally

### Requirements

- Windows and Visual Studio with the **ASP.NET and web development** workload
- .NET Framework 4.8 targeting pack
- SQL Server LocalDB, or a SQL Server instance configured in `Revayat.Web/Web.config`
- NuGet and MSBuild

The Visual Studio Web Application targets are required; installing only the .NET SDK is insufficient for this project.

### Build and launch

1. Restore packages and build the solution from Visual Studio Developer PowerShell:

   ```powershell
   nuget restore RevayatCMS.sln
   dotnet build RevayatCMS.sln --no-restore -m:1
   ```

2. Review the `RevayatContext` and `RevayatIdentityContext` connection strings in `Revayat.Web/Web.config`.

3. To initialize the first administrator, set the following variables in the environment that starts Visual Studio or IIS Express:

   ```powershell
   $env:REVAYAT_ADMIN_USER = "your-admin-name"
   $env:REVAYAT_ADMIN_EMAIL = "you@example.com"
   $env:REVAYAT_ADMIN_PASSWORD = "<your-strong-password>"
   ```

   The password must be at least 12 characters long and include uppercase and lowercase letters and a digit. Initialization creates an admin only when no `Admin` account exists.

4. Open `RevayatCMS.sln`, select `Revayat.Web` as the startup project, and run it. Sign in at `/Account/Login` or register a reader at `/Account/Register`.

Start with a category, then publish an article from `/Admin`. The public home page displays a presentation state until content has been added.

## Key implementation details

- **Password storage:** PBKDF2-SHA256 with a random salt and 210,000 iterations, integrated with Identity's `UserManager`.
- **Account protection:** five failed sign-in attempts trigger a 15-minute lockout. Authentication cookies are HTTP-only, with secure cookies in Release builds.
- **Authorization:** admin controllers use role-based authorization; reader comments require an authenticated account.
- **Request validation:** account forms, admin write actions, and comment submission use anti-forgery tokens.
- **Image handling:** article uploads accept JPEG/PNG up to 5 MB, validate decoded content and dimensions, and save re-encoded files with generated names.
- **Dashboard data:** counters and the seven-day chart are calculated from stored content rather than static UI values.
