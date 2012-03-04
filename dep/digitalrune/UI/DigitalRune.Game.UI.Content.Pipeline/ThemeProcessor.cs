using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using DigitalRune.Collections;
using DigitalRune.Game.UI.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;


namespace DigitalRune.Game.UI.Content.Pipeline
{
  /// <summary>
  /// Builds a UI theme including cursors, textures, fonts, etc.
  /// </summary>
  [ContentProcessor(DisplayName = "UI Theme - DigitalRune")]
  public class ThemeProcessor : ContentProcessor<ThemeContent, ThemeContent>
  {
    /// <summary>
    /// Processes the specified input data and returns the result.
    /// </summary>
    /// <param name="theme">Existing content object being processed.</param>
    /// <param name="context">Contains any required custom process parameters.</param>
    /// <returns>A typed object representing the processed input.</returns>
    public override ThemeContent Process(ThemeContent theme, ContentProcessorContext context)
    {
      ProcessCursors(theme, context);
      ProcessFonts(theme, context);
      ProcessTextures(theme, context);
      ProcessStyles(theme, context);
      return theme;
    }


    private void ProcessCursors(ThemeContent theme, ContentProcessorContext context)
    {
      theme.Cursors = new NamedObjectCollection<ThemeCursorContent>();

      // Hardware cursors are only supported on Windows.
      if (context.TargetPlatform != TargetPlatform.Windows)
        return;

      var document = theme.Description;
      var cursorsElement = document.Root.Element("Cursors");
      if (cursorsElement == null)
        return;

      foreach (var cursorElement in cursorsElement.Elements("Cursor"))
      {
        string name = GetMandatoryAttribute(theme, cursorElement, "Name");
        bool isDefault = (bool?)cursorElement.Attribute("IsDefault") ?? false;
        string filename = GetMandatoryAttribute(theme, cursorElement, "File");

        // Find cursor file.
        string sourceName = FindFile(theme, filename);
        context.AddDependency(sourceName);

        // Copy cursor file to output directory.
        filename = Path.GetFileName(sourceName);
        string outputName = Path.Combine(context.OutputDirectory, filename);
        File.Copy(sourceName, outputName, true);
        context.AddOutputFile(outputName);

        var cursor = new ThemeCursorContent
        {
          Name = name,
          IsDefault = isDefault,
          FileName = filename,
        };

        theme.Cursors.Add(cursor);
      }
    }


    private void ProcessFonts(ThemeContent theme, ContentProcessorContext context)
    {
      theme.Fonts = new NamedObjectCollection<ThemeFontContent>();

      var document = theme.Description;
      var fontsElement = document.Root.Element("Fonts");
      if (fontsElement == null)
        throw new InvalidContentException("The given UI theme does not contain a 'Fonts' node.", theme.Identity);

      foreach (var fontElement in fontsElement.Elements("Font"))
      {
        string name = GetMandatoryAttribute(theme, fontElement, "Name");
        bool isDefault = (bool?)fontElement.Attribute("IsDefault") ?? false;
        string filename = GetMandatoryAttribute(theme, fontElement, "File");

        // Build SpriteFont.
        // The input can be an XML file (.spritefont font description file) or a texture.
        filename = FindFile(theme, filename);
        bool isXml = IsXmlFile(filename);
        ExternalReference<SpriteFontContent> spriteFont;
        if (isXml)
        {
          // Build sprite font from font description file.
          spriteFont = context.BuildAsset<FontDescription, SpriteFontContent>(
            new ExternalReference<FontDescription>(filename),
            "FontDescriptionProcessor",
            null,
            "FontDescriptionImporter",
            name);
        }
        else
        {
          // Build sprite font from texture file.
          spriteFont = context.BuildAsset<Texture2DContent, SpriteFontContent>(
          new ExternalReference<Texture2DContent>(filename),
          "FontTextureProcessor",
          null,
          "TextureImporter",
          name);
        }

        var font = new ThemeFontContent
        {
          Name = name,
          IsDefault = isDefault,
          Font = spriteFont
        };

        theme.Fonts.Add(font);
      }

      if (theme.Fonts.Count == 0)
        throw new InvalidContentException("The UI theme does not contain any fonts. At least 1 font is required.", theme.Identity);
    }


    /// <summary>
    /// Determines whether the given file is an XML file.
    /// </summary>
    private bool IsXmlFile(string filePath)
    {
      // Open file as text.
      using (var stream = File.OpenText(filePath))
      {
        // Use XmlReader to read an XML node. If the file is not an XML file, the reader throws
        // an exception.
        using (XmlReader reader = XmlReader.Create(stream))
        {
          try
          {
            reader.Read();
            return true;
          }
          catch (XmlException)
          {
            // Not a valid XML file.
            return false;
          }
        }
      }
    }


    private void ProcessTextures(ThemeContent theme, ContentProcessorContext context)
    {
      theme.Textures = new NamedObjectCollection<ThemeTextureContent>();

      var document = theme.Description;

      if (document.Root.Elements("Texture").Any())
      {
        // Issue error because theme file is using old alpha version format.
        throw new InvalidContentException("Given theme file is using a format which is no longer supported. All textures need to be defined inside a 'Textures' node.", theme.Identity);
      }

      var texturesElement = document.Root.Element("Textures");
      if (texturesElement == null)
        throw new InvalidContentException("Given theme file does not contain a 'Textures' node.", theme.Identity);

      foreach (var textureElement in texturesElement.Elements("Texture"))
      {
        string name = GetMandatoryAttribute(theme, textureElement, "Name");
        bool isDefault = (bool?)textureElement.Attribute("IsDefault") ?? false;
        string filename = GetMandatoryAttribute(theme, textureElement, "File");

        // Build Texture.
        filename = FindFile(theme, filename);
        var textureReference = context.BuildAsset<TextureContent, TextureContent>(
          new ExternalReference<TextureContent>(filename),
          "TextureProcessor",
          null,
          "TextureImporter",
          name);

        var texture = new ThemeTextureContent
        {
          Name = name,
          IsDefault = isDefault,
          Texture = textureReference,
        };

        theme.Textures.Add(texture);
      }

      if (theme.Textures.Count == 0)
        throw new InvalidContentException("The UI theme does not contain any textures. At least 1 texture is required.", theme.Identity);
    }


    private void ProcessStyles(ThemeContent theme, ContentProcessorContext context)
    {
      theme.Styles = new NamedObjectCollection<ThemeStyleContent>();

      var document = theme.Description;
      var stylesElement = document.Root.Element("Styles");
      if (stylesElement == null)
        return;

      foreach (var styleElement in stylesElement.Elements("Style"))
      {
        var style = new ThemeStyleContent();
        style.Name = GetMandatoryAttribute(theme, styleElement, "Name");
        style.Inherits = (string)styleElement.Attribute("Inherits");

        foreach (var element in styleElement.Elements())
        {
          if (element.Name == "State")
          {
            try
            {
              var state = new ThemeStateContent
              {
                Name = GetMandatoryAttribute(theme, element, "Name"),
                IsInherited = (bool?)element.Attribute("IsInherited") ?? false,
              };

              foreach (var imageElement in element.Elements("Image"))
              {
                var image = new ThemeImageContent
                {
                  Name = (string)imageElement.Attribute("Name"),
                  Texture = (string)imageElement.Attribute("Texture"),
                  SourceRectangle = ThemeHelper.ParseRectangle((string)imageElement.Attribute("Source")),
                  Margin = ThemeHelper.ParseVector4F((string)imageElement.Attribute("Margin")),
                  HorizontalAlignment = ThemeHelper.ParseHorizontalAlignment((string)imageElement.Attribute("HorizontalAlignment")),
                  VerticalAlignment = ThemeHelper.ParseVerticalAlignment((string)imageElement.Attribute("VerticalAlignment")),
                  TileMode = ThemeHelper.ParseTileMode((string)imageElement.Attribute("TileMode")),
                  Border = ThemeHelper.ParseVector4F((string)imageElement.Attribute("Border")),
                  IsOverlay = (bool?)imageElement.Attribute("IsOverlay") ?? false,
                  Color = ThemeHelper.ParseColor((string)imageElement.Attribute("Color"), Color.White),
                };

                if (!string.IsNullOrEmpty(image.Texture) && !theme.Textures.Contains(image.Texture))
                {
                  string message = string.Format("Missing texture: The image '{0}' in state '{1}' of style '{2}' requires a texture named '{3}'.", image.Name, state.Name, style.Name, image.Texture);
                  throw new InvalidContentException(message, theme.Identity);
                }

                state.Images.Add(image);
              }

              var backgroundElement = element.Element("Background");
              if (backgroundElement != null)
                state.Background = ThemeHelper.ParseColor((string)backgroundElement, Color.Transparent);

              var foregroundElement = element.Element("Foreground");
              if (foregroundElement != null)
                state.Foreground = ThemeHelper.ParseColor((string)foregroundElement, Color.Black);

              state.Opacity = (float?)element.Element("Opacity");

              style.States.Add(state);
            }
            catch (Exception exception)
            {
              string message = string.Format("Could not load state '{0}' of style '{1}'.", element.Attribute("Name"), style.Name);
              throw new InvalidContentException(message, theme.Identity, exception);
            }
          }
          else
          {
            // A custom attribute.
            var attribute = new ThemeAttributeContent
            {
              Name = element.Name.ToString(),
              Value = element.Value,
            };
            style.Attributes.Add(attribute);
          }
        }

        theme.Styles.Add(style);
      }

      // Validate inheritance.
      foreach (var style in theme.Styles)
      {
        if (string.IsNullOrEmpty(style.Inherits))
          continue;

        if (!theme.Styles.Contains(style.Inherits))
        {
          // Parent of the given style not found. Log warning.
          context.Logger.LogWarning(
            null,
            theme.Identity,
            "The parent of style \"{0}\" (Inherits = \"{1}\") not found.",
            style.Name,
            style.Inherits);
        }
      }
    }


    private string GetMandatoryAttribute(ThemeContent theme, XElement fontElement, string name)
    {
      var attribute = fontElement.Attribute(name);
      if (attribute == null)
      {
        var lineInfo = (IXmlLineInfo)fontElement;
        if (lineInfo.HasLineInfo())
        {
          string message = string.Format(
            "\"{0}\" attribute is missing. (Element: \"{1}\", Line: {2})", name, fontElement.Name, lineInfo.LineNumber);
          throw new InvalidContentException(message, theme.Identity);
        }
        else
        {
          string message = string.Format("\"{0}\" attribute is missing. (Element: \"{1}\")", name, fontElement.Name);
          throw new InvalidContentException(message, theme.Identity);
        }
      }

      string s = (string)attribute;
      if (s.Length == 0)
      {
        var lineInfo = (IXmlLineInfo)fontElement;
        if (lineInfo.HasLineInfo())
        {
          string message = string.Format("\"{0}\" attribute must not be empty. (Element: \"{1}\", Line: {2})", name, fontElement.Name, lineInfo.LineNumber);
          throw new InvalidContentException(message, theme.Identity);
        }
        else
        {
          string message = string.Format("\"{0}\" attribute must not be empty. (Element: \"{1}\")", name, fontElement.Name);
          throw new InvalidContentException(message, theme.Identity);
        }
      }

      return s;
    }


    private static string FindFile(ThemeContent theme, string filename)
    {
      // Check whether 'filename' is a valid path.
      if (File.Exists(filename))
        return Path.GetFullPath(filename);

      // Perhaps 'filename' contains a relative path.
      string folder = Path.GetDirectoryName(theme.Identity.SourceFilename) ?? string.Empty;
      string relativeFilename = Path.Combine(folder, filename);
      if (File.Exists(relativeFilename))
        return Path.GetFullPath(relativeFilename);

      string message = string.Format("File \"{0}\" not found.", filename);
      throw new InvalidContentException(message, theme.Identity);
    }
  }
}