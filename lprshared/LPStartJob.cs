namespace lprshared
{
    public class LPStartJob
    {
        [Parameter("-S", Required = true)]
        public string Server { get; set; }

        [Parameter("-P", Required = true, AllowSpaces = false)]
        public string Printer { get; set; }
    }
}