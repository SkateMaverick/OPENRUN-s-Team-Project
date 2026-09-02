public interface IPressable
{
    bool IsPressed { get; set; }

    void OnPress();
}
