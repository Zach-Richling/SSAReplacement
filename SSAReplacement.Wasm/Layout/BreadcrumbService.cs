namespace SSAReplacement.Wasm.Layout;

public class BreadcrumbService
{
    public event Action? OnChange;

    public List<BreadcrumbItem> Items { get; private set; } = [];

    public BreadcrumbItem BaseItem { get; set; } = new BreadcrumbItem { Text = "Home", Url = "/" };

    public void Set(params BreadcrumbItem[] items)
    {
        Items.Clear();
        Items.Add(BaseItem);
        Items.AddRange(items);
        OnChange?.Invoke();
    }
}

public class BreadcrumbItem
{
    public string Text { get; set; } = "";
    public string? Url { get; set; }
}
