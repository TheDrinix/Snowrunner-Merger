# 🏔️ Snowrunner Merger
**Snowrunner Merger** is a full-stack solution designed to solve the lack of shared progress in Snowrunner co-op sessions. This application allows players to merge their save files with their friends' files, ensuring everyone stays in sync.

This repository is a monorepo containing both the Vue.js frontend and the .NET 8 API.

## 🏗️ Project Structure
```
.
├── apps/           
│   ├── api/                   # .NET 8 Web API
│   └── frontend/             # Vue 3 Web Application
├── docker-compose.prod.yml   # Orchestration for production
└── README.md                 # This file
```

## 🛠️ Tech Stack
|Feature   |Frontend             |Backend (API)                |
|----------|---------------------|-----------------------------|
|Framework |Vue 3 (Vite)         |ASP.NET Core (.NET 8.0)      |
|State/Data|Pinia                |EF Core + PostgreSQL         |
|Styling   |TailwindCSS + DaisyUI|Swagger / OpenAPI            |
|Auth      |Mail+Password + OAuth|JWT Bearer + OAuth 2.0       |
|Deployment|Docker               |Docker                       |

## 🚀 Quick Start
1. **Prerequisites**
    * Node.js (Latest LTS)
    * .NET 8.0 SDK
    * PostgreSQL instance
    * Docker (Optional, for containerized setup)

2. **Local Development Setup**

- **The API**
  1. Navigate to the api directory: `cd apps/api`
  2. Restore dependencies: `dotnet restore`
  3. Configure your `appsettings.example.json` (see [Example configuration](apps/api/appsettings.example.json) below).
  4. Apply migrations: `dotnet ef database update`
  5. Run the service: `dotnet run`
  6. The API will be available at `https://localhost:44303`

- **The Frontend**
  1. Navigate to the frontend directory: `cd apps/frontend`
  1. Install dependencies: `npm install`
  1. Create a .env file: `VITE_API_URL=https://localhost:44303`
  1. Start the dev server: `npm run dev`
  1. The UI will be available at `http://localhost:5173`

## ⚙️ Configuration
The system requires several environment variables/settings to function correctly, particularly for OAuth and Database connectivity.

### Backend (API)
You can configure these in `apps/api/appsettings.json` or via environment variables:
- **OAuth Credentials** for Discord and Google
- **JWT** - JwtSecret and RefreshSecret (Refresh secret must be 32 characters)
- **SMTP** - Host, Port, Username and Password
- **Database** connection string

### Frontend
Create a `apps/frontend/.env` file containing api url if it differs from frontend

## 🤝 Contributing
1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/AmazingFeature`.
1. Commit your changes: `git commit -m 'Add some AmazingFeature'`.
1. Push to the branch: `git push origin feature/AmazingFeature`.
1. Open a Pull Request.

## 📄 License
This project is licensed under the MIT [License](LICENSE) - see the LICENSE file for details.

## Support

For support, please [create an issue here](https://github.com/TheDrinix/snowrunner-merger/issues).