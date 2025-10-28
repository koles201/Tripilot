/**
 * User roles matching backend Tripilot.Domain.Enums.UserRole
 */
export const UserRole = {
  Tourist: 1,
  BusinessOwner: 2,
  Admin: 3,
} as const;

export type UserRoleType = (typeof UserRole)[keyof typeof UserRole];
