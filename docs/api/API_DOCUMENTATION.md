---
post_title: "Tripilot - API Documentation"
author1: "Development Team"
post_slug: "tripilot-api-documentation"
microsoft_alias: "development-team"
featured_image: ""
categories: ["API", "Documentation", "REST"]
tags: ["API", "REST", "Endpoints", "Documentation", "OpenAPI"]
ai_note: "AI assisted in creating this API documentation"
summary: "Comprehensive API documentation for Tripilot application including all endpoints, request/response schemas, authentication, and usage examples"
post_date: "2025-10-15"
---

## API Overview

The Tripilot API is a RESTful web service built with ASP.NET Core Web API following CQRS architecture. It provides endpoints for managing users, routes, places, reviews, audio guides, and business features.

## Base URL

- **Development**: `https://api-dev.tripilot.com`
- **Staging**: `https://api-staging.tripilot.com`
- **Production**: `https://api.tripilot.com`

## Authentication

The API uses JWT (JSON Web Token) for authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### Token Expiration
- Access tokens expire after 1 hour
- Refresh tokens expire after 30 days

## Response Format

All API responses follow a consistent format:

### Success Response
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully",
  "timestamp": "2025-10-15T10:30:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "The request contains invalid data",
    "details": [
      {
        "field": "email",
        "message": "Email is required"
      }
    ]
  },
  "timestamp": "2025-10-15T10:30:00Z"
}
```

## HTTP Status Codes

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Resource already exists
- `422 Unprocessable Entity` - Validation errors
- `429 Too Many Requests` - Rate limit exceeded
- `500 Internal Server Error` - Server error

---

## Authentication Endpoints

### POST /api/auth/register

Register a new user account.

**Request Body:**
```json
{
  "email": "user@example.com",
  "username": "johndoe",
  "password": "SecurePassword123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "user@example.com",
      "username": "johndoe",
      "firstName": "John",
      "lastName": "Doe",
      "role": "Tourist",
      "isBusiness": false
    },
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_here",
    "expiresAt": "2025-10-15T11:30:00Z"
  }
}
```

### POST /api/auth/login

Authenticate user and receive JWT token.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "user": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "email": "user@example.com",
      "username": "johndoe",
      "firstName": "John",
      "lastName": "Doe",
      "role": "Tourist",
      "isBusiness": false,
      "businessSubscriptionTier": "None"
    },
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_here",
    "expiresAt": "2025-10-15T11:30:00Z"
  }
}
```

### POST /api/auth/refresh

Refresh JWT token using refresh token.

**Request Body:**
```json
{
  "refreshToken": "refresh_token_here"
}
```

### POST /api/auth/logout

Logout user and invalidate refresh token.

**Headers:** `Authorization: Bearer <token>`

---

## User Management Endpoints

### GET /api/users/profile

Get current user's profile information.

**Headers:** `Authorization: Bearer <token>`

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "user@example.com",
    "username": "johndoe",
    "firstName": "John",
    "lastName": "Doe",
    "avatar": "https://storage.example.com/avatars/user.jpg",
    "bio": "Travel enthusiast exploring the world",
    "role": "Tourist",
    "isBusiness": false,
    "businessSubscriptionTier": "None",
    "locationPreferences": {
      "preferredCity": "New York",
      "radius": 50
    },
    "preferences": {
      "categories": ["Museums", "Restaurants"],
      "priceRange": "Medium",
      "accessibility": true
    },
    "createdAt": "2025-01-15T08:00:00Z"
  }
}
```

### PUT /api/users/profile

Update current user's profile.

**Headers:** `Authorization: Bearer <token>`

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "bio": "Updated bio text",
  "locationPreferences": {
    "preferredCity": "Los Angeles",
    "radius": 75
  },
  "preferences": {
    "categories": ["Museums", "Entertainment"],
    "priceRange": "High",
    "accessibility": true
  }
}
```

### PUT /api/users/profile/business

Enable business features for user account.

**Headers:** `Authorization: Bearer <token>`

**Request Body:**
```json
{
  "businessName": "John's Restaurant",
  "businessType": "Restaurant",
  "description": "Authentic Italian cuisine",
  "contactEmail": "business@example.com",
  "contactPhone": "+1-555-0123",
  "address": {
    "street": "123 Main St",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001",
    "country": "USA"
  }
}
```

### POST /api/users/avatar

Upload user avatar image.

**Headers:** 
- `Authorization: Bearer <token>`
- `Content-Type: multipart/form-data`

**Form Data:**
- `file`: Image file (JPEG, PNG, max 5MB)

---

## Places Endpoints

### GET /api/places

Get places with filtering and pagination.

**Query Parameters:**
- `page` (int): Page number (default: 1)
- `pageSize` (int): Page size (default: 20, max: 100)
- `categoryId` (UUID): Filter by category
- `city` (string): Filter by city
- `latitude` (decimal): User latitude for distance calculation
- `longitude` (decimal): User longitude for distance calculation
- `radius` (int): Search radius in kilometers (default: 50)
- `search` (string): Search term for name/description
- `minRating` (decimal): Minimum rating filter
- `priceRange` (string): Price range filter
- `sortBy` (string): Sort by (rating, distance, name, created)
- `sortOrder` (string): Sort order (asc, desc)

**Response:**
```json
{
  "success": true,
  "data": {
    "places": [
      {
        "id": "123e4567-e89b-12d3-a456-426614174000",
        "name": "Metropolitan Museum of Art",
        "description": "World-renowned art museum",
        "category": {
          "id": "cat-museums",
          "name": "Museums",
          "icon": "museum"
        },
        "address": "1000 5th Ave, New York, NY 10028",
        "location": {
          "latitude": 40.7794,
          "longitude": -73.9632
        },
        "images": [
          "https://storage.example.com/places/met1.jpg",
          "https://storage.example.com/places/met2.jpg"
        ],
        "avgRating": 4.5,
        "reviewCount": 1250,
        "priceRange": "Medium",
        "isVerified": true,
        "hasAudioGuide": true,
        "operatingHours": {
          "monday": "10:00-17:30",
          "tuesday": "10:00-17:30",
          "wednesday": "10:00-17:30",
          "thursday": "10:00-17:30",
          "friday": "10:00-21:00",
          "saturday": "10:00-21:00",
          "sunday": "10:00-17:30"
        },
        "contactInfo": {
          "phone": "+1-212-535-7710",
          "website": "https://www.metmuseum.org"
        },
        "distance": 2.5
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 157,
      "totalPages": 8,
      "hasNext": true,
      "hasPrevious": false
    }
  }
}
```

### GET /api/places/{id}

Get detailed information about a specific place.

**Path Parameters:**
- `id` (UUID): Place ID

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "name": "Metropolitan Museum of Art",
    "description": "The Metropolitan Museum of Art is the largest art museum in the Americas...",
    "category": {
      "id": "cat-museums",
      "name": "Museums",
      "icon": "museum",
      "hasAudioGuide": true
    },
    "address": "1000 5th Ave, New York, NY 10028",
    "location": {
      "latitude": 40.7794,
      "longitude": -73.9632
    },
    "images": [
      "https://storage.example.com/places/met1.jpg",
      "https://storage.example.com/places/met2.jpg",
      "https://storage.example.com/places/met3.jpg"
    ],
    "avgRating": 4.5,
    "reviewCount": 1250,
    "viewCount": 15678,
    "priceRange": "Medium",
    "features": ["Wheelchair Accessible", "Audio Guide", "Gift Shop", "Café"],
    "amenities": {
      "parking": true,
      "wifi": true,
      "restrooms": true,
      "giftShop": true,
      "café": true
    },
    "accessibility": {
      "wheelchairAccess": true,
      "elevatorAccess": true,
      "audioGuide": true,
      "signLanguage": false
    },
    "operatingHours": {
      "monday": "10:00-17:30",
      "tuesday": "10:00-17:30",
      "wednesday": "10:00-17:30",
      "thursday": "10:00-17:30",
      "friday": "10:00-21:00",
      "saturday": "10:00-21:00",
      "sunday": "10:00-17:30"
    },
    "contactInfo": {
      "phone": "+1-212-535-7710",
      "email": "info@metmuseum.org",
      "website": "https://www.metmuseum.org"
    },
    "businessInfo": {
      "claimedBy": {
        "id": "owner-id",
        "businessName": "Metropolitan Museum",
        "isVerified": true
      },
      "lastUpdated": "2025-10-10T14:30:00Z"
    },
    "hasAudioGuide": true,
    "audioGuideLanguages": ["en", "es", "fr", "de"],
    "createdAt": "2025-01-15T08:00:00Z",
    "updatedAt": "2025-10-10T14:30:00Z"
  }
}
```

### POST /api/places

Create a new place.

**Headers:** `Authorization: Bearer <token>`

**Request Body:**
```json
{
  "name": "New Restaurant",
  "description": "Amazing local cuisine",
  "categoryId": "cat-restaurants",
  "address": "456 Food St, New York, NY 10001",
  "latitude": 40.7589,
  "longitude": -73.9851,
  "contactInfo": {
    "phone": "+1-555-0123",
    "website": "https://example-restaurant.com"
  },
  "operatingHours": {
    "monday": "11:00-22:00",
    "tuesday": "11:00-22:00",
    "wednesday": "11:00-22:00",
    "thursday": "11:00-22:00",
    "friday": "11:00-23:00",
    "saturday": "11:00-23:00",
    "sunday": "12:00-21:00"
  },
  "priceRange": "Medium",
  "features": ["Outdoor Seating", "Delivery", "Takeout"]
}
```

### PUT /api/places/{id}

Update place information (only for place owners or admins).

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Place ID

### PUT /api/places/{id}/claim

Claim ownership of a place (business users only).

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Place ID

**Request Body:**
```json
{
  "verificationDocuments": [
    "https://storage.example.com/docs/business-license.pdf",
    "https://storage.example.com/docs/tax-document.pdf"
  ],
  "notes": "I am the owner of this restaurant"
}
```

### POST /api/places/{id}/images

Upload images for a place.

**Headers:** 
- `Authorization: Bearer <token>`
- `Content-Type: multipart/form-data`

**Path Parameters:**
- `id` (UUID): Place ID

**Form Data:**
- `files`: Image files (JPEG, PNG, max 10MB each, max 10 files)

---

## Audio Guide Endpoints

### GET /api/places/{id}/audio-guide

Get audio guide content for a place.

**Path Parameters:**
- `id` (UUID): Place ID

**Query Parameters:**
- `language` (string): Language code (default: en)

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "audio-guide-id",
    "placeId": "123e4567-e89b-12d3-a456-426614174000",
    "title": "Metropolitan Museum Audio Guide",
    "description": "Explore the highlights of the Met collection",
    "textContent": "Welcome to the Metropolitan Museum of Art...",
    "audioFileUrl": "https://storage.example.com/audio/met-guide-en.mp3",
    "audioDuration": 1800,
    "language": "en",
    "languageCode": "en-US",
    "version": 2,
    "fileSize": 25600000,
    "availableLanguages": [
      {
        "language": "en",
        "languageCode": "en-US",
        "name": "English"
      },
      {
        "language": "es",
        "languageCode": "es-ES",
        "name": "Español"
      }
    ],
    "createdAt": "2025-01-15T08:00:00Z",
    "updatedAt": "2025-10-01T12:00:00Z"
  }
}
```

### POST /api/places/{id}/audio-guide/download

Request download link for offline audio guide.

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Place ID

**Query Parameters:**
- `language` (string): Language code

**Response:**
```json
{
  "success": true,
  "data": {
    "downloadUrl": "https://storage.example.com/audio/met-guide-en.mp3?token=download_token",
    "expiresAt": "2025-10-15T11:30:00Z",
    "fileSize": 25600000,
    "checksum": "sha256:abcd1234..."
  }
}
```

---

## Routes Endpoints

### GET /api/routes

Get routes with filtering and pagination.

**Query Parameters:**
- `page` (int): Page number (default: 1)
- `pageSize` (int): Page size (default: 20, max: 100)
- `city` (string): Filter by city
- `latitude` (decimal): User latitude for distance calculation
- `longitude` (decimal): User longitude for distance calculation
- `radius` (int): Search radius in kilometers (default: 50)
- `search` (string): Search term for name/description
- `minRating` (decimal): Minimum rating filter
- `maxDuration` (int): Maximum duration in minutes
- `difficulty` (string): Difficulty level filter
- `priceRange` (string): Price range filter
- `createdBy` (UUID): Filter by creator
- `tags` (string[]): Filter by tags
- `sortBy` (string): Sort by (rating, duration, created, popular)
- `sortOrder` (string): Sort order (asc, desc)

**Response:**
```json
{
  "success": true,
  "data": {
    "routes": [
      {
        "id": "route-123",
        "name": "NYC Museums Tour",
        "description": "Explore the best museums in Manhattan",
        "duration": 480,
        "difficultyLevel": "Easy",
        "priceRange": "Medium",
        "city": "New York",
        "region": "New York",
        "country": "USA",
        "avgRating": 4.7,
        "reviewCount": 89,
        "viewCount": 2341,
        "completionCount": 156,
        "placeCount": 5,
        "tags": ["Museums", "Culture", "Art"],
        "isFeatured": true,
        "estimatedCost": 75.00,
        "creator": {
          "id": "creator-id",
          "username": "museumfan",
          "firstName": "Jane",
          "lastName": "Smith"
        },
        "coverImage": "https://storage.example.com/routes/nyc-museums.jpg",
        "createdAt": "2025-09-15T10:00:00Z"
      }
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 45,
      "totalPages": 3,
      "hasNext": true,
      "hasPrevious": false
    }
  }
}
```

### GET /api/routes/{id}

Get detailed information about a specific route.

**Path Parameters:**
- `id` (UUID): Route ID

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "route-123",
    "name": "NYC Museums Tour",
    "description": "A comprehensive tour of Manhattan's finest museums...",
    "duration": 480,
    "difficultyLevel": "Easy",
    "priceRange": "Medium",
    "city": "New York",
    "region": "New York",
    "country": "USA",
    "location": {
      "latitude": 40.7794,
      "longitude": -73.9632
    },
    "avgRating": 4.7,
    "reviewCount": 89,
    "viewCount": 2341,
    "completionCount": 156,
    "tags": ["Museums", "Culture", "Art"],
    "isFeatured": true,
    "estimatedCost": 75.00,
    "accessibilityLevel": "High",
    "bestTimeToVisit": "Year-round, weekday mornings recommended",
    "creator": {
      "id": "creator-id",
      "username": "museumfan",
      "firstName": "Jane",
      "lastName": "Smith",
      "avatar": "https://storage.example.com/avatars/jane.jpg"
    },
    "coverImage": "https://storage.example.com/routes/nyc-museums.jpg",
    "places": [
      {
        "id": "123e4567-e89b-12d3-a456-426614174000",
        "name": "Metropolitan Museum of Art",
        "category": "Museums",
        "order": 1,
        "estimatedTime": 120,
        "notes": "Focus on Egyptian and Greek collections",
        "isOptional": false,
        "location": {
          "latitude": 40.7794,
          "longitude": -73.9632
        },
        "images": ["https://storage.example.com/places/met1.jpg"],
        "avgRating": 4.5,
        "hasAudioGuide": true
      },
      {
        "id": "place-2",
        "name": "Museum of Modern Art",
        "category": "Museums",
        "order": 2,
        "estimatedTime": 90,
        "notes": "Don't miss Van Gogh and Picasso",
        "isOptional": false,
        "location": {
          "latitude": 40.7614,
          "longitude": -73.9776
        },
        "images": ["https://storage.example.com/places/moma1.jpg"],
        "avgRating": 4.6,
        "hasAudioGuide": true
      }
    ],
    "isPublic": true,
    "createdAt": "2025-09-15T10:00:00Z",
    "updatedAt": "2025-10-01T14:30:00Z"
  }
}
```

### POST /api/routes

Create a new route.

**Headers:** `Authorization: Bearer <token>`

**Request Body:**
```json
{
  "name": "My Custom Route",
  "description": "A personalized route for art lovers",
  "duration": 300,
  "difficultyLevel": "Easy",
  "city": "New York",
  "region": "New York",
  "country": "USA",
  "isPublic": true,
  "tags": ["Art", "Culture"],
  "places": [
    {
      "placeId": "123e4567-e89b-12d3-a456-426614174000",
      "order": 1,
      "estimatedTime": 120,
      "notes": "Start here early morning",
      "isOptional": false
    },
    {
      "placeId": "place-2",
      "order": 2,
      "estimatedTime": 90,
      "notes": "Great for lunch break",
      "isOptional": false
    }
  ]
}
```

### PUT /api/routes/{id}

Update route information (only for route creators).

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Route ID

### DELETE /api/routes/{id}

Delete a route (only for route creators).

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Route ID

---

## Reviews Endpoints

### GET /api/places/{id}/reviews

Get reviews for a specific place.

**Path Parameters:**
- `id` (UUID): Place ID

**Query Parameters:**
- `page` (int): Page number (default: 1)
- `pageSize` (int): Page size (default: 20)
- `sortBy` (string): Sort by (rating, date, helpful)
- `sortOrder` (string): Sort order (asc, desc)
- `minRating` (int): Minimum rating filter

**Response:**
```json
{
  "success": true,
  "data": {
    "reviews": [
      {
        "id": "review-123",
        "user": {
          "id": "user-456",
          "username": "traveler123",
          "firstName": "John",
          "avatar": "https://storage.example.com/avatars/john.jpg"
        },
        "comment": "Amazing experience! The exhibits were fantastic...",
        "photos": [
          "https://storage.example.com/reviews/photo1.jpg",
          "https://storage.example.com/reviews/photo2.jpg"
        ],
        "visitDate": "2025-10-05",
        "visitDuration": 3,
        "visitContext": "Family",
        "ratings": {
          "overall": 5,
          "value": 4,
          "service": 5,
          "cleanliness": 5,
          "accessibility": 4
        },
        "wouldRecommend": true,
        "helpfulVotes": 12,
        "notHelpfulVotes": 1,
        "businessReply": {
          "message": "Thank you for your wonderful review!",
          "repliedAt": "2025-10-07T09:30:00Z"
        },
        "isVerified": true,
        "createdAt": "2025-10-06T15:20:00Z"
      }
    ],
    "summary": {
      "totalReviews": 1250,
      "averageRating": 4.5,
      "ratingDistribution": {
        "5": 750,
        "4": 350,
        "3": 100,
        "2": 30,
        "1": 20
      },
      "aspectRatings": {
        "value": 4.3,
        "service": 4.6,
        "cleanliness": 4.7,
        "accessibility": 4.2
      }
    },
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 1250,
      "totalPages": 63,
      "hasNext": true,
      "hasPrevious": false
    }
  }
}
```

### POST /api/places/{id}/reviews

Add a review for a place.

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Place ID

**Request Body:**
```json
{
  "comment": "Great museum with amazing collections!",
  "visitDate": "2025-10-10",
  "visitDuration": 3,
  "visitContext": "Solo",
  "ratings": {
    "overall": 5,
    "value": 4,
    "service": 5,
    "cleanliness": 5,
    "accessibility": 4
  },
  "wouldRecommend": true,
  "photos": [
    "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD...",
    "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD..."
  ]
}
```

### POST /api/routes/{id}/reviews

Add a review for a route.

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Route ID

### PUT /api/reviews/{id}/vote

Vote on review helpfulness.

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Review ID

**Request Body:**
```json
{
  "isHelpful": true
}
```

### POST /api/reviews/{id}/report

Report inappropriate review.

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Review ID

**Request Body:**
```json
{
  "reason": "Inappropriate content",
  "details": "Contains offensive language"
}
```

---

## Categories Endpoints

### GET /api/categories

Get all available place categories.

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": "cat-restaurants",
      "name": "Restaurants",
      "icon": "restaurant",
      "description": "Dining establishments and food venues",
      "isActive": true,
      "hasAudioGuide": false,
      "placeCount": 1250
    },
    {
      "id": "cat-museums",
      "name": "Museums",
      "icon": "museum",
      "description": "Cultural institutions and exhibitions",
      "isActive": true,
      "hasAudioGuide": true,
      "placeCount": 89
    }
  ]
}
```

---

## User Favorites Endpoints

### GET /api/users/favorites

Get user's favorite places and routes.

**Headers:** `Authorization: Bearer <token>`

**Query Parameters:**
- `type` (string): Filter by type (places, routes)
- `page` (int): Page number
- `pageSize` (int): Page size

### POST /api/users/favorites

Add item to favorites.

**Headers:** `Authorization: Bearer <token>`

**Request Body:**
```json
{
  "targetType": "Place",
  "targetId": "123e4567-e89b-12d3-a456-426614174000"
}
```

### DELETE /api/users/favorites/{id}

Remove item from favorites.

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Favorite ID

---

## Business Endpoints

### GET /api/business/dashboard

Get business dashboard data.

**Headers:** `Authorization: Bearer <token>` (Business users only)

**Response:**
```json
{
  "success": true,
  "data": {
    "overview": {
      "claimedPlaces": 3,
      "totalViews": 15678,
      "totalReviews": 89,
      "averageRating": 4.5,
      "subscriptionTier": "Premium"
    },
    "places": [
      {
        "id": "place-1",
        "name": "My Restaurant",
        "category": "Restaurants",
        "views": 5432,
        "reviews": 34,
        "rating": 4.6,
        "status": "Active"
      }
    ],
    "recentReviews": [
      {
        "id": "review-1",
        "placeName": "My Restaurant",
        "rating": 5,
        "comment": "Excellent food and service!",
        "user": "johndoe",
        "createdAt": "2025-10-14T18:30:00Z",
        "needsResponse": true
      }
    ],
    "analytics": {
      "viewsThisMonth": 2341,
      "reviewsThisMonth": 12,
      "averageRatingThisMonth": 4.7,
      "topViewedPlace": "My Restaurant"
    }
  }
}
```

### POST /api/business/subscription

Subscribe to premium business features.

**Headers:** `Authorization: Bearer <token>`

**Request Body:**
```json
{
  "planType": "Premium",
  "paymentMethodId": "pm_1234567890",
  "billingCycle": "monthly"
}
```

---

## Search Endpoints

### GET /api/search

Global search across places and routes.

**Query Parameters:**
- `q` (string): Search query (required)
- `type` (string): Search type (places, routes, all)
- `latitude` (decimal): User latitude
- `longitude` (decimal): User longitude
- `radius` (int): Search radius in kilometers
- `page` (int): Page number
- `pageSize` (int): Page size

**Response:**
```json
{
  "success": true,
  "data": {
    "places": [
      {
        "id": "place-1",
        "name": "Metropolitan Museum",
        "type": "place",
        "category": "Museums",
        "rating": 4.5,
        "distance": 2.1,
        "image": "https://storage.example.com/places/met1.jpg"
      }
    ],
    "routes": [
      {
        "id": "route-1",
        "name": "NYC Museums Tour",
        "type": "route",
        "duration": 480,
        "rating": 4.7,
        "placeCount": 5,
        "image": "https://storage.example.com/routes/nyc.jpg"
      }
    ],
    "suggestions": [
      "Metropolitan Museum of Art",
      "Museum of Modern Art",
      "NYC Museum Tour"
    ]
  }
}
```

---

## Notifications Endpoints

### GET /api/notifications

Get user notifications.

**Headers:** `Authorization: Bearer <token>`

**Query Parameters:**
- `unreadOnly` (bool): Show only unread notifications
- `page` (int): Page number
- `pageSize` (int): Page size

### PUT /api/notifications/{id}/read

Mark notification as read.

**Headers:** `Authorization: Bearer <token>`

**Path Parameters:**
- `id` (UUID): Notification ID

---

## Error Codes

| Code | Description |
|------|-------------|
| `VALIDATION_ERROR` | Request validation failed |
| `AUTHENTICATION_REQUIRED` | Valid authentication token required |
| `INSUFFICIENT_PERMISSIONS` | User lacks required permissions |
| `RESOURCE_NOT_FOUND` | Requested resource does not exist |
| `RESOURCE_ALREADY_EXISTS` | Resource with same identifier already exists |
| `RATE_LIMIT_EXCEEDED` | Too many requests from client |
| `EXTERNAL_SERVICE_ERROR` | External service unavailable |
| `DATABASE_ERROR` | Database operation failed |
| `FILE_UPLOAD_ERROR` | File upload failed |
| `PAYMENT_ERROR` | Payment processing failed |

## Rate Limiting

API requests are rate-limited per user:

- **Authenticated users**: 1000 requests/hour
- **Anonymous users**: 100 requests/hour
- **Premium business users**: 5000 requests/hour

Rate limit headers are included in all responses:
- `X-RateLimit-Limit`: Request limit per window
- `X-RateLimit-Remaining`: Remaining requests in current window
- `X-RateLimit-Reset`: Time when rate limit resets (Unix timestamp)

## Webhooks

For business users, webhooks can be configured to receive real-time notifications:

### Webhook Events
- `review.created` - New review added
- `review.updated` - Review updated or responded to
- `place.claimed` - Place claimed by business
- `subscription.updated` - Subscription status changed

### Webhook Payload Example
```json
{
  "event": "review.created",
  "timestamp": "2025-10-15T10:30:00Z",
  "data": {
    "reviewId": "review-123",
    "placeId": "place-456",
    "placeName": "My Restaurant",
    "rating": 5,
    "userId": "user-789"
  }
}
```

## SDK and Libraries

Official SDKs are available for:
- JavaScript/TypeScript (npm package)
- Python (pip package)
- C# (.NET package)

Example JavaScript usage:
```javascript
import { TripilotAPI } from '@tripilot/api-client';

const api = new TripilotAPI({
  baseUrl: 'https://api.tripilot.com',
  apiKey: 'your-api-key'
});

const places = await api.places.search({
  city: 'New York',
  category: 'Museums',
  page: 1
});
```

This comprehensive API documentation provides all the endpoints, request/response formats, authentication details, and usage examples needed to integrate with the Tripilot platform.