﻿using Mutagen.Bethesda.Plugins.Assets;
namespace ModCleaner.Models;

public record AssetLinkIdentifier(IAssetLinkGetter AssetLink) : ILinkIdentifier;
