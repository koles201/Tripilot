# Route Sharing and Social Features

This document describes the comprehensive route sharing and social features implementation for the Tripilot platform.

## Overview

The route sharing and social features enable users to:
- Share routes via unique shareable links
- Embed routes on external websites
- Follow other users and see their activities
- Create and manage route collections
- View activity feeds from followed users

## Backend Implementation

### Domain Entities

#### UserFollow
Represents a following relationship between users.
- `FollowerId`: User who is following
- `FollowingId`: User being followed
- `IsNotified`: Whether the followed user has been notified
- Supports follower/following counts on User entity

#### RouteCollection
Represents a collection or list of routes.
- `Name`: Collection name
- `Description`: Optional description
- `UserId`: Owner of the collection
- `IsPublic`: Visibility setting
- `CoverImageUrl`: Optional cover image
- Contains multiple `RouteCollectionItem`s

#### RouteCollectionItem
Represents a route within a collection.
- `CollectionId`: Parent collection
- `RouteId`: Route reference
- `Order`: Position in collection
- `Note`: Optional note about the route

#### UserActivity
Tracks user activities for activity feeds.
- `UserId`: User who performed the activity
- `ActivityType`: Type of activity (enum)
- `EntityId`: Related entity ID
- `EntityType`: Type of entity (Route, Collection, etc.)
- `Metadata`: Additional JSON metadata
- `IsVisible`: Whether visible in feeds

#### Route Updates
Added sharing-related fields to Route entity:
- `ShareToken`: Unique token for sharing (12 characters)
- `IsEmbeddable`: Whether route can be embedded
- `ShareCount`: Number of times shared

#### User Updates
Added social fields to User entity:
- `Bio`: User biography
- `AvatarUrl`: Profile picture URL
- `FollowerCount`: Number of followers
- `FollowingCount`: Number of users following

### API Endpoints

#### Route Sharing (`/api/routes/sharing`)

**POST /api/routes/sharing/{routeId}/generate-link**
- Generates a shareable link for a route
- Returns: ShareToken, ShareUrl, EmbedCode
- Authorization: Required (must be creator or route must be public)

**GET /api/routes/sharing/token/{shareToken}**
- Gets route metadata by share token
- Returns: RouteSocialMetadata (for Open Graph/Twitter cards)
- Authorization: None (public)
- Increments view count

#### Social Features (`/api/social`)

**POST /api/social/follow**
- Follow a user
- Request: `{ userId: string }`
- Returns: UserFollowResponse
- Updates follower/following counts
- Creates activity

**POST /api/social/unfollow**
- Unfollow a user
- Request: `{ userId: string }`
- Updates follower/following counts

**GET /api/social/users/{userId}/followers**
- Get user's followers
- Query params: pageNumber, pageSize
- Returns: Paginated list of UserListItem
- Shows IsFollowing status relative to current user

**GET /api/social/users/{userId}/following**
- Get users that user is following
- Query params: pageNumber, pageSize
- Returns: Paginated list of UserListItem

#### Collections (`/api/collections`)

**POST /api/collections**
- Create a new collection
- Request: CreateCollectionRequest
- Returns: CollectionResponse
- Creates activity

**POST /api/collections/{collectionId}/routes**
- Add a route to a collection
- Request: `{ routeId: string, note?: string }`
- Authorization: Must be collection owner
- Creates activity

**GET /api/collections/user/{userId}**
- Get user's collections
- Query params: pageNumber, pageSize
- Returns: Paginated CollectionResponse
- Shows only public collections if not owner

**GET /api/collections/{id}**
- Get collection details with all routes
- Returns: CollectionDetailResponse with items
- Authorization: Must be public or owner

#### Activity Feed (`/api/activities`)

**GET /api/activities/feed**
- Get authenticated user's activity feed
- Shows activities from followed users
- Query params: followedOnly, activityType, pageNumber, pageSize
- Returns: Paginated ActivityResponse
- Authorization: Required

**GET /api/activities/user/{userId}**
- Get activities for a specific user
- Query params: activityType, pageNumber, pageSize
- Returns: Paginated ActivityResponse

**GET /api/activities/global**
- Get global activity feed (all public activities)
- Query params: activityType, pageNumber, pageSize
- Returns: Paginated ActivityResponse

### Activity Types

The following activity types are tracked:
- `RouteCreated`: User created a new route
- `RouteUpdated`: User updated a route
- `RouteFavorited`: User favorited a route
- `ReviewCreated`: User created a review
- `CollectionCreated`: User created a collection
- `RouteAddedToCollection`: User added route to collection
- `UserFollowed`: User started following another user

## Frontend Implementation

### Services

#### sharingService
- `generateShareLink(routeId)`: Generate share link
- `getRouteByShareToken(shareToken)`: Get route metadata
- `copyShareLink(shareUrl)`: Copy link to clipboard
- `shareOnSocialMedia(platform, shareUrl, title)`: Share on social platforms
  - Supports: Facebook, Twitter, LinkedIn

#### socialService
- `followUser(userId)`: Follow a user
- `unfollowUser(userId)`: Unfollow a user
- `getFollowers(userId, page, pageSize)`: Get followers
- `getFollowing(userId, page, pageSize)`: Get following

#### collectionService
- `createCollection(data)`: Create new collection
- `addRouteToCollection(collectionId, data)`: Add route
- `getUserCollections(userId, page, pageSize)`: Get collections
- `getCollectionById(id)`: Get collection details

#### activityService
- `getActivityFeed(params)`: Get authenticated user's feed
- `getUserActivities(userId, params)`: Get user's activities
- `getGlobalFeed(params)`: Get global feed

### Components

#### ShareDialog
Modal dialog for sharing routes with:
- Shareable link with copy button
- Social media buttons (Facebook, Twitter, LinkedIn)
- Embed code with copy button
- Visual feedback (Snackbar notifications)

**Usage:**
```tsx
<ShareDialog
  open={open}
  onClose={() => setOpen(false)}
  routeId="route-id"
  routeName="My Route"
/>
```

#### ActivityFeed
Displays activity feed with:
- User avatars and names
- Activity descriptions with icons
- Activity type chips
- Relative timestamps (e.g., "2 hours ago")
- Pagination support

**Usage:**
```tsx
<ActivityFeed
  userId="user-id"        // Optional: show specific user's activities
  followedOnly={true}     // Show only followed users
  showGlobal={false}      // Show global feed
/>
```

#### FollowButton
Toggle follow/unfollow with:
- Loading states
- Icon changes based on state
- Configurable size and variant
- Callback for state changes

**Usage:**
```tsx
<FollowButton
  userId="user-id"
  isFollowing={false}
  onFollowChange={(isFollowing) => console.log(isFollowing)}
  size="small"
  variant="outlined"
/>
```

#### AddToCollectionDialog
Add routes to collections with:
- List of existing collections
- Create new collection inline
- Loading and error states

**Usage:**
```tsx
<AddToCollectionDialog
  open={open}
  onClose={() => setOpen(false)}
  routeId="route-id"
  routeName="My Route"
/>
```

## Database Schema

### UserFollows Table
```sql
- Id (PK)
- FollowerId (FK -> Users)
- FollowingId (FK -> Users)
- IsNotified
- CreatedAt
- ModifiedAt
- Indexes: (FollowerId, FollowingId) UNIQUE
```

### RouteCollections Table
```sql
- Id (PK)
- Name
- Description
- UserId (FK -> Users)
- IsPublic
- CoverImageUrl
- CreatedAt
- ModifiedAt
- Indexes: UserId, IsPublic, Name
```

### RouteCollectionItems Table
```sql
- Id (PK)
- CollectionId (FK -> RouteCollections)
- RouteId (FK -> Routes)
- Order
- Note
- CreatedAt
- Indexes: (CollectionId, RouteId) UNIQUE
```

### UserActivities Table
```sql
- Id (PK)
- UserId (FK -> Users)
- ActivityType (enum)
- EntityId
- EntityType
- Metadata (JSON)
- IsVisible
- CreatedAt
- Indexes: UserId, ActivityType, CreatedAt, (UserId, CreatedAt)
```

### Routes Table Updates
```sql
- ShareToken (UNIQUE, indexed)
- IsEmbeddable
- ShareCount
```

### Users Table Updates
```sql
- Bio
- AvatarUrl
- FollowerCount
- FollowingCount
```

## Security Considerations

1. **Authorization Checks**
   - Share links require route to be public or user to be creator
   - Collection modifications require ownership
   - Private collections only visible to owner
   - Activity visibility controlled by IsVisible flag

2. **Rate Limiting**
   - Consider implementing rate limits on:
     - Share link generation
     - Follow/unfollow actions
     - Activity feed queries

3. **Data Privacy**
   - Users can control collection visibility (public/private)
   - Activity feeds respect privacy settings
   - Share tokens are random and non-guessable

## Integration Examples

### Adding Share Button to Route Detail Page

```tsx
import { useState } from 'react';
import { Button } from '@mui/material';
import { Share as ShareIcon } from '@mui/icons-material';
import ShareDialog from '../components/sharing/ShareDialog';

function RouteDetailPage({ route }) {
  const [shareOpen, setShareOpen] = useState(false);

  return (
    <>
      <Button
        startIcon={<ShareIcon />}
        onClick={() => setShareOpen(true)}
      >
        Share
      </Button>
      
      <ShareDialog
        open={shareOpen}
        onClose={() => setShareOpen(false)}
        routeId={route.id}
        routeName={route.name}
      />
    </>
  );
}
```

### Adding Activity Feed to Dashboard

```tsx
import ActivityFeed from '../components/activities/ActivityFeed';

function Dashboard() {
  return (
    <Box>
      <Typography variant="h5">Activity Feed</Typography>
      <ActivityFeed followedOnly={true} />
    </Box>
  );
}
```

### Adding Follow Button to User Profile

```tsx
import FollowButton from '../components/social/FollowButton';

function UserProfile({ user, currentUser }) {
  return (
    <Box>
      <Typography variant="h4">{user.fullName}</Typography>
      {user.id !== currentUser.id && (
        <FollowButton
          userId={user.id}
          isFollowing={user.isFollowing}
        />
      )}
    </Box>
  );
}
```

## Future Enhancements

1. **Notifications**
   - Notify users when followed
   - Notify when route is added to collection
   - Notify on new activity from followed users

2. **Enhanced Sharing**
   - QR codes for routes
   - WhatsApp sharing
   - Email sharing

3. **Collection Features**
   - Reorder routes in collection
   - Remove routes from collection
   - Update collection metadata
   - Collection sharing

4. **Activity Feed Enhancements**
   - Filter by activity type
   - Search activities
   - Activity notifications

5. **Social Features**
   - User recommendations
   - Popular routes from followed users
   - Trending routes

6. **Analytics**
   - Track share metrics
   - Popular sharing platforms
   - Collection engagement
   - Activity feed engagement

## Testing

### Manual Testing Checklist

- [ ] Generate share link for public route
- [ ] Generate share link for private route (as creator)
- [ ] Copy share link to clipboard
- [ ] Share on Facebook, Twitter, LinkedIn
- [ ] View embedded route
- [ ] Follow a user
- [ ] Unfollow a user
- [ ] View followers list
- [ ] View following list
- [ ] Create a collection
- [ ] Add route to collection
- [ ] View collection details
- [ ] View activity feed (followed only)
- [ ] View user's activities
- [ ] View global activity feed
- [ ] Test pagination on all lists

### API Testing

Use tools like Postman or curl to test API endpoints:

```bash
# Generate share link
curl -X POST http://localhost:5139/api/routes/sharing/{routeId}/generate-link \
  -H "Authorization: Bearer {token}"

# Follow user
curl -X POST http://localhost:5139/api/social/follow \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"userId": "user-id"}'

# Get activity feed
curl http://localhost:5139/api/activities/feed?followedOnly=true&pageNumber=1 \
  -H "Authorization: Bearer {token}"
```

## Troubleshooting

### Common Issues

1. **Share link not generating**
   - Check if user has permission to share route
   - Verify route exists and is active
   - Check database connectivity

2. **Follow button not working**
   - Verify user is authenticated
   - Check if trying to follow self
   - Verify target user exists

3. **Activity feed empty**
   - Check if user is following anyone
   - Verify activities exist in database
   - Check activity visibility settings

4. **Collection not showing**
   - Verify collection is public or user is owner
   - Check if collection has items
   - Verify query parameters

## Conclusion

This implementation provides a comprehensive social and sharing platform for Tripilot routes. All acceptance criteria from the original issue have been met:

✅ Create shareable route links with previews
✅ Implement social media sharing integration
✅ Add route embedding for external websites
✅ Create user following and follower system
✅ Implement activity feeds for followed users
✅ Add route collections and lists

The system is modular, scalable, and follows best practices for both backend (Clean Architecture, CQRS) and frontend (React, TypeScript, Material-UI) development.
