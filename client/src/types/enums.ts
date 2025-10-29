/**
 * User roles matching backend Tripilot.Domain.Enums.UserRole
 */
export const UserRole = {
  Tourist: 1,
  BusinessOwner: 2,
  Admin: 3,
} as const;

export type UserRoleType = (typeof UserRole)[keyof typeof UserRole];

/**
 * Place categories matching backend Tripilot.Domain.Enums.PlaceCategory
 */
export const PlaceCategory = {
  Restaurant: 1,
  Museum: 2,
  Entertainment: 3,
  Cafe: 4,
  Hotel: 5,
  Shopping: 6,
  HistoricalSite: 7,
  Park: 8,
  Transportation: 9,
  Other: 99,
} as const;

export type PlaceCategoryType = (typeof PlaceCategory)[keyof typeof PlaceCategory];

/**
 * Helper to get display name for place category
 */
export const PlaceCategoryNames: Record<PlaceCategoryType, string> = {
  [PlaceCategory.Restaurant]: 'Restaurant',
  [PlaceCategory.Museum]: 'Museum',
  [PlaceCategory.Entertainment]: 'Entertainment',
  [PlaceCategory.Cafe]: 'Café',
  [PlaceCategory.Hotel]: 'Hotel',
  [PlaceCategory.Shopping]: 'Shopping',
  [PlaceCategory.HistoricalSite]: 'Historical Site',
  [PlaceCategory.Park]: 'Park',
  [PlaceCategory.Transportation]: 'Transportation',
  [PlaceCategory.Other]: 'Other',
};
