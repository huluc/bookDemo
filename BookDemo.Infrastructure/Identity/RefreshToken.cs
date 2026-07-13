using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Identity
{

    // Represents a single refresh token tied to one login session/device.
    // MULTI-DEVICE SUPPORT: a user can have MANY active RefreshToken rows at
    // once (one per device), because logging in on a new device never revokes
    // tokens issued elsewhere. This mirrors how Google/GitHub/Microsoft work —
    // signing in on your phone doesn't log you out of your laptop.
    public class RefreshToken
    {
        public Guid Id { get; set; }

        // We NEVER store the raw token — only its SHA256 hash. Same principle
        // as password hashing: if the database is ever compromised, stored
        // hashes are useless to an attacker without the original token.
        public string TokenHash { get; set; } = default!;

        // One user -> many tokens (one per device). Not a one-to-one relationship.
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        // Soft-revocation (flag) instead of deleting the row — keeping revoked
        // tokens around preserves an audit trail and enables reuse-detection
        // (see ReplacedByTokenHash below).
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }

        // When this token is rotated (used once to get a new access token), we
        // record which NEW token replaced it. This builds a traceable chain:
        // if someone ever presents an already-rotated token again, we know
        // it's a replay/theft attempt, not just an expired token.
        public string? ReplacedByTokenHash { get; set; }

        // Computed, not persisted — see RefreshTokenConfiguration.Ignore(...).
        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
    }
}
