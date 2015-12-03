namespace lprshared
{
    public class LPQJob
    {
        [Parameter("-S", Required = true)]
        public string Server { get; set; }

        [Parameter("-P", Required = true, AllowSpaces = false)]
        public string Printer { get; set; }

        [Parameter("-l")]
        public bool Verbose { get; set; }

        protected bool Equals(LPQJob other)
        {
            return string.Equals(Server, other.Server) && string.Equals(Printer, other.Printer) && Verbose == other.Verbose;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((LPQJob) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                var hashCode = Server?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (Printer?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ Verbose.GetHashCode();
                return hashCode;
            }
        }
    }
}