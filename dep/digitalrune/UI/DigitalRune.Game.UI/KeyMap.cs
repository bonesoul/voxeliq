using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using DigitalRune.Game.Input;
using Microsoft.Xna.Framework.Input;


namespace DigitalRune.Game.UI
{
  /// <summary>
  /// Maps XNA <see cref="Keys"/> (key codes) to characters (keyboard layout).
  /// </summary>
  public class KeyMap
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private readonly Dictionary<Keys, Dictionary<ModifierKeys, char>> _map = new Dictionary<Keys, Dictionary<ModifierKeys, char>>();
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets or sets the <see cref="System.Char"/> with the specified key and modifiers.
    /// </summary>
    /// <remarks>
    /// If an entry is set that does not yet exist, the entry is added to the map.
    /// If an entry is fetched that does not exist, 0 is returned.
    /// </remarks>
    public char this[Keys key, ModifierKeys modifierKeys]
    {
      get
      {
        // Get variants (different modifiers results) for the specified key.
        Dictionary<ModifierKeys, char> keyVariants;
        bool exists = _map.TryGetValue(key, out keyVariants);
        if (!exists)
          return '\0';

        // Get character for the given modifier.
        char c;
        exists = keyVariants.TryGetValue(modifierKeys, out c);
        if (!exists)
          return '\0';

        return c;
      }
      set
      {
        Dictionary<ModifierKeys, char> keyVariants;
        bool exists = _map.TryGetValue(key, out keyVariants);
        if (!exists)
        {
          // No entries for key exist.
          // Add new dictionary for the key and its variants.
          keyVariants = new Dictionary<ModifierKeys, char>();
          _map.Add(key, keyVariants);
        }

        // Add new key variant or change existing one.
        exists = keyVariants.ContainsKey(modifierKeys);
        if (!exists)
          keyVariants.Add(modifierKeys, value);
        else
          keyVariants[modifierKeys] = value;
      }
    }


    /// <summary>
    /// Gets the an automatic key mapping for the current culture.
    /// </summary>
    /// <value>An automatic key mapping.</value>
    /// <remarks>
    /// <para>
    /// For Windows: Windows OS functions are used to get key mapping for the active keyboard 
    /// layout. 
    /// </para>
    /// <para>
    /// For Xbox 360: An English key mapping is used for USB keyboards. For the ChatPad an
    /// English or German key mapping is used, based on the current culture.
    /// </para>
    /// </remarks>
    public static KeyMap AutoKeyMap
    {
      get
      {
        if (_autoKeyMap != null)
          return _autoKeyMap;

        // Check if CurrentCulture is German.
        string cultureName = CultureInfo.CurrentCulture.EnglishName;
        bool isGerman = cultureName.Contains("German");

#if WINDOWS
        // ----- Automatically generate key map. 
        // Start with one of the predefined maps.
        _autoKeyMap = new KeyMap(isGerman ? GermanKeyMap : EnglishKeyMap);

        // Get keyboard layout of current thread.
        IntPtr layout = GetKeyboardLayout(0);

        // We need an array where the index is a virtual-key code.
        // Relevant entries are 0x10 - 0x12 (Shift, Control and Alt).
        byte[] keyStates = new byte[256];

        // Get the character for all virtual-keys and modifier key combos.
        foreach (Keys key in Enum.GetValues(typeof(Keys)))
        {
          // The XNA Key enumeration contains virtual-key codes.
          uint virtualKeyCode = (uint)key;

          // Get scancode.
          uint scanCode = MapVirtualKeyEx(virtualKeyCode, 0, layout);
          char c;

          keyStates[0x10] = 0x00;  // No Shift.
          keyStates[0x11] = 0x00;  // No Control.
          keyStates[0x12] = 0x00;  // No Alt.
          int result = ToUnicodeEx((uint)key, scanCode, keyStates, out c, 1, 0, layout);
          if (result == 1)
            _autoKeyMap[key, ModifierKeys.None] = c;

          keyStates[0x10] = 0x80;  // Only Shift pressed.
          keyStates[0x11] = 0x00;
          keyStates[0x12] = 0x00;
          result = ToUnicodeEx((uint)key, scanCode, keyStates, out c, 1, 0, layout);
          if (result == 1)
            _autoKeyMap[key, ModifierKeys.Shift] = c;

          keyStates[0x10] = 0x00;
          keyStates[0x11] = 0x80;  // Control pressed.
          keyStates[0x12] = 0x80;  // Alt pressed.
          result = ToUnicodeEx((uint)key, scanCode, keyStates, out c, 1, 0, layout);
          if (result == 1)
            _autoKeyMap[key, ModifierKeys.ControlAlt] = c;
        }
        return _autoKeyMap;
#else
        // Use predefined key map.
        if (isGerman)
          _autoKeyMap = EnglishKeyMapGermanChatPad;
        else
          _autoKeyMap = EnglishKeyMap;
        return _autoKeyMap;
#endif
      }
    }
    private static KeyMap _autoKeyMap;


    /// <summary>
    /// Gets the German key mapping.
    /// </summary>
    /// <value>The German key mapping.</value>
    public static KeyMap GermanKeyMap
    {
      get
      {
        if (_germanKeyMap == null)
        {
          _germanKeyMap = new KeyMap();
          _germanKeyMap.LoadEmbeddedResource("DigitalRune.Game.UI.Resources.GermanKeyMap.xml");
        }
        return _germanKeyMap;
      }
    }
    private static KeyMap _germanKeyMap;


    /// <summary>
    /// Gets the English key mapping.
    /// </summary>
    /// <value>The English key mapping.</value>
    public static KeyMap EnglishKeyMap
    {
      get
      {
        if (_englishKeyMap == null)
        {
          _englishKeyMap = new KeyMap();
          _englishKeyMap.LoadEmbeddedResource("DigitalRune.Game.UI.Resources.EnglishKeyMap.xml");
        }
        return _englishKeyMap;
      }
    }
    private static KeyMap _englishKeyMap;


    /// <summary>
    /// Gets the English key mapping, but with a German key mapping for the ChatPad.
    /// </summary>
    /// <value>The English key mapping, but with a German key mapping for the ChatPad.</value>
    public static KeyMap EnglishKeyMapGermanChatPad
    {
      get
      {
        if (_englishKeyMapGermanChatPad == null)
        {
          _englishKeyMapGermanChatPad = new KeyMap();
          _englishKeyMapGermanChatPad.LoadEmbeddedResource("DigitalRune.Game.UI.Resources.EnglishKeyMapGermanChatPad.xml");
        }
        return _englishKeyMapGermanChatPad;
      }
    }
    private static KeyMap _englishKeyMapGermanChatPad;
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <overloads>
    /// <summary>
    /// Initializes a new empty instance of the <see cref="KeyMap"/> class.
    /// </summary>
    /// </overloads>
    /// <summary>
    /// Initializes a new empty instance of the <see cref="KeyMap"/> class.
    /// </summary>
    public KeyMap()
    {
    }


    /// <summary>
    /// Initializes a instance of the <see cref="KeyMap"/> class with the entries
    /// from another <see cref="KeyMap"/>.
    /// </summary>
    /// <param name="source">The source map that will be cloned.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    public KeyMap(KeyMap source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      // Clone the source KeyMap.
      foreach (KeyValuePair<Keys, Dictionary<ModifierKeys, char>> entry in source._map)
      {
        var sourceKey = entry.Key;
        var sourceKeyVariants = entry.Value;

        var keyVariants = new Dictionary<ModifierKeys, char>();
        foreach (var keyVariant in sourceKeyVariants)
          keyVariants.Add(keyVariant.Key, keyVariant.Value);

        _map.Add(sourceKey, keyVariants);
      }
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    private void LoadEmbeddedResource(string keyMapName)
    {
      // Get current assembly.
      var assembly = Assembly.GetExecutingAssembly();
      Stream stream = assembly.GetManifestResourceStream(keyMapName);
      if (stream != null)
      {
        using (XmlReader reader = XmlReader.Create(stream))
          Load(XDocument.Load(reader));

        stream.Close();
      }
    }


    /// <summary>
    /// Loads the key map entries.
    /// </summary>
    /// <param name="xDocument">The XML document containing the key map entries.</param>
    public void Load(XContainer xDocument)
    {
      _map.Clear();

      if (xDocument == null)
        return;

      var root = xDocument.Descendants("KeyMap").FirstOrDefault();
      if (root == null)
        return;

      foreach (var entry in root.Elements("KeyEntry"))
      {
        XAttribute keyAttribute = entry.Attribute("Key");
        Keys key;
        bool success = EnumHelper.TryParse((string)keyAttribute, true, out key);
        if (!success)
          continue;

        XAttribute modifiersAttribute = entry.Attribute("Modifiers");
        ModifierKeys modifierKeys;
        success = EnumHelper.TryParse((string)modifiersAttribute, true, out modifierKeys);
        if (!success)
          modifierKeys = ModifierKeys.None;

        XAttribute charAttribute = entry.Attribute("Character");
        string c = (string)charAttribute;
        if (c == null || c.Length != 1)
          continue;

        this[key, modifierKeys] = c[0];
      }
    }


    /// <summary>
    /// Saves the key map entries.
    /// </summary>
    /// <returns>
    /// An XML document with the serialized key map entries.
    /// </returns>
    public XDocument Save()
    {
      XDocument xDocument = new XDocument();
      var root = new XElement("KeyMap");
      xDocument.Add(root);

      foreach (KeyValuePair<Keys, Dictionary<ModifierKeys, char>> keyVariants in _map)
      {
        foreach (KeyValuePair<ModifierKeys, char> keyVariant in keyVariants.Value)
        {
          root.Add(new XElement(
            "KeyEntry",
            new XAttribute("Key", keyVariants.Key),
            new XAttribute("Modifiers", keyVariant.Key),
            new XAttribute("Character", keyVariant.Value)));
        }
      }
      return xDocument;
    }


    //--------------------------------------------------------------
    #region PInvoke
    //--------------------------------------------------------------

#if WINDOWS
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, out char pwszBuff, int cchBuff, uint wFlags, IntPtr layout);

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr layout);

    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint thread);
#endif
    #endregion

    #endregion
  }
}
