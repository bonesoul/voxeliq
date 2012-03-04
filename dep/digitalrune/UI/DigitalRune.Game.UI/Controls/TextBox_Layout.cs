using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DigitalRune.Mathematics;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework.Graphics;

#if WINDOWS_PHONE || XBOX
using DigitalRune.Text;
#endif


namespace DigitalRune.Game.UI.Controls
{
  partial class TextBox
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets the offset which is the horizontal offset for single-line text boxes or the vertical
    /// offset for multi-line text boxes (see also <see cref="MaxLines"/> and 
    /// <see cref="IsMultiline"/>).
    /// </summary>
    /// <value>
    /// The offset which is the horizontal offset for single-line text boxes or the vertical offset
    /// for multi-line text boxes (see also <see cref="MaxLines"/> and <see cref="IsMultiline"/>).
    /// </value>
    public float VisualOffset { get; private set; }


    /// <summary>
    /// Gets the text exactly as it should be displayed (wrapping already applied).
    /// </summary>
    /// <value>The text, exactly as it should be displayed (wrapping already applied).</value>
    public StringBuilder VisualText { get; private set; }


    /// <summary>
    /// Gets the position of the top left corner of the caret rectangle/line.
    /// </summary>
    /// <value>The position of the top left corner of the caret rectangle/line.</value>
    public Vector2F VisualCaret { get; private set; }


    /// <summary>
    /// Gets the clipping rectangle.
    /// </summary>
    /// <value>
    /// The clipping rectangle.
    /// </value>
    public RectangleF VisualClip { get; private set; }


    /// <summary>
    /// Gets the bounds of the text selection (for rendering).
    /// </summary>
    /// <value>
    /// The bounds of the text selection (for rendering).
    /// </value>
    public List<RectangleF> VisualSelectionBounds { get; private set; }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override Vector2F OnMeasure(Vector2F availableSize)
    {
      // This control can only measure itself if it is in a screen because it needs a font.
      var screen = Screen;
      if (screen == null)
        return base.OnMeasure(availableSize);

      var desiredSize = base.OnMeasure(availableSize);

      // Desired width is not determined by text because a text box should not shrink if text
      // is removed.
      // Desired height depends on the desired number of lines - but not on the actual text.
      if (!Numeric.IsPositiveFinite(Height))
      {
        float lineHeight = screen.Renderer.GetFont(Font).LineSpacing;
        var padding = Padding;
        float minHeight = MinLines * lineHeight + padding.Y + padding.W;
        float maxHeight = MaxLines * lineHeight + padding.Y + padding.W;

        desiredSize.Y = maxHeight;
        if (desiredSize.Y > availableSize.Y)
          desiredSize.Y = availableSize.Y;
        if (desiredSize.Y < minHeight)
          desiredSize.Y = minHeight;
      }

      return desiredSize;
    }


    /// <inheritdoc/>
    protected override void OnArrange(Vector2F position, Vector2F size)
    {
      // This method updates/arranges the scroll bar and computes the VisualText, VisualClip, etc.
      // It does not arrange any other visual children!

      // This control can only arrange itself if it is in a screen because it needs a font.
      var screen = Screen;
      if (screen == null)
      {
        base.OnArrange(position, size);
        return;
      }

      // The code assumes that text is never null but it can be "".
      var text = Text ?? string.Empty;

      if (IsPassword)
      {
        // Replace text with password characters.
        text = new String(PasswordCharacter, text.Length);
      }

      // Coerce caret index.
      if (CaretIndex > text.Length)
        CaretIndex = text.Length;

      // Get content bounds.
      var padding = Padding;
      var textBounds = new RectangleF(position.X, position.Y, size.X, size.Y);
      textBounds.X += padding.X;
      textBounds.Y += padding.Y;
      textBounds.Width -= padding.X + padding.Z;
      textBounds.Height -= padding.Y + padding.W;

      var font = screen.Renderer.GetFont(Font);

      // Find out if the scroll bar is visible.
      if (_verticalScrollBar != null)
      {
        if (IsMultiline)
        {
          // ----- Multi-line
          var scrollBarVisibility = VerticalScrollBarVisibility;
          if (scrollBarVisibility == ScrollBarVisibility.Auto)
          {
            // Scroll bar is visible if the text is too large for the available space.
            float totalTextHeight = font.MeasureString(text).Y;
            if (totalTextHeight > textBounds.Height)
            {
              // Text without wrapping is already too large.
              _verticalScrollBar.IsVisible = true;
            }
            else
            {
              // Text height is ok. But with text wrapping it might need more lines!
              UpdateVisualText(text, textBounds, font);
              totalTextHeight = _lineStarts.Count * font.LineSpacing;
              _verticalScrollBar.IsVisible = (totalTextHeight > textBounds.Height);
            }
          }
          else if (scrollBarVisibility == ScrollBarVisibility.Visible)
          {
            _verticalScrollBar.IsVisible = true;
          }
          else
          {
            _verticalScrollBar.IsVisible = false;
          }
        }
        else
        {
          // ----- Single-line
          _verticalScrollBar.IsVisible = false;
        }
      }

      // If the scroll bar is visible, its size is subtracted from the available text area.
      if (_verticalScrollBar != null && _verticalScrollBar.IsVisible)
      {
        _verticalScrollBar.Measure(size);
        textBounds.Width = (int)(textBounds.Width - _verticalScrollBar.DesiredWidth);
      }

      // Now we know the exact space for the text and can wrap the text.
      UpdateVisualText(text, textBounds, font);

      // Update caret position.
      UpdateVisualCaret(text, textBounds, font);

      // Update text selection bounds.
      UpdateSelectionBounds(text, textBounds, font);

      // Update scrollbar properties and arrange the scrollbar.
      if (_verticalScrollBar != null && _verticalScrollBar.IsVisible)
      {
        float totalTextHeight = _lineStarts.Count * font.LineSpacing;
        _verticalScrollBar.Minimum = 0;
        _verticalScrollBar.Maximum = Math.Max(0, totalTextHeight - textBounds.Height);
        _verticalScrollBar.ViewportSize = textBounds.Height / totalTextHeight;
        _verticalScrollBar.SmallChange = font.LineSpacing;
        _verticalScrollBar.LargeChange = VisualClip.Height;
        _verticalScrollBar.Value = VisualOffset;
        _verticalScrollBar.Measure(size);
        _verticalScrollBar.Arrange(new Vector2F(textBounds.Right, textBounds.Top), new Vector2F(_verticalScrollBar.DesiredWidth, textBounds.Height));
      }

      VisualClip = textBounds;
    }


    /// <summary>
    /// Updates the <see cref="VisualText"/> including text wrapping.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="textBounds">The text bounds.</param>
    /// <param name="font">The font.</param>
    private void UpdateVisualText(string text, RectangleF textBounds, SpriteFont font)
    {
      // Password boxes replace all text with the password character.
      VisualText.Clear();

      WrapText(textBounds.Width, font);

      if (_lineStarts == null || _lineStarts.Count == 0)
      {
        // Single-line: Just add all text. The renderer has to clip the text.
        VisualText.Append(text);
      }
      else
      {
        // Multiline: Add line by line.
        for (int i = 0; i < _lineStarts.Count; i++)
        {
          int start = _lineStarts[i];
          int end = (i + 1 < _lineStarts.Count) ? _lineStarts[i + 1] : -text.Length;

          VisualText.Append(text, Math.Abs(start), Math.Abs(end) - Math.Abs(start));

          // A positive index indicates that a newline character needs to be added
          // for text wrapping.
          if (end > 0)
            VisualText.Append('\n');
        }
      }
    }


    /// <summary>
    /// Computes the <see cref="_lineStarts"/> array that indicates where new lines have to start.
    /// </summary>
    /// <param name="maxWidth">The max width in pixels.</param>
    /// <param name="font">The font.</param>
    private void WrapText(float maxWidth, SpriteFont font)
    {
      if (!IsMultiline)
      {
        // Single-line text box - no wrapping.
        _lineStarts = null;
        return;
      }

      // Possibly more than one line. Compute wrapping indices.
      if (_lineStarts == null)
        _lineStarts = new List<int>();
      else
        _lineStarts.Clear();

      // The first line always starts at index 0.
      _lineStarts.Add(0);

      string text = Text ?? string.Empty;
      StringBuilder line = new StringBuilder();
      int words = 0;
      int i = 0;
      while (i < text.Length)
      {
        // Add word to line until we find a whitespace or newline.
        int wordLength = 0;
        while (i < text.Length && !char.IsWhiteSpace(text[i]) && text[i] != '\n')
        {
          line.Append(text[i]);
          i++;
          wordLength++;
        }

        // Trailing spaces belong to the word because a new line should not start with a space.
        while (i < text.Length && char.IsWhiteSpace(text[i]) && text[i] != '\n')
        {
          line.Append(text[i]);
          i++;
          wordLength++;
        }

        // Ok, we have one more word.
        words++;

        // Check if we have to start a new line.
        float width = font.MeasureString(line).X;
        if (width > maxWidth || (i < text.Length && text[i] == '\n'))
        {
          // We have to start a new line, either because the line is too long or the
          // last character was a newline.
          if (width > maxWidth)
          {
            // Ups, line is too long we have to remove something.
            if (words > 1)
            {
              // There are several words. --> Remove the last word.
              line.Remove(line.Length - wordLength, wordLength);
              i -= wordLength;
            }
            else
            {
              // Only one word - we have to cut the word.
              while (line.Length > 1)
              {
                line.Remove(line.Length - 1, 1);
                i--;

                if (font.MeasureString(line).X <= maxWidth)
                  break;
              }
            }
          }

          // We have the final line.
          words = 0;
          line.Clear();

          if (i < text.Length)
          {
            if (text[i] == '\n')
            {
              // When a line was started by a user-entered newline, we indicate this with a 
              // negative index.
              i++;
              _lineStarts.Add(-i);
            }
            else
            {
              // The new line was started because the line was too long and had to be wrapped.
              _lineStarts.Add(i);
            }
          }
        }
      }
    }


    /// <summary>
    /// Updates the visual caret position.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="textBounds">The text bounds.</param>
    /// <param name="font">The font.</param>
    private void UpdateVisualCaret(string text, RectangleF textBounds, SpriteFont font)
    {
      if (IsReadOnly)
      {
        // Read-only: Hide caret.
        VisualCaret = new Vector2F(float.NaN);
        return;
      }

      // Get visual caret position from CaretIndex.
      var caret = GetPosition(CaretIndex, text, textBounds, font);
      if (_bringCaretIntoView)
      {
        _bringCaretIntoView = false;

        if (!IsMultiline)
        {
          // Single line text box --> Compute horizontal offset in pixels.
          if (caret.X < textBounds.X)
          {
            VisualOffset -= textBounds.X - caret.X;
            caret.X = textBounds.X;
          }
          else if (caret.X > textBounds.Right)
          {
            VisualOffset += caret.X - textBounds.Right + 4;
            caret.X = textBounds.Right - 4;
          }
        }
        else
        {
          // Multi line text box --> Compute vertical offset in pixels.
          if (caret.Y < textBounds.Y)
          {
            VisualOffset -= textBounds.Y - caret.Y;
            caret.Y = textBounds.Y;
          }
          else if (caret.Y + font.LineSpacing > textBounds.Bottom)
          {
            VisualOffset += caret.Y - textBounds.Bottom + font.LineSpacing;
            caret.Y = textBounds.Bottom - font.LineSpacing;
          }
        }
      }

#if WINDOWS_PHONE
      // Hide caret on Windows Phone.
      VisualCaret = new Vector2F(float.NaN);
#else
      VisualCaret = caret;
#endif
    }


    /// <summary>
    /// Updates the text selection bounds.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="textBounds">The text bounds.</param>
    /// <param name="font">The font.</param>
    private void UpdateSelectionBounds(string text, RectangleF textBounds, SpriteFont font)
    {
      VisualSelectionBounds.Clear();

      if (_selectionStart < 0)
        return;

      // Sort selection indices.
      int startIndex, endIndex;
      if (_selectionStart < _caretIndex)
      {
        startIndex = _selectionStart;
        endIndex = _caretIndex;
      }
      else
      {
        startIndex = _caretIndex;
        endIndex = _selectionStart;
      }

      if (startIndex == endIndex)
        return;

      // Determine lines containing start and end of selection.
      int startLine = GetLine(startIndex);
      int endLine = GetLine(endIndex);

      // Determine the screen position of the selection. 
      // (Upper, left corner of start and end index.)
      var selectionStartPosition = GetPosition(startIndex, startLine, text, textBounds, font);
      var selectionEndPosition = GetPosition(endIndex, endLine, text, textBounds, font);

      // The selection bounds contain one RectangleF per line.
      if (startLine == endLine)
      {
        // ----- Single-line selection.
        VisualSelectionBounds.Add(new RectangleF(selectionStartPosition.X, selectionStartPosition.Y, selectionEndPosition.X - selectionStartPosition.X, font.LineSpacing));
      }
      else
      {
        // ----- Multi-line selection.
        int lineStartIndex, lineEndIndex;
        Vector2F lineStartPosition, lineEndPosition;

        // First line.
        lineEndIndex = Math.Abs(_lineStarts[startLine + 1]) - 1;
        lineEndPosition = GetPosition(lineEndIndex, startLine, text, textBounds, font);
        VisualSelectionBounds.Add(new RectangleF(selectionStartPosition.X, selectionStartPosition.Y, lineEndPosition.X - selectionStartPosition.X, font.LineSpacing));

        // Intermediate lines.
        for (int line = startLine + 1; line < endLine; line++)
        {
          lineStartIndex = lineEndIndex + 1;
          lineEndIndex = Math.Abs(_lineStarts[line + 1]) - 1;
          lineStartPosition = GetPosition(lineStartIndex, line, text, textBounds, font);
          lineEndPosition = GetPosition(lineEndIndex, line, text, textBounds, font);
          VisualSelectionBounds.Add(new RectangleF(lineStartPosition.X, lineStartPosition.Y, lineEndPosition.X - lineStartPosition.X, font.LineSpacing));
        }

        // Last line.
        lineStartIndex = lineEndIndex + 1;
        lineStartPosition = GetPosition(lineStartIndex, endLine, text, textBounds, font);
        VisualSelectionBounds.Add(new RectangleF(lineStartPosition.X, lineStartPosition.Y, selectionEndPosition.X - lineStartPosition.X, font.LineSpacing));
      }
    }


#if WINDOWS
    /// <summary>
    /// Gets the nearest index of the caret for a given screen position (e.g. for a mouse click).
    /// </summary>
    /// <param name="position">The absolute screen position.</param>
    /// <param name="screen">The screen.</param>
    /// <returns>The index of the caret.</returns>
    private int GetIndex(Vector2F position, UIScreen screen)
    {
      string text = Text ?? string.Empty;
      StringBuilder buffer = new StringBuilder();
      var font = screen.Renderer.GetFont(Font);

      if (!IsMultiline)
      {
        // ----- Single-line 
        // Measure substrings until we now the number of characters before the position.
        for (int i = 0; i < VisualText.Length; i++)
        {
          buffer.Clear();
          buffer.Append(text, 0, i + 1);

          float x = VisualClip.X - VisualOffset + font.MeasureString(buffer).X;
          if (x > position.X)
            return i;
        }
        return VisualText.Length;
      }

      // ----- Multi-line
      // Find line number.
      float textY = VisualClip.Y - VisualOffset;
      int line = (int)(position.Y - textY) / font.LineSpacing;
      if (line < 0)
        return 0;
      if (line >= _lineStarts.Count)
        return text.Length;

      // Get info for this line.
      int lineStartIndex = Math.Abs(_lineStarts[line]);
      int lineEndIndex = (line + 1 < _lineStarts.Count) ? Math.Abs(_lineStarts[line + 1]) : text.Length;
      int lineLength = lineEndIndex - lineStartIndex;

      // Measure substrings until we know the column.
      for (int i = 0; i < lineLength; i++)
      {
        buffer.Clear();
        buffer.Append(text, lineStartIndex, i + 1);

        float x = VisualClip.X + font.MeasureString(buffer).X;
        if (x > position.X)
          return lineStartIndex + i;
      }

      // If the last character is a newline, then we want to position the caret before
      // the newline, otherwise the caret is displayed on the next line.
      if (lineEndIndex > 0 && text[lineEndIndex - 1] == '\n')
        return lineEndIndex - 1;

      return lineEndIndex;
    }
#endif


    // ----- Not used.
    //private int GetColumn(int index)
    //{
    //  if (!IsMultiline)
    //    return index;

    //  int line = GetLine(index);
    //  return index - Math.Abs(_lineStarts[line]);
    //}


    private int GetLine(int index)
    {
      // Compute the line number of the character index using _lineStarts.
      int line = 0;
      if (_lineStarts != null && _lineStarts.Count > 0)
      {
        for (int i = 1; i < _lineStarts.Count; i++)  // Starting at second line (i = 1)!
        {
          if (Math.Abs(_lineStarts[i]) <= index)
            line++;
          else
            break;
        }
      }

      return line;
    }


    /// <summary>
    /// Gets the position of the given index.
    /// </summary>
    /// <param name="index">The zero-based index of a character in <paramref name="text"/>.</param>
    /// <param name="text">The text.</param>
    /// <param name="textBounds">The text bounds.</param>
    /// <param name="font">The font.</param>
    /// <returns>The upper, left corner of the character</returns>
    private Vector2F GetPosition(int index, string text, RectangleF textBounds, SpriteFont font)
    {
      // Find line number using _lineStarts.
      int line = GetLine(index);

      return GetPosition(index, line, text, textBounds, font);
    }


    // Same as GetPosition() above except that line number is already known. 
    // (Avoids recomputation of line number.)
    private Vector2F GetPosition(int index, int line, string text, RectangleF textBounds, SpriteFont font)
    {
      Debug.Assert(line == GetLine(index), "The line does not contain the given character index.");

      int lineStartIndex = (line == 0) ? 0 : Math.Abs(_lineStarts[line]);
      float y = textBounds.Y + line * font.LineSpacing;

      // Find column using sprite font.
      var textBeforeCaret = text.Substring(lineStartIndex, index - lineStartIndex);
      var x = textBounds.X + font.MeasureString(textBeforeCaret).X;

      // Handle VisualOffset (different interpretation for single-line vs. multi-line).
      if (IsMultiline)
        y -= VisualOffset;
      else
        x -= VisualOffset;

      return new Vector2F(x, y);
    }
    #endregion
  }
}
