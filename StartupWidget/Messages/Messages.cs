using System;
using System.Collections.Generic;
using System.Text;

namespace StartupWidget.Messages
{
    public record QuitAppMessage();
    public record HideMainWindow();

    public record HideWidgetWindow();
    public record ShowWidgetWindow();


    public record OpenAddWindow();
    public record CloseAddWindow();

    public record OpenSettingsWindow();
    public record CloseSettingsWindow();
    public record ApplySettings();

    public record OpenEditWindow(string? AddingItemPath = null);
}
