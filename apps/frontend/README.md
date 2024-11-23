# Snowrunner Merger Frontend

A Vue.js frontend application for the Snowrunner Merger service. This web application provides a user interface for managing Snowrunner save files, groups, and merging functionality.
As Snowrunner does not share progress when playing coop, this application allows players to merge their save files with their friends' save files to keep their progress in sync.

## Related Repositories

This is the frontend web app for the Snowrunner Merger project. For the backend api, see [Snowrunner Merger Api](https://github.com/TheDrinix/snowrunner-merger-api).

## Technologies Used

- Vue 3 (Vite)
- Pinia for state management
- TailwindCSS
- DaisyUI
- Docker

## Features

- Google OAuth Authentication
- Group Management
    - Create and join groups
    - Manage group members
- Save File Management
    - Upload save files to groups
    - Merge saves with group saves
    - Download merged saves
- Responsive UI using DaisyUI components

## Prerequisites

- Node.js (Latest LTS version recommended)
- npm or yarn or pnpm
- Docker (for deployment)

## Development Setup

1. Clone the repository
```bash
git clone https://github.com/yourusername/snowrunner-merger-frontend.git
cd snowrunner-merger-frontend
```

2. Install dependencies
```bash
npm install
# or
yarn install
#or
pnpm install
```

3. Create `.env` file for development
```env
VITE_API_URL=http://localhost:5000  # Your API URL for development
```

4. Start the development server
```bash
npm run dev
# or
yarn dev
# or
pnpm dev
```

The application will be available at `http://localhost:5173`

## Building for Production

### Local Build
```bash
npm run build
# or
yarn build
# or
pnpm build
```

### Docker Deployment

#### Building the Docker Image
```bash
docker build -t snowrunner-merger-frontend .
```

#### Running the Container
```bash
docker run -d -p 80:80 snowrunner-merger-frontend
```

## Project Structure

```
src/
├── assets/          # Static assets
├── components/      # Vue components
├── composables/     # Vue composables
├── stores/          # Pinia stores
├── views/           # Vue views/pages
├── router/          # Vue Router configuration
├── plugins/         # Vue plugins
├── helpers/         # Utility functions
├── types/           # TypeScript types
└── App.vue          # Root component
```

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build locally

## Environment Variables

### Development
- `VITE_API_URL` - API URL for development environment

### Production
- No environment variables needed as the application assumes the API is under the same domain

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support, please [create an issue here](https://github.com/TheDrinix/snowrunner-merger-web/issues).