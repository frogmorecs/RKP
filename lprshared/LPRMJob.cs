namespace lprshared
{
    public class LPRMJob
    {
        [Parameter("-S", Required = true)]
        public string Server { get; set; }

        [Parameter("-P", Required = true, AllowSpaces = false)]
        public string Printer { get; set; }

        [Parameter("-J", Required = true)]
        public int Id { get; set; }
    }
}