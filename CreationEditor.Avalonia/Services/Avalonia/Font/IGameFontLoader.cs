namespace CreationEditor.Avalonia.Services.Avalonia.Font;

/// <summary>
/// Sets up and provides game fonts to be used in Avalonia.
/// </summary>
public interface IGameFontLoader {
    /// <summary>
    /// Get the full name of a font as needed by Avalonia by its base id.
    /// </summary>
    /// <param name="fontId">Font identifier</param>
    /// <returns>Name of the font for reference in Avalonia</returns>
    string GetFontName(string fontId);
}
