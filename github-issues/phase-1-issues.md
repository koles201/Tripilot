# GitHub Issues for Phase 1: MVP

This file contains formatted GitHub issues that can be created in the repository. Each issue includes title, description, acceptance criteria, and labels.

---

## Issue #1: Initialize ASP.NET Core Web API project structure

**Labels:** `epic`, `phase-1`, `backend`, `setup`
**Milestone:** Phase 1: MVP
**Estimated Time:** 8-12 hours

### Description
Set up the foundational ASP.NET Core Web API project structure following Clean Architecture principles with CQRS pattern.

### Acceptance Criteria
- [ ] Create solution with 5 projects: Api, Application, Domain, Infrastructure, Shared
- [ ] Configure proper dependency injection between projects
- [ ] Set up development environment configuration with appsettings.json
- [ ] Implement proper project references and dependencies
- [ ] Add basic health check endpoint
- [ ] Configure Swagger/OpenAPI documentation
- [ ] Set up logging with Serilog or built-in logging

### Technical Requirements
- Target .NET 8.0
- Use Clean Architecture folder structure
- Implement dependency injection container setup
- Configure environment-specific settings (Development, Staging, Production)

### Value to Project
- **Foundation**: Establishes the core architectural foundation for the entire backend
- **Scalability**: Clean Architecture enables easy scaling and maintenance
- **Team Productivity**: Clear separation of concerns makes development faster
- **Quality**: Proper structure enforces best practices from day one

### Definition of Done
- [ ] Solution builds without errors
- [ ] All projects have correct references
- [ ] Health check endpoint returns 200 OK
- [ ] Swagger UI is accessible at /swagger
- [ ] Development configuration loads correctly
- [ ] Code follows established naming conventions

---

## Issue #2: Set up Entity Framework Core with PostgreSQL

**Labels:** `feature`, `phase-1`, `backend`, `database`
**Milestone:** Phase 1: MVP
**Estimated Time:** 10-14 hours

### Description
Configure Entity Framework Core with PostgreSQL database using Code-First approach. Implement base entities, DbContext, and migration system.

### Acceptance Criteria
- [ ] Install and configure Entity Framework Core packages
- [ ] Set up PostgreSQL connection string configuration
- [ ] Create base entity classes (BaseEntity, IAuditableEntity)
- [ ] Implement ApplicationDbContext with proper configurations
- [ ] Set up migration system and initial database creation
- [ ] Configure database seeding for development data
- [ ] Implement Unit of Work pattern

### Technical Requirements
- Use Entity Framework Core 8.0
- PostgreSQL as primary database
- Code-First approach with migrations
- Proper entity configurations and relationships
- Connection pooling and performance optimizations

### Value to Project
- **Data Layer**: Establishes reliable data persistence layer
- **Type Safety**: Strong typing with Entity Framework reduces runtime errors
- **Maintainability**: Code-First migrations make database changes trackable
- **Performance**: Proper EF configuration ensures optimal database performance

### Definition of Done
- [ ] Database connection established successfully
- [ ] Initial migration creates database schema
- [ ] Seed data populates development database
- [ ] Unit of Work pattern implemented and tested
- [ ] Database performance logging configured

---

## Issue #3: Implement user registration and login system

**Labels:** `feature`, `phase-1`, `backend`, `authentication`
**Milestone:** Phase 1: MVP
**Estimated Time:** 12-16 hours

### Description
Implement secure user authentication system with JWT tokens, user registration, login, and role-based authorization for Tourists and Business Owners.

### Acceptance Criteria
- [ ] Create User entity with proper authentication fields
- [ ] Implement password hashing using BCrypt or similar
- [ ] Create registration endpoint with email verification
- [ ] Implement login endpoint with JWT token generation
- [ ] Set up role-based authorization (Tourist, BusinessOwner, Admin)
- [ ] Add token refresh mechanism
- [ ] Implement logout functionality
- [ ] Add password reset via email

### Technical Requirements
- JWT token authentication with configurable expiration
- Secure password hashing (BCrypt, Argon2, or PBKDF2)
- Email verification for new accounts
- Role-based authorization attributes
- Refresh token mechanism for security

### Value to Project
- **Security**: Secure authentication protects user data and system integrity
- **User Experience**: Seamless login/logout experience for users
- **Authorization**: Role-based access control enables different user types
- **Compliance**: Proper authentication meets security standards

### Definition of Done
- [ ] Users can register with email verification
- [ ] Login returns valid JWT token
- [ ] Role-based endpoints work correctly
- [ ] Token refresh mechanism implemented
- [ ] Password reset flow functional
- [ ] Security tests pass

---

## Issue #4: Create basic Place entity and management

**Labels:** `feature`, `phase-1`, `backend`, `core`
**Milestone:** Phase 1: MVP
**Estimated Time:** 10-14 hours

### Description
Implement the core Place entity system with CRUD operations, including restaurants, attractions, and accommodations with basic information and location data.

### Acceptance Criteria
- [ ] Create Place entity with all required fields
- [ ] Implement PlaceType enumeration (Restaurant, Attraction, Accommodation)
- [ ] Create CQRS commands and queries for Place operations
- [ ] Implement Place repository with Entity Framework
- [ ] Add Place controller with CRUD endpoints
- [ ] Implement basic search functionality by name and type
- [ ] Add location-based queries (latitude/longitude)

### Technical Requirements
- Entity Framework Code-First Place entity
- CQRS pattern for commands and queries
- Repository pattern for data access
- RESTful API endpoints following OpenAPI specifications
- Geospatial queries for location-based searches

### Value to Project
- **Core Feature**: Places are the foundation of the route system
- **Scalability**: Proper entity design allows for feature expansion
- **Search**: Location-based search enables core functionality
- **API Foundation**: Establishes patterns for other entities

### Definition of Done
- [ ] Place entity created and migrated to database
- [ ] All CRUD operations work via API
- [ ] Search by name and type functional
- [ ] Location-based queries return correct results
- [ ] API endpoints documented in Swagger
- [ ] Unit tests cover core functionality

---

## Issue #5: Implement Route creation and management

**Labels:** `feature`, `phase-1`, `backend`, `core`
**Milestone:** Phase 1: MVP
**Estimated Time:** 14-18 hours

### Description
Create the Route system allowing users to create, manage, and share routes containing multiple places with ordering and timing information.

### Acceptance Criteria
- [ ] Create Route entity with relationship to Places
- [ ] Implement RoutePlace join entity for ordering and timing
- [ ] Create CQRS commands for route creation and management
- [ ] Implement route queries with place details
- [ ] Add route sharing functionality (public/private routes)
- [ ] Create route duplication feature
- [ ] Implement route statistics and analytics

### Technical Requirements
- Many-to-many relationship between Routes and Places
- Route ordering system for place sequence
- Time estimation for route completion
- Public/private route visibility settings
- Route analytics and usage tracking

### Value to Project
- **Core Feature**: Routes are the primary user-generated content
- **Social Aspect**: Route sharing creates community engagement
- **User Value**: Personalized route creation is key differentiator
- **Business Model**: Popular routes drive engagement

### Definition of Done
- [ ] Route entity with Places relationship implemented
- [ ] Route creation and editing endpoints functional
- [ ] Route sharing works (public/private visibility)
- [ ] Route duplication feature implemented
- [ ] Route statistics tracked correctly
- [ ] API endpoints tested and documented

---

## Issue #6: Create basic Review system

**Labels:** `feature`, `phase-1`, `backend`, `core`
**Milestone:** Phase 1: MVP
**Estimated Time:** 10-12 hours

### Description
Implement review system allowing users to rate and review places with multiple rating aspects and helpful vote system.

### Acceptance Criteria
- [ ] Create Review entity with multiple rating categories
- [ ] Implement rating aspects (Overall, Service, Value, Cleanliness, etc.)
- [ ] Add review text with moderation capabilities
- [ ] Create helpful vote system for reviews
- [ ] Implement review aggregation and statistics
- [ ] Add review reporting and moderation features

### Technical Requirements
- Multi-aspect rating system (1-5 stars per category)
- Review text with character limits
- Helpful vote tracking system
- Review aggregation calculations
- Basic content moderation flags

### Value to Project
- **Trust Building**: Reviews create trust and transparency
- **Quality Control**: User feedback improves place quality
- **Engagement**: Review system increases user interaction
- **Business Intelligence**: Reviews provide valuable insights

### Definition of Done
- [ ] Review entity implemented with rating aspects
- [ ] Users can create and edit reviews
- [ ] Review aggregation calculations work
- [ ] Helpful vote system functional
- [ ] Review moderation flags implemented
- [ ] API endpoints tested and documented

---

## Issue #7: Set up Google Maps API integration

**Labels:** `feature`, `phase-1`, `backend`, `integration`
**Milestone:** Phase 1: MVP
**Estimated Time:** 8-12 hours

### Description
Integrate Google Maps API for geocoding, place details, and directions functionality to support location-based features.

### Acceptance Criteria
- [ ] Configure Google Maps API credentials and services
- [ ] Implement geocoding service for address to coordinates
- [ ] Create reverse geocoding for coordinates to address
- [ ] Add place details lookup from Google Places API
- [ ] Implement directions service for route planning
- [ ] Create distance matrix calculations
- [ ] Add place photo retrieval functionality

### Technical Requirements
- Google Maps Platform API integration
- Proper API key management and security
- Rate limiting and quota management
- Error handling for API failures
- Caching for frequently accessed data

### Value to Project
- **Accuracy**: Google Maps provides accurate location data
- **Rich Content**: Place photos and details enhance user experience
- **Navigation**: Directions enable practical route usage
- **Global Coverage**: Works worldwide for international users

### Definition of Done
- [ ] Google Maps API configured and functional
- [ ] Geocoding services work correctly
- [ ] Place details retrieval implemented
- [ ] Directions API integrated
- [ ] API rate limiting implemented
- [ ] Error handling covers all failure scenarios

---

## Issue #8: Create React application structure

**Labels:** `feature`, `phase-1`, `frontend`, `setup`
**Milestone:** Phase 1: MVP
**Estimated Time:** 8-12 hours

### Description
Set up React application with TypeScript, Material-UI, and Redux Toolkit following modern React development practices.

### Acceptance Criteria
- [ ] Create React app with TypeScript template
- [ ] Configure Material-UI theme and component library
- [ ] Set up Redux Toolkit for state management
- [ ] Implement React Router for navigation
- [ ] Configure Axios for API communication
- [ ] Set up environment configuration
- [ ] Create basic layout components

### Technical Requirements
- React 18+ with TypeScript
- Material-UI v5 for component library
- Redux Toolkit for state management
- React Router v6 for routing
- Axios for HTTP requests
- Environment-based configuration

### Value to Project
- **Modern Stack**: Latest React features and best practices
- **Type Safety**: TypeScript reduces runtime errors
- **UI Consistency**: Material-UI provides consistent design
- **State Management**: Redux enables complex state handling

### Definition of Done
- [ ] React app builds and runs without errors
- [ ] Material-UI theme configured and working
- [ ] Redux store configured with basic slices
- [ ] Routing works for different pages
- [ ] API service layer implemented
- [ ] Basic layout components created

---

## Issue #9: Implement authentication UI components

**Labels:** `feature`, `phase-1`, `frontend`, `authentication`
**Milestone:** Phase 1: MVP
**Estimated Time:** 12-16 hours

### Description
Create user interface components for registration, login, and user profile management with proper form validation and error handling.

### Acceptance Criteria
- [ ] Create registration form with validation
- [ ] Implement login form with error handling
- [ ] Add user profile management interface
- [ ] Create password reset functionality UI
- [ ] Implement role-based UI elements
- [ ] Add authentication state management
- [ ] Create protected route components

### Technical Requirements
- Form validation using Formik or React Hook Form
- Material-UI form components
- Redux authentication state management
- Protected routes with React Router
- JWT token storage and management
- Error handling and user feedback

### Value to Project
- **User Onboarding**: Smooth registration increases user adoption
- **Security**: Proper authentication UI ensures security
- **User Experience**: Intuitive forms improve satisfaction
- **Access Control**: Role-based UI provides appropriate access

### Definition of Done
- [ ] Registration form works with backend API
- [ ] Login form authenticates users successfully
- [ ] User profile editing functional
- [ ] Password reset flow completed
- [ ] Protected routes work correctly
- [ ] Authentication state managed properly

---

## Issue #10: Create place listing and search UI

**Labels:** `feature`, `phase-1`, `frontend`, `core`
**Milestone:** Phase 1: MVP
**Estimated Time:** 14-18 hours

### Description
Build user interface for browsing, searching, and viewing place details with filtering and sorting capabilities.

### Acceptance Criteria
- [ ] Create place list view with pagination
- [ ] Implement search functionality with filters
- [ ] Add place detail view with comprehensive information
- [ ] Create place type filtering (Restaurant, Attraction, etc.)
- [ ] Implement location-based search with map view
- [ ] Add sorting options (rating, distance, name)
- [ ] Create responsive design for mobile devices

### Technical Requirements
- Material-UI components for place cards and lists
- Search functionality with debouncing
- Google Maps integration for location display
- Responsive design using Material-UI breakpoints
- Infinite scrolling or pagination for large lists
- Filter and sort state management

### Value to Project
- **Discovery**: Users can easily find places of interest
- **User Experience**: Intuitive search and filtering
- **Mobile First**: Responsive design supports mobile users
- **Engagement**: Rich place details increase user engagement

### Definition of Done
- [ ] Place listing displays correctly with data
- [ ] Search functionality works with backend API
- [ ] Filtering by type and location functional
- [ ] Place detail view shows complete information
- [ ] Responsive design works on mobile devices
- [ ] Sorting and pagination implemented