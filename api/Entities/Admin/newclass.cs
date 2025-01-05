using DocumentFormat.OpenXml.Bibliography;

namespace api.Entities.Admin
{
    public class FCNMessage
    {
        public string Topic { get; set; }
        public Notification Notification { get; set; } = new Notification();
        public AndroidConfig Android { get; set; }

    
    }

    public class Notification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        
    }

    public class AndroidConfig
    {
        public TimeSpan TimeToLive { get; set;} = TimeSpan.FromHours(1);
        public AndroidNotification Notification { get; set; }=new AndroidNotification();
    }

    public class AndroidNotification 
    {
        public string Icon { get; set; }="stock_ticker_update";
        public string Color { get; set; }="#f45342";
    }

}
