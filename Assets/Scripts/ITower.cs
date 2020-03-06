namespace AOTVBR
{
    public interface ITower
    {
        int Cost { get; set; }
        float Damage { get; set; }
        void IsPlacing(bool enable);
    } 
}