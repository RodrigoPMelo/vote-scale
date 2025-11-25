using System;

public class ToastService
{
    public event Action<string, string>? OnShow;

    public virtual void ShowSuccess(string message)
    {
        OnShow?.Invoke(message, "success");
    }

    public virtual void ShowError(string message)
    {
        OnShow?.Invoke(message, "danger");
    }
}