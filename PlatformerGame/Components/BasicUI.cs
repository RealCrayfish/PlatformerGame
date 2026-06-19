using Nez;
using Nez.UI;
using System;

/// <summary>
/// BasicUI class that provides a simple way to create UI elements
/// </summary>
public class BasicUI
{
    protected UICanvas uiCanvas;
    protected Table _table;
    protected Window _window;

    public bool IsVisible => _table.GetChildren().Contains(_window);

    /// <summary>
    /// Creates a new BasicUI object with a title and font scale
    /// </summary>
    /// <param name="uiCanvas"></param>
    /// <param name="title"></param>
    /// <param name="fontScale"></param>
    public BasicUI(UICanvas uiCanvas, string title, float fontScale)
    {
        this.uiCanvas = uiCanvas;
        _table = uiCanvas.Stage.AddElement(new Table());
        _table.SetFillParent(true).Center();
        _window = new Window("", Skin.CreateDefaultSkin());
        _window.SetMovable(false);
        _window.Center();
        _window.Pad(20);
        _window.Add(new Label(title).SetFontScale(fontScale));
        _window.Row().SetPadTop(20);
    }

    /// <summary>
    /// Adds a button to the window with the specified text and onClickAction
    /// </summary>
    /// <param name="buttonText"></param>
    /// <param name="onClickAction"></param>
    /// <returns></returns>
    public Button AddButton(string buttonText, Action<Button> onClickAction)
    {
        var button = _window.Add(new TextButton(buttonText, Skin.CreateDefaultSkin())).SetFillX().SetMinHeight(30).GetElement<TextButton>();
        button.OnClicked += onClickAction;
        _window.Row().SetPadTop(20);

        return button;
    }

    /// <summary>
    /// Adds a label to the window with the specified text and font scale
    /// </summary>
    /// <param name="text"></param>
    /// <param name="fontScale"></param>
    public void AddLabel(string text, float fontScale)
    {
        _window.Add(new Label(text).SetFontScale(fontScale));
        _window.Row().SetPadTop(20);
    }

    /// <summary>
    /// Adds a text field to the window with the specified text
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public TextField AddTextField(string text)
    {
        var textField = _window.Add(new TextField(text, Skin.CreateDefaultSkin())).SetFillX().SetMinHeight(30).GetElement<TextField>();
        _window.Row().SetPadTop(20);

        return textField;
    }

    /// <summary>
    /// Adds a checkbox to the window with the specified text and isChecked value
    /// </summary>
    /// <param name="text"></param>
    /// <param name="isChecked"></param>
    /// <returns></returns>
    public CheckBox AddCheckbox(string text, bool isChecked)
    {
        var checkBox = _window.Add(new CheckBox(text, Skin.CreateDefaultSkin())).SetFillX().SetMinHeight(30).GetElement<CheckBox>();
        checkBox.IsChecked = isChecked;
        _window.Row().SetPadTop(20);
        return checkBox;
    }

    /// <summary>
    /// Displays the window
    /// </summary>
    public virtual void Display()
    {
        _table.Add(_window);
        _table.ToFront();
    }

    /// <summary>
    /// Hides the window
    /// </summary>
    public virtual void Hide()
    {
        _table.RemoveElement(_window);
    }
}