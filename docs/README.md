# Tripilot Documentation

Welcome to the Tripilot project documentation. This directory contains comprehensive documentation for the tourist route application including technical specifications, architecture designs, and development guides.

## Documentation Structure

```
docs/
├── PROJECT_SPECIFICATION.md           # Complete project specification
├── development/
│   └── DEVELOPMENT_FEATURES_TASKS.md  # Development roadmap and tasks
├── architecture/
│   └── TECHNICAL_ARCHITECTURE.md     # System architecture and design
├── database/
│   └── DATABASE_SCHEMA.md            # Database design and schema
└── api/
    └── API_DOCUMENTATION.md          # REST API documentation
```

## Quick Start

1. **Start Here**: Read the [Project Specification](PROJECT_SPECIFICATION.md) for a complete overview
2. **Development Planning**: Check the [Development Features & Tasks](development/DEVELOPMENT_FEATURES_TASKS.md) for implementation roadmap
3. **Technical Details**: Review the [Technical Architecture](architecture/TECHNICAL_ARCHITECTURE.md) for system design
4. **Database Design**: Examine the [Database Schema](database/DATABASE_SCHEMA.md) for data modeling
5. **API Reference**: Use the [API Documentation](api/API_DOCUMENTATION.md) for endpoint specifications

## Project Overview

Tripilot is a comprehensive web application designed to connect tourists with local businesses through curated tourist routes. The platform serves two primary user types:

- **Tourists**: Seeking authentic local experiences through curated routes
- **Business Owners**: Wanting to showcase their services to visitors

### Key Features

- **Route Management**: Create, browse, and share tourist routes
- **Place Discovery**: Explore restaurants, museums, entertainment venues, and more
- **Audio Guides**: Professional narrated guides for museums and historical sites
- **Review System**: Detailed multi-aspect reviews and ratings
- **Business Features**: Free and premium tiers for business promotion
- **Location Services**: GPS-based recommendations and navigation

### Technology Stack

#### Frontend
- **React 18+** with TypeScript
- **Material-UI (MUI)** for design system
- **Redux Toolkit** for state management
- **Google Maps API** for location services

#### Backend
- **ASP.NET Core Web API**
- **CQRS Architecture** with MediatR
- **Entity Framework Core Code-First** with PostgreSQL
- **JWT Authentication** with role-based authorization

## Development Phases

### Phase 1: MVP (8-10 weeks)
- User authentication and profiles
- Basic place and route management
- Simple review system
- Maps integration
- Responsive design

### Phase 2: Core Features (6-8 weeks)
- Business user features
- Advanced search and filtering
- Social features and sharing
- Notification system
- Enhanced mobile experience

### Phase 3: Advanced Features (8-10 weeks)
- **Audio guide system**
- Premium business subscriptions
- AI-powered recommendations
- Advanced analytics
- Multi-language support

### Phase 4: Enterprise & Scale (6-8 weeks)
- Performance optimization
- Advanced security features
- Third-party integrations
- Enterprise-level capabilities

## Documentation Standards

All documentation follows the established [Markdown Instructions](.github/instructions/markdown.instructions.md):

- Use appropriate heading levels (H2, H3, etc.)
- Include proper front matter with metadata
- Maintain consistent formatting and structure
- Limit line length for readability
- Use fenced code blocks with language specification

## Contributing

When updating documentation:

1. Follow the existing structure and naming conventions
2. Update this README if adding new documentation files
3. Ensure all links are valid and accessible
4. Include proper front matter in all markdown files
5. Test code examples and API endpoints

## Additional Resources

- **GitHub Repository**: [https://github.com/koles201/Tripilot](https://github.com/koles201/Tripilot)
- **Project Management**: Development tasks and progress tracking
- **Design Assets**: UI/UX mockups and design system
- **Deployment Guides**: Environment setup and deployment instructions

## Contact

For questions or clarifications about the documentation or project:

- **Development Team**: development-team@tripilot.com
- **Project Manager**: project-manager@tripilot.com
- **Technical Lead**: tech-lead@tripilot.com

---

*Last Updated: October 15, 2025*