﻿using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Nebx.API.BuildingBlocks.Extensions;

public static class EntityExtensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry is not null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
}