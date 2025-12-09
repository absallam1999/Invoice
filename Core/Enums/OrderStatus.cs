using System.Text.Json.Serialization;

namespace invoice.Core.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {

        Delivered,  //0

        WaitingReview, //1

        Processing,   //2

        InDelivery,  //3


        Refunded,  //4


    }
}