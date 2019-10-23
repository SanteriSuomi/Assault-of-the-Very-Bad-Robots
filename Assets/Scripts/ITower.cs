public interface ITower
{
    string Name { get; set; }
    int Cost { get; set; }
    float Damage { get; set; }
    void IsPlacing(bool enable);
}