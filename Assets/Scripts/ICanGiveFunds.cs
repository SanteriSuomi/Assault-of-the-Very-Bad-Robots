namespace AOTVBR
{
    public interface ICanGiveFunds
    {
        float FundAmount { get; set; }
        void GiveFunds();
    }
}