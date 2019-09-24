
namespace Fizz.Ingestion
{
    public interface IFizzIngestionClient
    {
        string BuildVer { get; set; }
        string CustomDimesion01 { get; set; }
        string CustomDimesion02 { get; set; }
        string CustomDimesion03 { get; set; }

        void ProductPurchased(string productId, double amount, string currency, string receipt);
        void TextMessageSent(string channelId, string content, string senderNick);
    }
}
