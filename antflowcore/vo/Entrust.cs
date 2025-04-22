namespace AntFlowCore.Vo
{
    public class Entrust
    {
        // Start time
        public DateTime? BeginTime { get; set; }

        public string PowerId { get; set; }

        // End time
        public DateTime? EndTime { get; set; }

        public string Sender { get; set; }

        // Entrust name
        public string Name { get; set; }

        public int Id { get; set; }

        // Receiver ID
        public string ReceiverId { get; set; }

        // Receiver name
        public string ReceiverName { get; set; }

        // Creation time
        public DateTime? CreateTime { get; set; }
    }
}