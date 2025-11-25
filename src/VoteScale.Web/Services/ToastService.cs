using System;

public class ToastService
{
    public event Action<string, string>? OnShow;

    public void ShowSuccess(string message)
    {
        OnShow?.Invoke(message, "success");
    }

    public void ShowError(string message)
    {
        OnShow?.Invoke(message, "danger");
    }
}